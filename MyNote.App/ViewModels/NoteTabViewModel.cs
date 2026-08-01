using CommunityToolkit.Mvvm.ComponentModel;
using MyNote.Domain.Notes;
using System;

namespace MyNote.App.ViewModels
{
    public partial class NoteTabViewModel : ViewModelBase
    {
        public NoteTabViewModel(Note note)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note));

            Path = note.Path;
            Title = note.Title;
            Content = note.Content;
            ModifiedAt = note.ModifiedAt;
        }

        [ObservableProperty]
        public partial string Title { get; set; }
        [ObservableProperty]
        public partial string Content { get; set; }
        public string Path { get; }
        public DateTime ModifiedAt { get; }
    }
}
