namespace MyNote.Domain.Vaults;

public interface IVaultTreeReader
{
    void ReadDirectory(string path, List<NoteNode> model);
}