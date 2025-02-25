using System.Windows;
using System.Windows.Controls;
using NoteboardWPFUI.Models;

namespace NoteboardWPFUI.Views.Pages
{
    public partial class ClipsPage : Page
    {
        public ClipsPage()
        {
            InitializeComponent();
        }

        // When a clip button is clicked, copy its text back to the clipboard.
        private void ClipButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ClipItem clip)
            {
                Clipboard.SetText(clip.Text);
            }
        }
    }
}
