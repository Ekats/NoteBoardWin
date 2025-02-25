using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using Wpf.Ui.Tray.Controls;

namespace NoteboardWPFUI
{
    public partial class App : Application
    {
        public static NotifyIcon? TrayIcon { get; private set; }
        private ContextMenu? _trayContextMenu;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create tray icon programmatically.
            TrayIcon = new NotifyIcon
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/applicationIcon-256.png")),
                Visibility = Visibility.Visible,
                ToolTip = "NoteboardWPFUI"
            };

            TrayIcon.LeftClick += TrayIcon_LeftClick;
            TrayIcon.RightClick += TrayIcon_RightClick;
        }

#nullable disable
        private void TrayIcon_LeftClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            }
        }

        private void TrayIcon_RightClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _trayContextMenu = new ContextMenu
                {
                    Placement = PlacementMode.MousePoint,
                    PlacementTarget = Application.Current.MainWindow, // Helps anchor the menu
                    MinWidth = 150,
                    MinHeight = 40,
                    Background = System.Windows.Media.Brushes.White,
                    Foreground = System.Windows.Media.Brushes.Black
                };

                var showItem = new MenuItem
                {
                    Header = "Show",
                    Height = 30
                };
                showItem.Click += (s, args) =>
                {
                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.Show();
                        mainWindow.WindowState = WindowState.Normal;
                        mainWindow.Activate();
                    }
                };

                var settingsItem = new MenuItem
                {
                    Header = "Settings",
                    Height = 30
                };
                settingsItem.Click += (s, args) =>
                {
                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.Show();
                        mainWindow.WindowState = WindowState.Normal;
                        mainWindow.Activate();
                        mainWindow.RootNavigation.Navigate(typeof(Views.Pages.SettingsPage));
                    }
                };

                var closeItem = new MenuItem
                {
                    Header = "Close",
                    Height = 30
                };
                closeItem.Click += (s, args) => Shutdown();

                _trayContextMenu.Items.Add(showItem);
                _trayContextMenu.Items.Add(settingsItem);
                _trayContextMenu.Items.Add(new Separator());
                _trayContextMenu.Items.Add(closeItem);

                _trayContextMenu.IsOpen = true;
            });
        }
#nullable restore




        // Dummy handlers to satisfy App.xaml
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Handle exceptions as needed.
            e.Handled = true;
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            // Cleanup if needed.
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            // Additional startup code.
        }
    }
}
