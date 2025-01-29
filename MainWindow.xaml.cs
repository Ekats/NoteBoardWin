using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Threading;
namespace NoteBoard
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }
    public partial class MainWindow : Window
    {
        // 1. Constants and DLL Imports
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        private string _lastClipboardText = string.Empty;
        private System.Windows.Controls.Button? _latestEntryButton = null;
        private bool _isEyedropperActive = false;

        // 2. Constructor
        public MainWindow()
        {
            InitializeComponent();
            this.Deactivated += MainWindow_Deactivated;
        }
        public void SetAlwaysOnTop(bool isTopmost)
        {
            this.Topmost = isTopmost;
        }
        private void MainWindow_Deactivated(object? sender, EventArgs e)
        {
            // Check the user setting and if Eyedropper is not active
            if (Settings.Default.CloseOnFocusLost && !_isEyedropperActive)
            {
                // Hide (go to tray) rather than closing the application
                this.Hide();
            }
        }

        // This method is called when the left mouse button is pressed on the title bar
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Check if the mouse is pressed on the title bar area
            // DragMove() should be called on the window, and it only works when the left mouse button is down.
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Close button handler
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Instead of closing the window, hide it.
            this.Hide();
        }

        // 3. Window Source Initialization (Register for Clipboard Messages)
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get the window handle (HWND) for this WPF window
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // Register to receive WM_CLIPBOARDUPDATE message
            AddClipboardFormatListener(hwnd);

            // Attach a message hook to process WM_CLIPBOARDUPDATE
            HwndSource source = HwndSource.FromHwnd(hwnd);
            if (source != null)
            {
                source.AddHook(WndProc);
            }
        }

        // 4. Custom WndProc to Handle WM_CLIPBOARDUPDATE
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // If the clipboard has changed, retrieve the new content
            if (msg == WM_CLIPBOARDUPDATE)
            {
                OnClipboardChanged();
                handled = true;
            }
            return IntPtr.Zero;
        }

        // 5. Retrieve Clipboard Data in WPF
        private void OnClipboardChanged()
        {
            try
            {
                // Grab text if it's available
                if (System.Windows.Clipboard.ContainsText())
                {
                    string newText = TryGetClipboardText();
                    if (string.IsNullOrEmpty(newText))
                    {
                        // If we can't read the clipboard after retries, skip,
                        // Log an error if clipboard text couldn't be retrieved after retries
                        var errorEntry = new TextBlock
                        {
                            Text = $"[{DateTime.Now:T}] Error: Clipboard locked. Could not read text.",
                            Foreground = System.Windows.Media.Brushes.Red,
                            TextWrapping = TextWrapping.Wrap
                        };
                        ClipboardBoard.Children.Add(errorEntry);
                        return;
                    }

                    if (newText == _lastClipboardText)
                        return;

                    // 1) Revert the old latest button
                    if (_latestEntryButton != null)
                    {
                        _latestEntryButton.Style = (Style)FindResource("ClipboardEntryButtonStyle");
                    }


                    _lastClipboardText = newText; // Update the last known text
                                                  // Append it to our TextBox log
                                                  // Now we create a new "log instance" each time
                                                  // 1. Create a border for background
                                                  // --- HIGHLIGHT LOGIC (1/2): Revert old "latest" to normal ---

                    // Create the new button
                    var entryButton = new System.Windows.Controls.Button
                    {
                        // Instead of the normal ClipboardEntryButtonStyle,
                        // we will momentarily set it to highlight below:
                        Style = (Style)FindResource("LatestClipboardEntryButtonStyle"),
                        Tag = newText,
                        Content = new TextBlock
                        {
                            Text = $"[{DateTime.Now:T}] {newText}",
                            Foreground = System.Windows.Media.Brushes.White,
                            TextWrapping = TextWrapping.Wrap
                        }
                    };

                    _latestEntryButton = entryButton;

                    // Handle the click event to set clipboard/paste, etc.
                    entryButton.Click += (s, e) =>
                    {
                        if (s is System.Windows.Controls.Button clickedButton && clickedButton.Tag is string entryText)
                        {
                            System.Windows.Clipboard.SetText(entryText);
                            this.Hide();
                            
                            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
                            timer.Tick += (o, args) =>
                            {
                                timer.Stop();
                                System.Windows.Forms.SendKeys.SendWait("^v");
                            };

                            timer.Start();
                        }
                    };

                    if (TryGetColorFromHex(newText, out System.Windows.Media.Color parsedColor))
                    {
                        // 1) Build a horizontal StackPanel
                        var panel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };

                        // 2) Create a small 16x16 rectangle to show the color
                        var rect = new System.Windows.Shapes.Rectangle
                        {
                            Width = 16,
                            Height = 16,
                            Margin = new Thickness(0, 0, 5, 0),
                            Fill = new SolidColorBrush(parsedColor)
                        };

                        // 3) Create a text block
                        var textBlock = new TextBlock
                        {
                            Text = $"[{DateTime.Now:T}] {newText}",
                            Foreground = System.Windows.Media.Brushes.White,
                            TextWrapping = TextWrapping.Wrap
                        };

                        // 4) Add them to the panel
                        panel.Children.Add(rect);
                        panel.Children.Add(textBlock);

                        // 5) Set the panel as the button's content
                        entryButton.Content = panel;
                    }

                    // Add the new button to the ClipboardBoard
                    ClipboardBoard.Children.Insert(0, entryButton);

                }
                else
                {
                                // For non-text data, create a different log entry
                    var newLogEntry = new TextBlock
                    {
                        Text = $"[{DateTime.Now:T}] Non-text data.",
                        Margin = new Thickness(0, 0, 0, 5),
                        TextWrapping = TextWrapping.Wrap
                    };
                    ClipboardBoard.Children.Insert(0, newLogEntry);
                }
            }
            catch (Exception ex)
            {
                // In case of any exception (e.g. clipboard locked), log an error message.
                var errorEntry = new TextBlock
                {
                    Text = $"Error reading clipboard: {ex.Message}",
                    Margin = new Thickness(0, 0, 0, 5),
                    Foreground = System.Windows.Media.Brushes.Red,
                    TextWrapping = TextWrapping.Wrap
                };
                ClipboardBoard.Children.Add(errorEntry);
            }
        }

        // 6. Unregister on Window Close
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Get HWND again to remove listener
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            RemoveClipboardFormatListener(hwnd);
        }
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWin = new SettingsWindow(this);

            // Position the settings window relative to NoteBoard
            settingsWin.Left = this.Left + 20; // Offset slightly to the right
            settingsWin.Top = this.Top + 20;  // Offset slightly downward

            settingsWin.Owner = this; // Optional: make it modal and tied to this window
            settingsWin.ShowDialog(); // Show as a modal dialog
        }
        private bool _isNotesView = false;  // false means start on Clipboard view

        private void NotesButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the bool
            _isNotesView = !_isNotesView;

            if (_isNotesView)
            {
                // Switch to NOTES tab
                ClipboardBoard.Visibility = Visibility.Collapsed;
                NotesBoard.Visibility = Visibility.Visible;

                // Change icon to a "clipboard" icon for going back
                NotesButtonIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/clipboard.ico", UriKind.RelativeOrAbsolute));
            }
            else
            {
                // Switch back to CLIPBOARD tab
                NotesBoard.Visibility = Visibility.Collapsed;
                ClipboardBoard.Visibility = Visibility.Visible;

                // Change icon to the "stickies" (notes) icon
                NotesButtonIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/stickies.ico", UriKind.RelativeOrAbsolute));
            }
        }

        private string? TryGetClipboardText()
        {
            const int maxRetries = 3;
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    return System.Windows.Clipboard.GetText();
                }
                catch
                {
                    // Clipboard is locked, wait briefly then retry
                    System.Threading.Thread.Sleep(50);
                }
            }
            // If still locked after retries, return null or empty
            return null;
        }

        private void EyeDropperButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var scr in System.Windows.Forms.Screen.AllScreens)
            {
                var overlay = new ColorPickerOverlay(scr);
                // Show modeless or store them in a list until one picks a color
                overlay.Show();
            }
        }


        private bool TryGetColorFromHex(string input, out System.Windows.Media.Color color)
        {
            color = default;

            // Quick pattern check: must start with # and have 7 chars total (# + 6 hex digits)
            if (string.IsNullOrWhiteSpace(input) || input.Length != 7 || !input.StartsWith("#"))
                return false;

            // Then try WPF’s ColorConverter
            try
            {
                var converted = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(input);
                color = converted;
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
