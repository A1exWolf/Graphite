using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNote.App.Models;

namespace MyNote.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    public MainViewModel()
    {
        #region DEBUG
        IsFolderSelected = true;
        SelectedFolderPath = @"C:\Users\Alex\Documents\TestValue";
        #endregion

        if (!string.IsNullOrWhiteSpace(SelectedFolderPath))
        {
            tree = new();
            ReadDirectory(SelectedFolderPath);
        }
    }

    [ObservableProperty]
    private ObservableCollection<TreeFolderModel> tree = new()
    {
        new TreeFolderModel
        {
            Name = "First",
            Children =
            {
                new TreeFolderModel
                {
                    Name = "Second"
                },
                new TreeFolderModel
                {
                    Name = "Three"
                }
            }
        }
    };

    [ObservableProperty]
    public partial bool IsFolderSelected { get; set; }

    [ObservableProperty]
    public partial string SelectedFolderPath { get; set; } = string.Empty;

    [RelayCommand]
    public void SetSelectedFolder(IStorageFolder folder)
    {
        SelectedFolderPath = folder.TryGetLocalPath() ?? folder.Path.ToString();
        IsFolderSelected = true;
    }

    private void ReadDirectory(string path)
    {
        var pathFolders = Directory.GetDirectories(path);

        foreach (var pathFolder in pathFolders)
        {
            var folderName = Path.GetFileName(pathFolder);

            var d = new TreeFolderModel()
            {
                Name = folderName
            };

            foreach (var files in Directory.GetFiles(pathFolder))
            {
                d.Children.Add(new TreeFolderModel()
                {
                    Name = Path.GetFileName(files)
                });
            }
            
            tree.Add(d);
        }
    }
}
