using System;
using System.Windows;
using System.Windows.Input;
using NoteboardWPFUI.Views.Pages;
using Wpf.Ui.Controls.Tray;  // Added

namespace NoteboardWPFUI
{
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        private ClipboardMonitor? _clipboardMonitor;

        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            this.Width = 400;
            this.Height = 500;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Loaded += MainWindow_Loaded;
            SourceInitialized += MainWindow_SourceInitialized;

            // Retrieve tray icon and attach event handler
            var notifyIcon = FindName("AppNotifyIcon") as NotifyIcon;
            if (notifyIcon != null)
            {
                notifyIcon.MouseLeftButtonUp += AppNotifyIcon_MouseLeftButtonUp;
            }
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            _clipboardMonitor = new ClipboardMonitor(this);
            _clipboardMonitor.ClipboardUpdated += OnClipboardUpdated;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(ClipsPage));
            Hide(); // Hide window so only tray icon is visible
        }

        private void AppNotifyIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void OnClipboardUpdated(object sender, string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    Models.ClipManager.Clips.Add(new Models.ClipItem { Text = text });
                }
            });
        }
    }
}
