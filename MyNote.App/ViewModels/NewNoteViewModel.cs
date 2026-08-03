using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNote.Domain.Notes;

namespace MyNote.App.ViewModels
{
    public partial class NewNoteViewModel : ViewModelBase
    {
        public NewNoteViewModel(INoteStorage noteStorage, List<string> folders, string path = "")
        {
            SelectedFolderPath = path;
            Folders = folders;
            NoteStorage = noteStorage;
        }

        private INoteStorage NoteStorage { get; set; }

        [ObservableProperty]
        public partial string Name { get; set; }
        [ObservableProperty]
        public partial string SelectedFolderPath { get; set; }
        public List<string> Folders { get; set; }
        [ObservableProperty]
        public partial string ErrorMessage { get; set; }

        public delegate void CloseRequestDelegate(Note? note);
        public event CloseRequestDelegate CloseRequested;

        [RelayCommand]
        public void Cancel()
        {
            CloseRequested?.Invoke(null);
        }

        [RelayCommand]
        public async Task Create()
        {
            try
            {
                var newNote = await NoteStorage.CreateAsync(SelectedFolderPath, Name, default);

                CloseRequested?.Invoke(newNote);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

    }
}
