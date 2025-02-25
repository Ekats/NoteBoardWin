using System.Collections.ObjectModel;

namespace NoteboardWPFUI.Models
{
    public static class ClipManager
    {
        public static ObservableCollection<ClipItem> Clips { get; } = new ObservableCollection<ClipItem>();
    }
}
