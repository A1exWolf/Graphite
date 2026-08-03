namespace MyNote.Domain.Notes
{
    public interface INoteStorage
    {
        Task<IReadOnlyList<NoteInfo>> ListAsync(string vaultPath, CancellationToken cancellationToken = default);
        Task<Note?> ReadAsync(string path, CancellationToken cancellationToken = default);
        Task<Note> CreateAsync(string folderPath, string name, CancellationToken cancellationToken = default);
        Task RenameAsync(string oldName, string newName);
        Task DeleteAsync();
    }
}