using System.Collections.ObjectModel;

namespace MyNote.App.Models;

public class TreeFolderModel
{
    public string Name { get; set; }
    public ObservableCollection<TreeFolderModel> Children { get; } = [];
}