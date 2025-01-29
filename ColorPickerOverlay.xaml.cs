// ColorPickerOverlay.xaml.cs
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms; // For Screen
using System.Windows.Media.Imaging; // For BitmapSource
using System.Drawing; // For Bitmap

namespace NoteBoard
{
    public partial class ColorPickerOverlay : Window
    {
        // Struct for GetCursorPos
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }

        // Win32 API functions
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        // Static list to keep track of all overlays
        private static List<ColorPickerOverlay> _overlays = new List<ColorPickerOverlay>();

        // Constructor accepting a Screen object
        public ColorPickerOverlay(Screen screen)
        {
            InitializeComponent();
            _overlays.Add(this);

            // Position the overlay window to cover the target screen
            this.Left = screen.Bounds.Left;
            this.Top = screen.Bounds.Top;
            this.Width = screen.Bounds.Width;
            this.Height = screen.Bounds.Height;

            // Ensure the overlay captures all mouse events
            this.Cursor = System.Windows.Input.Cursors.Cross;
        }

        // When the window is loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Optionally, you can add a visual indicator or crosshair here
        }

        // Handle mouse movement to update the popup
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!GetCursorPos(out POINT pt)) return;

            // Convert screen coordinates to this window's coordinates
            System.Windows.Point relativePos = this.PointFromScreen(new System.Windows.Point(pt.X, pt.Y));

            // Position the popup near the cursor with an offset
            PreviewPopup.HorizontalOffset = relativePos.X + 12;
            PreviewPopup.VerticalOffset = relativePos.Y + 12;

            // Get the color at the cursor position
            IntPtr screenDC = GetDC(IntPtr.Zero);
            if (screenDC == IntPtr.Zero) return;

            try
            {
                uint pixel = GetPixel(screenDC, pt.X, pt.Y); // 0x00BBGGRR
                byte r = (byte)(pixel & 0xFF);
                byte g = (byte)((pixel >> 8) & 0xFF);
                byte b = (byte)((pixel >> 16) & 0xFF);

                string hexColor = $"#{r:X2}{g:X2}{b:X2}";

                // Update the preview rectangle and text
                PreviewRect.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                PreviewText.Text = hexColor;
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, screenDC);
            }

            // Ensure the popup is open
            if (!PreviewPopup.IsOpen)
                PreviewPopup.IsOpen = true;
        }

        // Handle mouse click to finalize color selection
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (!GetCursorPos(out POINT pt))
            {
                CloseAllOverlays();
                return;
            }

            IntPtr screenDC = GetDC(IntPtr.Zero);
            if (screenDC == IntPtr.Zero)
            {
                CloseAllOverlays();
                return;
            }

            try
            {
                uint pixel = GetPixel(screenDC, pt.X, pt.Y);
                byte r = (byte)(pixel & 0xFF);
                byte g = (byte)((pixel >> 8) & 0xFF);
                byte b = (byte)((pixel >> 16) & 0xFF);

                string hexColor = $"#{r:X2}{g:X2}{b:X2}";

                // Copy the hex color to the clipboard
                System.Windows.Clipboard.SetText(hexColor);
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, screenDC);
            }

            // Close all overlay windows
            CloseAllOverlays();
        }

        // Method to close all overlay windows
        private void CloseAllOverlays()
        {
            foreach (var overlay in _overlays)
            {
                overlay.Close();
            }
            _overlays.Clear();
        }
    }
}
