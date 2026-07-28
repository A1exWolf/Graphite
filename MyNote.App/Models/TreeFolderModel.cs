using System;
using System.Collections.ObjectModel;

namespace MyNote.App.Models;

[Obsolete("Скорее всего данная модель не подойдет дальше")]
public class TreeFolderModel
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public ObservableCollection<TreeFolderModel> Children { get; } = [];
}