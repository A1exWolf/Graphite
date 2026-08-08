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

        public Task DeleteAsync(string pathVault, string path, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(path);

            if (!File.Exists(path))
                throw new FileNotFoundException("File not exists");

            try
            {
                var pathToFolderTrash = Path.Combine(pathVault, ".trash");
                
                var directoryInfo = Directory.CreateDirectory(pathToFolderTrash);
                
                File.Move(path, Path.Combine(pathToFolderTrash, $"{Guid.NewGuid():D}.md"));

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task SaveAsync(Note note, CancellationToken token = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(note.Path);

            string tempPath = string.Empty;

            try
            {
                var pathToFile = Path.GetDirectoryName(note.Path);

                ArgumentException.ThrowIfNullOrEmpty(pathToFile);

                tempPath = Path.Combine(pathToFile, $"{Guid.NewGuid():N}");

                token.ThrowIfCancellationRequested();

                await File.WriteAllTextAsync(tempPath, note.Content, Encoding.UTF8, token);

                token.ThrowIfCancellationRequested();

                File.Move(tempPath, note.Path, true);
            }
            catch (PathTooLongException)
            {
                throw new PathTooLongException("Too long name file");
            }
            catch (IOException)
            {
                throw new IOException("Error while writing file");
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempPath))
                    await DeleteTempFile(tempPath);
            }
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

                        var relativePath = Path.GetRelativePath(
                            vaultPath,
                            notePath);

                        if (string.Equals(relativePath, ".trash", StringComparison.OrdinalIgnoreCase) ||
                            relativePath.StartsWith(
                                $".trash{Path.DirectorySeparatorChar}",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        noteList.Add(
                            new NoteInfo(
                                Path: notePath,
                                RelativePath: relativePath,
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

        public Task<string> Rename(string oldName, string newName, string path, CancellationToken token = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(oldName);
            ArgumentException.ThrowIfNullOrEmpty(newName);
            ArgumentException.ThrowIfNullOrEmpty(path);
            
            if (string.Equals(oldName, newName))
                throw new ArgumentException("The lines must be different");

            var oldPath = Path.Combine(path, $"{oldName}.md");
            var newPath = Path.Combine(path, $"{newName}.md");
            
            if (!File.Exists(oldPath))
                throw new FileNotFoundException("File not exists or not search");

            if (File.Exists(newPath))
                throw new IOException("File already exists");
            
            token.ThrowIfCancellationRequested();
            
            try
            {
                NoteRules.EnsureValidNameNote(newName);
                
                token.ThrowIfCancellationRequested();
                
                File.Move(oldPath, newPath);

                return Task.FromResult(newPath);
            }
            catch (PathTooLongException exception)
            {
                throw new PathTooLongException("File too long contain chars");
            }
            catch (NotSupportedException exception)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new IOException("Soma kind of error occurred");
            }
        }

        private Task DeleteTempFile(string path)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(path);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return Task.CompletedTask;
            }
            catch (IOException)
            {
                throw new IOException("Error while deleting temporary file");
            }
        }
    }
}
