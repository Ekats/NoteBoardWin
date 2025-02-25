using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using NoteboardWPFUI.Views.Pages;
using Wpf.Ui.Tray;
using Wpf.Ui.Tray.Controls;

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
            this.Topmost = true;
            this.Width = 400;
            this.Height = 500;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ShowInTaskbar = false; // Remove from taskbar

            // Create tray icon programmatically so it’s independent of the window’s visual tree.
            _appNotifyIcon = new NotifyIcon
            {
                Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/applicationIcon-256.png")),
                IsVisible = true,
                Text = "NoteboardWPFUI"
            };
            _appNotifyIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                    this.Activate();
                }
            };

            Loaded += MainWindow_Loaded;
            SourceInitialized += MainWindow_SourceInitialized;
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
            Hide(); // Hide the window so only the tray icon is visible
        }

        private void AppNotifyIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void OnClipboardUpdated(object? sender, string text)
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
