using System.Collections.ObjectModel;

namespace MyNote.App.Models;

public class TreeFolderModel
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public ObservableCollection<TreeFolderModel> Children { get; } = [];
}