using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WBR
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WBR.MainWindow window = new WBR.MainWindow();
            MainWindow = window;

            // Started by "Start with Windows": the window is never shown, the app just sits in the
            // system tray (click the tray icon / "Open" to show it).
            if (!Autostart.IsMinimizedLaunch())
            {
                window.Show();
            }
        }
    }

}
