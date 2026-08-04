using System.Text;
using MyNote.Domain.Notes;

namespace MyNote.Infrastructure.Storage
{
    /// <summary>
    /// Класс для работы с файловой директорией
    /// </summary>
    public class FileNoteStorage : INoteStorage
    {
        Task INoteStorage.CreateAsync()
        {
            throw new NotImplementedException();
        }

        Task INoteStorage.DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAsync(Note note, CancellationToken token = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(note.Path);

            try
            {
                var pathToFile = Path.GetDirectoryName(note.Path);

                ArgumentException.ThrowIfNullOrEmpty(pathToFile);

                var tempPath = Path.Combine(pathToFile, $"{note.Title}_{Guid.NewGuid():N}");

                token.ThrowIfCancellationRequested();

                await File.WriteAllTextAsync(tempPath, note.Content, Encoding.UTF8, token);

                File.Move(pathToFile, note.Path, true);

                return true;
            }
            catch (OperationCanceledException)
            {

            }
            catch (PathTooLongException)
            {
                throw new PathTooLongException("Длинный путь к файлу");
            }
            catch (IOException)
            {
                throw new IOException("Ошибка записи файла");
            }

            return false;
        }

        public Task<IReadOnlyList<NoteInfo>> ListAsync(
            string vaultPath,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(vaultPath);

            return Task.Run<IReadOnlyList<NoteInfo>>(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!Directory.Exists(vaultPath))
                    {
                        throw new DirectoryNotFoundException(
                            $"Данной директории нет: {vaultPath}");
                    }

                    var noteList = new List<NoteInfo>();

                    var markdownFiles = Directory.EnumerateFiles(
                        vaultPath,
                        "*.md",
                        SearchOption.AllDirectories);

                    foreach (var notePath in markdownFiles)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        noteList.Add(
                            new NoteInfo(
                                Path: notePath,
                                RelativePath: Path.GetRelativePath(
                                    vaultPath,
                                    notePath),
                                Title: Path.GetFileNameWithoutExtension(
                                    notePath),
                                ModifiedAt: File.GetLastWriteTimeUtc(
                                    notePath)));
                    }

                    return noteList;
                },
                cancellationToken);
        }

        /// <summary>
        /// Чтение одного файла
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>заполненный класс Note</returns>
        public async Task<Note?> ReadAsync(string path, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!File.Exists(path))
                return null;

            var content = await File.ReadAllTextAsync(path, Encoding.UTF8, cancellationToken);

            return new Note()
            {
                Path = path,
                Title = Path.GetFileNameWithoutExtension(path),
                Content = content,
                ModifiedAt = File.GetLastWriteTimeUtc(path)
            };
        }

        Task INoteStorage.RenameAsync(string oldName, string newName)
        {
            throw new NotImplementedException();
        }
    }
}