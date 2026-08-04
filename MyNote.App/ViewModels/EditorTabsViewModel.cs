using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNote.Domain.Notes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyNote.App.ViewModels
{
    public partial class EditorTabsViewModel : ViewModelBase
    {
        private readonly INoteStorage _noteStorage;

        public EditorTabsViewModel(INoteStorage noteStorage)
        {
            _noteStorage = noteStorage ?? throw new ArgumentNullException(nameof(noteStorage));
        }

        [ObservableProperty] public partial NoteTabViewModel? SelectedNote { get; set; }

        [ObservableProperty] public ObservableCollection<NoteTabViewModel> _openNotes = [];

        private bool _processOpenNote = false;

        //todo: This implementation allows you to open only one tab at a time, losing other user instructions.
        public async Task OpenNote(string path, CancellationToken token = default)
        {
            if (_processOpenNote) return;

            try
            {
                _processOpenNote = true;

                var searchTab = OpenNotes.FirstOrDefault(x => x.Path == path);

                if (searchTab != null)
                {
                    SelectedNote = searchTab;
                    return;
                }

                var note = await _noteStorage.ReadAsync(path, token);

                if (note != null)
                {
                    var noteTab = new NoteTabViewModel(note);
                    OpenNotes.Add(noteTab);
                    SelectedNote = noteTab;
                }
            }
            catch (OperationCanceledException e)
            {

            }
            finally
            {
                _processOpenNote = false;
            }
        }

        [RelayCommand]
        public void CloseTab(NoteTabViewModel tab)
        {
            OpenNotes.Remove(tab);

            if (SelectedNote == tab)
            {
                SelectedNote = OpenNotes.FirstOrDefault();
            }
        }
    }
}