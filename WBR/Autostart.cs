using Microsoft.Win32;
using System;

namespace WBR
{
    /// <summary>
    /// "Start with Windows" for the current user via HKCU\...\Run.
    /// (No admin rights needed; visible and switchable in Task Manager > Startup apps.)
    /// </summary>
    static class Autostart
    {
        const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        // Same value name the old built-in startup code used, so an existing entry is
        // picked up (shown as checked in the tray menu) instead of being left behind.
        const string ValueName = "WBR";

        static string Command
        {
            get { return "\"" + Environment.ProcessPath + "\""; }
        }

        public static bool IsEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey, false))
                {
                    string value = key == null ? null : key.GetValue(ValueName) as string;
                    return !String.IsNullOrEmpty(value);
                }
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
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RunKey))
                {
                    if (key == null)
                    {
                        return false;
                    }

                    if (enable)
                    {
                        key.SetValue(ValueName, Command, RegistryValueKind.String);
                    }
                    else
                    {
                        key.DeleteValue(ValueName, false);
                    }
                }

                ErrorHandler.NewError("Autostart " + (enable ? "enabled: " + Command : "disabled"));
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.NewError("Autostart.SetEnabled failed");
                ErrorHandler.NewError(ex);
                return false;
            }
        }

        /// <summary>
        /// If autostart is on but points to a different location (exe was moved/updated), fix the path.
        /// </summary>
        public static void RefreshPathIfEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey, true))
                {
                    string value = key == null ? null : key.GetValue(ValueName) as string;

                    if (!String.IsNullOrEmpty(value) && !String.Equals(value, Command, StringComparison.OrdinalIgnoreCase))
                    {
                        key.SetValue(ValueName, Command, RegistryValueKind.String);
                        ErrorHandler.NewError("Autostart path updated to " + Command);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.NewError("Autostart.RefreshPathIfEnabled failed");
                ErrorHandler.NewError(ex);
            }
        }
    }
}
