using System.Threading.Tasks;

namespace MyNote.Domain.Notes
{
    public interface INoteStorage
    {
        Task<IReadOnlyList<NoteInfo>> ListAsync(string vaultPath, CancellationToken cancellationToken = default);
        Task<Note?> ReadAsync(string path, CancellationToken cancellationToken = default);
        Task CreateAsync();
        Task RenameAsync(string oldName, string newName);
        Task DeleteAsync();
        Task<bool> SaveAsync(Note note, CancellationToken token = default);
    }
}