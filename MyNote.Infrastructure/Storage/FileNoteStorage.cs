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

        public async Task<IReadOnlyList<NoteInfo>> ListAsync(string vaultPath, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(vaultPath);

            List<NoteInfo> noteList = new List<NoteInfo>();

            if (!Directory.Exists(vaultPath))
                return noteList;

            var markdownFiles = Directory.EnumerateFiles(vaultPath, "*.md", SearchOption.AllDirectories);

            foreach (var note in markdownFiles)
            {
                noteList.Add(new NoteInfo(note,
                Path.GetRelativePath(vaultPath, note),
                Path.GetFileNameWithoutExtension(note),
                File.GetLastWriteTimeUtc(note)));
            }

            return noteList;
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