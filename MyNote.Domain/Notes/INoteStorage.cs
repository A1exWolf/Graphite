using System.Threading.Tasks;

namespace MyNote.Domain.Notes
{
    public interface INoteStorage
    {
        Task<IReadOnlyList<Note>> GetAllAsync(string vaultPath, CancellationToken cancellationToken = default);
        Task<Note?> ReadAsync(string path, CancellationToken cancellationToken = default);
        Task CreateAsync();
        Task RenameAsync(string oldName, string newName);
        Task DeleteAsync();
    }
}