using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MyNote.App.ViewModels;

namespace MyNote.App.Views;

public partial class FolderSelectionView : UserControl
{
    public FolderSelectionView()
    {
        InitializeComponent();
    }

    private async void SelectFolder_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null) return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions()
        {
            Title = "Select folder with note",
            AllowMultiple = false
        });

        if (folders.Count == 0) return;

        var selectedFolder = folders[0];

        if (DataContext is MainViewModel viewModel)
            viewModel.SetSelectedFolder(selectedFolder);
    }
}