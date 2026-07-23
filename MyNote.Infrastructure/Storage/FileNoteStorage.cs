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

        Task<IReadOnlyList<Note>> INoteStorage.GetAllAsync(string vaultPath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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