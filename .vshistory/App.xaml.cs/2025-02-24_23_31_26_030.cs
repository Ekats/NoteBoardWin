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
        // Create a global tray icon instance.
        public static NotifyIcon AppTrayIcon { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize tray icon independently of MainWindow.
            AppTrayIcon = new NotifyIcon
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/applicationIcon-256.png")),
                Visibility = Visibility.Visible,
                ToolTip = "NoteboardWPFUI"
            };

            AppTrayIcon.LeftClick += TrayIcon_LeftClick;
        }
#nullable disable
        private void TrayIcon_LeftClick(NotifyIcon s, RoutedEventArgs e)
        {
            // Show MainWindow when tray icon is left-clicked.
            if (Current.MainWindow is not null)
            {
                Current.MainWindow.Show();
                Current.MainWindow.WindowState = WindowState.Normal;
                Current.MainWindow.Activate();
            }
        }
#nullable restore
    }
}
