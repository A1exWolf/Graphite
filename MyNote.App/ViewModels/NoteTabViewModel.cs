using CommunityToolkit.Mvvm.ComponentModel;
using MyNote.Domain.Notes;
using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MyNote.Infrastructure.Storage;

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

        private CancellationTokenSource _cancellationTokenSource;
        private readonly INoteStorage _storage;


        async partial void OnContentChanged(string value)
        {
            await SaveFile();
        }

        private async Task SaveFile()
        {
            StatusSave = "Изменено";

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                StatusSave = "Сохранение";

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                await Task.Delay(1000);

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                var note = new Note()
                {
                    Path = Path,
                    Content = Content,
                    Title = Title,
                    ModifiedAt = DateTime.Now
                };

                await _storage.SaveAsync(note, _cancellationTokenSource.Token);

                StatusSave = "Сохранено";
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                StatusSave = $"Ошибка [{e.Message}]";
            }
        }
    }
}
