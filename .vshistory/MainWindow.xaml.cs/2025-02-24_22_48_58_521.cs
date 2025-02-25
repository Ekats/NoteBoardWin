using System;
using System.Windows;
using NoteboardWPFUI.Views.Pages;

namespace NoteboardWPFUI
{
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        private ClipboardMonitor _clipboardMonitor;

        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            this.Width = 400;
            this.Height = 500;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Loaded += MainWindow_Loaded;
            SourceInitialized += MainWindow_SourceInitialized;
        }


        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            _clipboardMonitor = new ClipboardMonitor(this);
            _clipboardMonitor.ClipboardUpdated += OnClipboardUpdated;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(ClipsPage));
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
