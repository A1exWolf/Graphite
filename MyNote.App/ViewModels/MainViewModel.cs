using System.Collections.ObjectModel;
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
}
