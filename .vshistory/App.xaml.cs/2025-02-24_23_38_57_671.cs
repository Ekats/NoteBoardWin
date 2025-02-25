// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Wpf.Ui.Tray.Controls;

namespace NoteboardWPFUI
{
    public partial class App : Application
    {
        public static NotifyIcon? TrayIcon { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create the tray icon programmatically.
            TrayIcon = new NotifyIcon
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/applicationIcon-256.png")),
                Visibility = Visibility.Visible,
                ToolTip = "NoteboardWPFUI"
            };
            TrayIcon.LeftClick += TrayIcon_LeftClick;
        }

        private void TrayIcon_LeftClick(NotifyIcon? sender, RoutedEventArgs e)
        {
            if (Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            }
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Log and/or display the exception.
            e.Handled = true;
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            // Perform cleanup if necessary.
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            // Additional startup code if required.
        }
    }
}
