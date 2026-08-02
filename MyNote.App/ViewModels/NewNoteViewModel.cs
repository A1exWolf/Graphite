using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MyNote.App.ViewModels
{
    public partial class NewNoteViewModel : ViewModelBase
    {
        public NewNoteViewModel(List<string> folders, string path = "")
        {
            SelectedFolderPath = path;
            Folders = folders;
        }

        [ObservableProperty]
        public partial string Name { get; set; }
        [ObservableProperty]
        public partial string SelectedFolderPath { get; set; }
        private List<string> Folders { get; set; }

    }
}
