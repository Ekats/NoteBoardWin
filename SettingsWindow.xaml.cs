using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NoteBoard
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public bool EnableDarkMode { get; set; }
        public bool EnableNotifications { get; set; }
        public bool EnableAutoPaste { get; set; }
        public bool EnableAlwaysOnTop { get; set; }
        public bool EnableCloseOnFocusLost { get; set; }

        private readonly MainWindow _mainWindow;
        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            // Load the current values
            EnableDarkMode = Settings.Default.EnableDarkMode;
            EnableNotifications = Settings.Default.EnableNotifications;
            EnableAutoPaste = Settings.Default.EnableAutoPaste;
            EnableAlwaysOnTop = Settings.Default.EnableAlwaysOnTop;
            EnableCloseOnFocusLost = Settings.Default.CloseOnFocusLost;

            // Bind data context for TwoWay binding
            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save the settings to storage
            Settings.Default.EnableDarkMode = EnableDarkMode;
            Settings.Default.EnableNotifications = EnableNotifications;
            Settings.Default.EnableAutoPaste = EnableAutoPaste;
            Settings.Default.EnableAlwaysOnTop = EnableAlwaysOnTop;
            Settings.Default.CloseOnFocusLost = EnableCloseOnFocusLost;
            Settings.Default.Save();
            if (_mainWindow != null)
            {
                _mainWindow.SetAlwaysOnTop(AlwaysOnTopCheckbox.IsChecked ?? false);
            }
            // Close the window
            this.Close();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

    }
}
