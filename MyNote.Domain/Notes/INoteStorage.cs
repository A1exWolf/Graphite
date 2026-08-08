namespace MyNote.Domain.Notes
{
    public interface INoteStorage
    {
        Task<IReadOnlyList<NoteInfo>> ListAsync(string vaultPath, CancellationToken cancellationToken = default);
        Task<Note?> ReadAsync(string path, CancellationToken cancellationToken = default);
        Task<Note> CreateAsync(string folderPath, string name, CancellationToken cancellationToken = default);
        Task<string> Rename(string oldName, string newName, string path, CancellationToken cancellationToken = default);
        Task DeleteAsync(string pathVault, string path, CancellationToken cancellationToken = default);
        Task SaveAsync(Note note, CancellationToken token = default);
    }
}