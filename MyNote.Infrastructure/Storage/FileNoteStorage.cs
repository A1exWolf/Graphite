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
        public Task<Note> CreateAsync(string folderPath, string name, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException("Не найдена директория");

            if (!CheckAvialableName(name)) 
                throw new IOException("Имя файла содержит недопустимые символы");

            if (File.Exists(Path.Combine(folderPath, name)))
                throw new DuplicateWaitObjectException("Файл уже существует");

            try
            {
                File.Create(Path.Combine(folderPath, name));

                return new Task<Note>(() => new Note
                {
                    Path = folderPath,
                    Title = name,
                    Content = string.Empty
                });
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception e)
            {
                throw new IOException("Ошибка во время создания файла");
            }
        }

        /// <summary>
        /// function for check name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool CheckAvialableName(string name)
        {
            throw new NotImplementedException();
        }

        Task INoteStorage.DeleteAsync()
        {
            throw new NotImplementedException();
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