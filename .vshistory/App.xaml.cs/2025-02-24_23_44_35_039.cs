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

        private void TrayIcon_LeftClick(NotifyIcon sender, RoutedEventArgs e)
        {
            if (Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            }
        }

        private void TrayIcon_RightClick(NotifyIcon sender, RoutedEventArgs e)
        {
            // Build a context menu programmatically.
            var contextMenu = new ContextMenu
            {
                Placement = PlacementMode.MousePoint,
                Background = System.Windows.Media.Brushes.White,
                Foreground = System.Windows.Media.Brushes.Black
            };

            var showItem = new MenuItem { Header = "Show" };
            showItem.Click += (s, args) =>
            {
                if (Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.Show();
                    mainWindow.WindowState = WindowState.Normal;
                    mainWindow.Activate();
                }
            };

            var settingsItem = new MenuItem { Header = "Settings" };
            settingsItem.Click += (s, args) =>
            {
                if (Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.Show();
                    mainWindow.WindowState = WindowState.Normal;
                    mainWindow.Activate();
                    mainWindow.RootNavigation.Navigate(typeof(Views.Pages.SettingsPage));
                }
            };

            var closeItem = new MenuItem { Header = "Close" };
            closeItem.Click += (s, args) =>
            {
                Shutdown();
            };

            contextMenu.Items.Add(showItem);
            contextMenu.Items.Add(settingsItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(closeItem);

            contextMenu.IsOpen = true;
        }


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
