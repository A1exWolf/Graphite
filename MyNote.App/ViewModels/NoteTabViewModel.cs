using CommunityToolkit.Mvvm.ComponentModel;
using MyNote.Domain.Notes;
using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MyNote.App.Models;
using MyNote.Infrastructure.Storage;

namespace MyNote.App.ViewModels
{
    public partial class NoteTabViewModel : ViewModelBase
    {
        public NoteTabViewModel(Note note)
        {
            ArgumentNullException.ThrowIfNull(note);

            Path = note.Path;
            Title = note.Title;
            Content = note.Content;
            ModifiedAt = note.ModifiedAt;

            _storage = new FileNoteStorage();
        }

        [ObservableProperty]
        public partial string Title { get; set; }
        [ObservableProperty]
        public partial string Content { get; set; }
        public string Path { get; }
        public DateTime ModifiedAt { get; }

        [ObservableProperty]
        public partial string StatusSave { get; set; }

        private CancellationTokenSource? _cancellationTokenSource;
        private readonly INoteStorage _storage;
        private bool IsFirstInitizize { get; set; }
        public StateNote State;

        async partial void OnContentChanged(string value)
        {
            await SaveFileAsync();
        }

        private async Task SaveFileAsync()
        {
            if (!IsFirstInitizize)
            {
                State = StateNote.Idle;
                IsFirstInitizize = true;
                return;
            }

            StatusSave = "Изменено";
            State = StateNote.Modified;

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource?.Dispose();
            }

            var token = new CancellationTokenSource();
            _cancellationTokenSource = token;

            try
            {
                token.Token.ThrowIfCancellationRequested();

                await Task.Delay(800, token.Token);

                token.Token.ThrowIfCancellationRequested();

                StatusSave = "Сохранение";
                State = StateNote.Saving;

                var note = new Note()
                {
                    Path = Path,
                    Content = Content,
                    Title = Title,
                    ModifiedAt = DateTime.Now
                };

                await _storage.SaveAsync(note, token.Token);

                StatusSave = "Сохранено";
                State = StateNote.Saved;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                StatusSave = $"Ошибка [{e.Message}]";
                State = StateNote.Error;
            }
        }
        
        public async Task SaveFile()
        {
            try
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = null;
                }

                State = StateNote.Saving;
                
                var note = new Note()
                {
                    Path = Path,
                    Content = Content,
                    Title = Title,
                    ModifiedAt = DateTime.Now
                };

                await _storage.SaveAsync(note);

                StatusSave = "Сохранено";
                State = StateNote.Saved;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                StatusSave = $"Ошибка [{e.Message}]";
                State = StateNote.Error;
            }
        }
    }
}
