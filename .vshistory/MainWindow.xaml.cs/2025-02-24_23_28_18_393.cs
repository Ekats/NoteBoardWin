using System;
using System.Windows;
using NoteboardWPFUI.Views.Pages;

namespace NoteboardWPFUI
{
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        private ClipboardMonitor? _clipboardMonitor;

        public MainWindow()
        {
            InitializeComponent();

            Topmost = true;
            Width = 400;
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ShowInTaskbar = false;

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
            Hide(); // Hide the window while keeping the tray icon visible.
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
