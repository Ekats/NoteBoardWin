using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NoteboardWPFUI
{
    public class ClipboardMonitor : IDisposable
    {
        public event EventHandler<string> ClipboardUpdated;
        private HwndSource _hwndSource;
        private IntPtr _windowHandle;

        public ClipboardMonitor(Window window)
        {
            _windowHandle = new WindowInteropHelper(window).Handle;
            _hwndSource = HwndSource.FromHwnd(_windowHandle)!;
            _hwndSource.AddHook(WndProc);
            NativeMethods.AddClipboardFormatListener(_windowHandle);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();
                    ClipboardUpdated?.Invoke(this, text);
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            NativeMethods.RemoveClipboardFormatListener(_windowHandle);
            _hwndSource.RemoveHook(WndProc);
            GC.SuppressFinalize(this);
        }
    }

    internal static class NativeMethods
    {
        public const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
    }
}
