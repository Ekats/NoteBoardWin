using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using NoteboardWPFUI.Views.Pages;
using Wpf.Ui.Tray.Controls; // Ensure your project references the Wpf.Ui.Tray package

namespace NoteboardWPFUI
{
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        private ClipboardMonitor? _clipboardMonitor;
        private readonly NotifyIcon _appNotifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            // Force window properties
            Topmost = true;
            Width = 400;
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ShowInTaskbar = false;

            // Create the tray icon programmatically so it’s independent of the window’s visual tree.
            _appNotifyIcon = new NotifyIcon
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/applicationIcon-256.png")),
                Visibility = Visibility.Visible,
                ToolTip = "NoteboardWPFUI"
            };
            _appNotifyIcon.LeftClick += AppNotifyIcon_LeftClick;

            Loaded += MainWindow_Loaded;
            SourceInitialized += MainWindow_SourceInitialized;
        }

        private void AppNotifyIcon_LeftClick(NotifyIcon s, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void MainWindow_SourceInitialized(object? sender, EventArgs e)
        {
            if (sender is MainWindow window)
            {
                _clipboardMonitor = new ClipboardMonitor(window);
                _clipboardMonitor.ClipboardUpdated += OnClipboardUpdated;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(ClipsPage));
            //Hide(); // Hide the window so only the tray icon is visible
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
