using System.Text;
using MyNote.Domain.Notes;

namespace MyNote.Infrastructure.Storage
{
    /// <summary>
    /// Класс для работы с файловой директорией
    /// </summary>
    public class FileNoteStorage : INoteStorage
    {
        /// <summary>
        /// Create note
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Note> CreateAsync(string folderPath, string name,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            cancellationToken.ThrowIfCancellationRequested();

            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(
                    $"Папка не найдена: {folderPath}");

            var title = name.Trim();

            if (!NoteRules.IsValidName(title) ||
                title.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException(
                    "Имя заметки содержит недопустимые символы.",
                    nameof(name));
            }

            var fileName = $"{title}.md";
            var filePath = Path.Combine(folderPath, fileName);

            try
            {
                await using (var stream = new FileStream(
                                 filePath,
                                 new FileStreamOptions
                                 {
                                     Mode = FileMode.CreateNew,
                                     Access = FileAccess.Write,
                                     Share = FileShare.None,
                                     Options = FileOptions.Asynchronous
                                 }))
                {
                    
                }
            }
            catch (IOException exception) when (File.Exists(filePath))
            {
                throw new IOException(
                    $"Заметка «{title}» уже существует.",
                    exception);
            }

            return new Note
            {
                Path = filePath,
                Title = title,
                Content = string.Empty,
                ModifiedAt = File.GetLastWriteTimeUtc(filePath)
            };
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