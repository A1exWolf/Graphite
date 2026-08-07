using MyNote.Domain.Vaults;

namespace MyNote.Infrastructure.Vault;

public class FileVaultTreeReader : IVaultTreeReader
{
    public void ReadDirectory(string path, List<NoteNode> model)
    {
        var dirList = Directory.GetFileSystemEntries(path);

        foreach (var i in dirList)
        {
            var attr = File.GetAttributes(i);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                var newItem = new NoteNode
                {
                    Name = Path.GetFileName(i),
                    Path = i,
                    TypeNode = TypeNode.Folder
                };


                model.Add(newItem);

                ReadDirectory(i, newItem.Children);
            }
            else
            {
                if (!string.Equals(Path.GetExtension(i), ".md", StringComparison.OrdinalIgnoreCase))
                    continue;

                var newItem = new NoteNode
                {
                    Name = Path.GetFileNameWithoutExtension(i),
                    Path = i,
                    TypeNode = TypeNode.Note
                };

                model.Add(newItem);
            }
        }

        model.Sort((i1, i2) =>
        {
            var t = i2.TypeNode.CompareTo(i1.TypeNode);

            if (t == 0)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(i1.Name, i2.Name);
            }

            return t;
        });
    }
}