using System.Threading.Tasks;

namespace MyNote.Interface
{
    public interface INoteStorage
    {
        Task GetList();
        Task ReadFile(string path);
        Task CreateNote();
        Task RenameNote(string oldName, string newName);
        Task DeleteNote();
    }
}