using MyNote.Domain.Notes;

namespace MyNote.Infrastructure.Storage
{
    public class LocalStorage : INoteStorage
    {
        public LocalStorage()
        {

        }

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

        Task<Note?> INoteStorage.ReadAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task INoteStorage.RenameAsync(string oldName, string newName)
        {
            throw new NotImplementedException();
        }
    }
}