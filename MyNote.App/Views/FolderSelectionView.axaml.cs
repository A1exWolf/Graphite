using System.Collections.Generic;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using MyNote.App.ViewModels;

namespace MyNote.App.Views;

public partial class FolderSelectionView : UserControl
{
    public FolderSelectionView()
    {
        InitializeComponent();

        cancellationToken = cancellationTokenSource.Token;
    }

    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    CancellationToken cancellationToken;

    private async void SelectFolder_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null) return;

        IReadOnlyList<IStorageFolder> folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions()
        {
            Title = "Select folder with note",
            AllowMultiple = false
        });

        if (folders.Count == 0) return;

        var selectedFolder = folders[0].TryGetLocalPath() ?? folders[0].Path.ToString(); ;

        if (DataContext is MainViewModel viewModel)
            await viewModel.LoadVaultAsync(selectedFolder, cancellationToken);
    }
}