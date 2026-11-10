using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Principal;
using System.Text;

namespace WBR
{
    /// <summary>
    /// "Start with Windows" for WBR.
    ///
    /// WBR's manifest requires administrator rights. Windows silently ignores HKCU\...\Run entries
    /// of programs that need elevation at logon, so a Run key (or the Startup folder) never works
    /// for this app. Instead a Task Scheduler task "At log on of this user, run with highest
    /// privileges" is used: it starts WBR elevated without a UAC prompt.
    /// (Visible and switchable in Task Scheduler and in Task Manager > Startup apps.)
    /// </summary>
    static class Autostart
    {
        const string TaskName = "WBR";

        /// <summary>Command-line switch the task passes so WBR starts hidden in the system tray.</summary>
        public const string MinimizedArg = "--minimized";

        // Old versions wrote a Run-key value with this name; it never worked for an elevated app.
        const string LegacyRunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        const string LegacyValueName = "WBR";

        public static bool IsMinimizedLaunch()
        {
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (String.Equals(arg, MinimizedArg, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        static string ExePath
        {
            get { return Environment.ProcessPath; }
        }

        public static bool IsEnabled()
        {
            try
            {
                return RunSchtasks("/Query /TN \"" + TaskName + "\"") == 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.NewError("Autostart.IsEnabled failed");
                ErrorHandler.NewError(ex);
                return false;
            }
        }

        public static bool SetEnabled(bool enable)
        {
            try
            {
                bool ok;
                if (enable)
                {
                    ok = CreateTask();
                    if (ok) RemoveLegacyRunValue();
                }
                else
                {
                    // exit code != 0 just means the task did not exist
                    RunSchtasks("/Delete /TN \"" + TaskName + "\" /F");
                    RemoveLegacyRunValue();
                    ok = !IsEnabled();
                }

                ErrorHandler.NewError("Autostart " + (enable ? "enabled: " + ExePath : "disabled") + (ok ? "" : " (FAILED)"));
                return ok;
            }
            catch (Exception ex)
            {
                ErrorHandler.NewError("Autostart.SetEnabled failed");
                ErrorHandler.NewError(ex);
                return false;
            }
        }

        /// <summary>
        /// - If the old Run-key value exists, the user had autostart on: migrate it to the task.
        /// - If the task exists but points to a different location (exe moved/updated) or lacks the
        ///   start-minimized switch (task made by an earlier build), recreate it.
        /// </summary>
        public static void RefreshPathIfEnabled()
        {
            try
            {
                bool hadLegacy = HasLegacyRunValue();
                bool enabled = IsEnabled();

                if (enabled)
                {
                    string xml = QuerySchtasks("/Query /TN \"" + TaskName + "\" /XML");
                    if (xml == null
                        || xml.IndexOf(ExePath, StringComparison.OrdinalIgnoreCase) < 0
                        || xml.IndexOf(MinimizedArg, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        CreateTask();
                        ErrorHandler.NewError("Autostart task updated for " + ExePath);
                    }
                }
                else if (hadLegacy)
                {
                    if (CreateTask())
                    {
                        ErrorHandler.NewError("Autostart migrated from Run key to scheduled task");
                    }
                }

                if (hadLegacy) RemoveLegacyRunValue();
            }
            catch (Exception ex)
            {
                ErrorHandler.NewError("Autostart.RefreshPathIfEnabled failed");
                ErrorHandler.NewError(ex);
            }
        }

        // ---- scheduled task -------------------------------------------------------------------

        static bool CreateTask()
        {
            string user = WindowsIdentity.GetCurrent().Name;
            string exe = ExePath;
            string dir = Path.GetDirectoryName(exe);

            string xml =
"<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n" +
"<Task version=\"1.2\" xmlns=\"http://schemas.microsoft.com/windows/2004/02/mit/task\">\r\n" +
"  <RegistrationInfo><Description>Start WBR (Wireless Button Reprogrammer) when you log on</Description></RegistrationInfo>\r\n" +
"  <Triggers>\r\n" +
"    <LogonTrigger>\r\n" +
"      <Enabled>true</Enabled>\r\n" +
"      <UserId>" + SecurityElement.Escape(user) + "</UserId>\r\n" +
"      <Delay>PT10S</Delay>\r\n" +
"    </LogonTrigger>\r\n" +
"  </Triggers>\r\n" +
"  <Principals>\r\n" +
"    <Principal id=\"Author\">\r\n" +
"      <UserId>" + SecurityElement.Escape(user) + "</UserId>\r\n" +
"      <LogonType>InteractiveToken</LogonType>\r\n" +
"      <RunLevel>HighestAvailable</RunLevel>\r\n" +
"    </Principal>\r\n" +
"  </Principals>\r\n" +
"  <Settings>\r\n" +
"    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>\r\n" +
"    <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>\r\n" +
"    <StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>\r\n" +
"    <AllowHardTerminate>true</AllowHardTerminate>\r\n" +
"    <StartWhenAvailable>true</StartWhenAvailable>\r\n" +
"    <ExecutionTimeLimit>PT0S</ExecutionTimeLimit>\r\n" +
"    <Priority>7</Priority>\r\n" +
"  </Settings>\r\n" +
"  <Actions Context=\"Author\">\r\n" +
"    <Exec>\r\n" +
"      <Command>" + SecurityElement.Escape(exe) + "</Command>\r\n" +
"      <Arguments>" + MinimizedArg + "</Arguments>\r\n" +
"      <WorkingDirectory>" + SecurityElement.Escape(dir) + "</WorkingDirectory>\r\n" +
"    </Exec>\r\n" +
"  </Actions>\r\n" +
"</Task>\r\n";

            string tmp = Path.Combine(Path.GetTempPath(), "WBR_task_" + Guid.NewGuid().ToString("N") + ".xml");
            try
            {
                File.WriteAllText(tmp, xml, Encoding.Unicode); // schtasks wants UTF-16
                int code = RunSchtasks("/Create /TN \"" + TaskName + "\" /XML \"" + tmp + "\" /F");
                if (code != 0) ErrorHandler.NewError("schtasks /Create failed, exit code " + code);
                return code == 0;
            }
            finally
            {
                try { File.Delete(tmp); } catch { }
            }
        }

        static int RunSchtasks(string args)
        {
            using (Process p = StartSchtasks(args))
            {
                p.StandardOutput.ReadToEnd();
                p.StandardError.ReadToEnd();
                p.WaitForExit();
                return p.ExitCode;
            }
        }

        static string QuerySchtasks(string args)
        {
            using (Process p = StartSchtasks(args))
            {
                string output = p.StandardOutput.ReadToEnd();
                p.StandardError.ReadToEnd();
                p.WaitForExit();
                return p.ExitCode == 0 ? output : null;
            }
        }

        static Process StartSchtasks(string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo("schtasks.exe", args);
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            return Process.Start(psi);
        }

        // ---- legacy Run key -------------------------------------------------------------------

        static bool HasLegacyRunValue()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(LegacyRunKey, false))
            {
                string value = key == null ? null : key.GetValue(LegacyValueName) as string;
                return !String.IsNullOrEmpty(value);
            }
        }

        static void RemoveLegacyRunValue()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(LegacyRunKey, true))
                {
                    if (key != null) key.DeleteValue(LegacyValueName, false);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.NewError(ex);
            }
        }
    }
}
