using NoteBoard;
using System;
using System.Runtime.InteropServices;
using System.Windows;             // WPF Application
using System.Windows.Interop;     // For HwndSource
using WF = System.Windows.Forms;  // Alias for WinForms
using Application = System.Windows.Application; // Disambiguate "Application"
using Hardcodet.Wpf.TaskbarNotification;

namespace NoteBoard
{
    public partial class App : Application
    {
        // NotifyIcon for system tray
        //private WF.NotifyIcon? _trayIcon;
        private TaskbarIcon? _trayIcon;
        private WF.ContextMenuStrip? _trayMenu = null;
        private System.Windows.Controls.ContextMenu? _customContextMenu;
        // Hotkey constants and Win32 APIs
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 0x9999; // arbitrary
        private const uint MOD_ALT = 0x0001;  // "Alt" key modifier
        private const uint VK_X = 0x58;       // 'X' virtual key code

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT { public int X; public int Y; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

                // Create the main window.
            MainWindow = new MainWindow();
            
                // Show the window briefly to force HWND creation, then hide it.
            MainWindow.Show();
                // Now the window has a valid handle.
            MainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            MainWindow.ShowInTaskbar = false;
            
            IntPtr hwnd = new WindowInteropHelper(MainWindow).Handle;
            HwndSource source = HwndSource.FromHwnd(hwnd);
            if (source != null)
            {
                source.AddHook(WndProc);
                RegisterHotKey(hwnd, HOTKEY_ID, MOD_ALT, VK_X);
            }

            // Create and show tray icon
            _trayIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon("NoteBoard.ico"),
                ToolTipText = "NoteBoard"
            };
            // Hide the window now that the hotkey is registered.
            MainWindow.Hide();

            // Create Context Menu
            var contextMenu = new System.Windows.Controls.ContextMenu();
            var exitItem = new System.Windows.Controls.MenuItem { Header = "Exit" };
            exitItem.Click += ExitApp_Click;
            _trayIcon.MouseRightButtonUp += TrayIcon_MouseRightButtonUp;
            contextMenu.Items.Add(exitItem);
            contextMenu.HorizontalOffset = RealCursorPosition()[0];
            contextMenu.VerticalOffset = RealCursorPosition()[1];

            // Attach the context menu to the tray icon
            _trayIcon.ContextMenu = contextMenu;
            _customContextMenu = contextMenu;
        }

        // Right-click "Exit" handler
        private void ExitApp_Click(object? sender, EventArgs e)
        {
            // Fully close the application
            Application.Current.Shutdown();
        }
        private void TrayIcon_MouseRightButtonUp(object sender, RoutedEventArgs e)
        {
            ShowCustomContextMenu();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Unregister hotkey
            IntPtr hwnd = new WindowInteropHelper(MainWindow).Handle;
            UnregisterHotKey(hwnd, HOTKEY_ID);

            // Dispose tray icon
            if (_trayIcon != null)
            {
                _trayIcon?.Dispose();
            }

            base.OnExit(e);
        }

        // 5. Hook to catch WM_HOTKEY messages
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_DPICHANGED = 0x02E0;

            switch (msg)
            {
                case WM_HOTKEY:
                    if (wParam.ToInt32() == HOTKEY_ID)
                    {
                        ShowWindowNearCursor();
                        handled = true;
                    }
                    break;
                case WM_DPICHANGED:
                    // Retrieve the new DPI
                    int dpiX = (short)((lParam.ToInt32() >> 16) & 0xFFFF);
                    int dpiY = (short)(lParam.ToInt32() & 0xFFFF);
                    // Adjust window scaling if necessary
                    // Example: Update layout or sizes based on new DPI
                    break;
            }

            return IntPtr.Zero;
        }

        // 6. Show the main window near the current mouse cursor
        private void ShowWindowNearCursor()
        {
            if (GetCursorPos(out POINT cursorPos))
            {
                // Get the screen where the cursor is located
                var screen = Screen.FromPoint(new System.Drawing.Point(cursorPos.X, cursorPos.Y));

                // Get DPI for the target monitor
                var source = PresentationSource.FromVisual(MainWindow);
                double dpiX = 96.0, dpiY = 96.0; // Default DPI

                if (source != null)
                {
                    dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                    dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                }
                else
                {
                    // Fallback: Get DPI using Graphics (for initial startup)
                    using (var g = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        dpiX = g.DpiX;
                        dpiY = g.DpiY;
                    }
                }

                // Convert cursor position to WPF DIPs
                var dipPoint = new System.Windows.Point(cursorPos.X * 96.0 / dpiX, cursorPos.Y * 96.0 / dpiY);

                // Set window position
                MainWindow.Left = dipPoint.X;
                MainWindow.Top = dipPoint.Y;

                // Optionally adjust size based on DPI
                MainWindow.Width = 500 * 96.0 / dpiX;  // Example scaling
                MainWindow.Height = 600 * 96.0 / dpiY;
            }

            MainWindow.Show();
            MainWindow.Activate();
        }

        public float[] RealCursorPosition()
        {
            if (GetCursorPos(out POINT cursorPos))
            {
                // Get the screen where the cursor is located
                var screen = Screen.FromPoint(new System.Drawing.Point(cursorPos.X, cursorPos.Y));

                // Get DPI for the target monitor
                var source = PresentationSource.FromVisual(MainWindow);
                double dpiX = 96.0, dpiY = 96.0; // Default DPI

                if (source != null)
                {
                    dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                    dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                }
                else
                {
                    // Fallback: Get DPI using Graphics (for initial startup)
                    using (var g = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        dpiX = g.DpiX;
                        dpiY = g.DpiY;
                    }
                }
                var dipPoint = new System.Windows.Point(cursorPos.X * 96.0 / dpiX, cursorPos.Y * 96.0 / dpiY);
                return [(float)dipPoint.X, (float)dipPoint.Y];
            }
            else
            {
                // Fallback: Get DPI using Graphics (for initial startup)
                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    return [g.DpiX, g.DpiY]; //X = width Y = height
                }
            }
        }



        private void ShowCustomContextMenu()
        {
            if (_customContextMenu == null || _trayIcon == null)
                return;

            // Get cursor position
            POINT cursorPos;
            if (!GetCursorPos(out cursorPos))
                return;

            // Get DPI for the monitor
            IntPtr monitor = MonitorFromPoint(cursorPos, MONITOR_DEFAULTTONEAREST);
            uint dpiX, dpiY;
            GetDpiForMonitor(monitor, MonitorDpiType.MDT_EFFECTIVE_DPI, out dpiX, out dpiY);
            double scaleX = dpiX / 96.0;
            double scaleY = dpiY / 96.0;

            // Convert cursor position to WPF DIPs
            double dipX = cursorPos.X / scaleX;
            double dipY = cursorPos.Y / scaleY;

            // Set Placement
            _customContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.AbsolutePoint;
            _customContextMenu.HorizontalOffset = dipX;
            _customContextMenu.VerticalOffset = dipY;

            _customContextMenu.IsOpen = true;
        }



        // Additional P/Invoke for DPI
        enum MonitorDpiType
        {
            MDT_EFFECTIVE_DPI = 0,
            MDT_ANGULAR_DPI = 1,
            MDT_RAW_DPI = 2,
            MDT_DEFAULT = MDT_EFFECTIVE_DPI
        }

        [DllImport("Shcore.dll")]
        static extern int GetDpiForMonitor(IntPtr hmonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);

        [DllImport("User32.dll")]
        static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        const uint MONITOR_DEFAULTTONEAREST = 2;

    }
}
