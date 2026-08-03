using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNote.App.Views;
using MyNote.Domain.Config;
using MyNote.Domain.Notes;
using MyNote.Domain.Vaults;
using MyNote.Infrastructure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MyNote.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IVaultManager _vaultManager;
    private readonly IConfigStorage _configStorage;
    private readonly IVaultTreeReader _vaultTreeReader;
    
    public MainViewModel( 
        IVaultManager vaultManager, 
        IConfigStorage configStorage, 
        IVaultTreeReader vaultTreeReader,
        EditorTabsViewModel editorTabsViewModel)
    {
        _vaultManager = vaultManager ?? throw new ArgumentNullException(nameof(vaultManager));
        _configStorage = configStorage ?? throw new ArgumentNullException(nameof(configStorage));
        _vaultTreeReader = vaultTreeReader ?? throw new ArgumentNullException(nameof(vaultTreeReader));
        EditorTabsViewModel = editorTabsViewModel ?? throw new ArgumentNullException(nameof(editorTabsViewModel));
    }

    public EditorTabsViewModel EditorTabsViewModel { get; }
    public Window Owner { get; set; }

    [ObservableProperty] public List<NoteNode> tree = [];

    [ObservableProperty] public partial bool IsFolderSelected { get; set; }
    [ObservableProperty] public partial string SelectedFolderPath { get; set; } = string.Empty;

    [ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty] public partial bool IsLoading { get; set; }

    VaultInfo? CurrentVault { get; set; }
    private Config? _config { get; set; }

    [RelayCommand]
    public async Task OpenNewNote(string path = "")
    {
        var listFolder = new List<string>{ SelectedFolderPath };
        listFolder.AddRange(Directory.GetDirectories(SelectedFolderPath, "*", 
            new EnumerationOptions() { RecurseSubdirectories = true }));
        var newNoteViewModel = new NewNoteViewModel(new FileNoteStorage(), 
            listFolder, 
            string.IsNullOrEmpty(path) ? SelectedFolderPath : path);
        var newNoteView = new NewNoteView
        {
            DataContext = newNoteViewModel
        };
        newNoteViewModel.CloseRequested += newNoteView.Close;
        var note = await newNoteView.ShowDialog<Note?>(Owner);

        if (note != null)
        {
            await Refresh(SelectedFolderPath, default);
            await EditorTabsViewModel.OpenNote(note.Path, default);
        }
    }

    public async Task Refresh(string path, CancellationToken token)
    {
        await LoadVaultAsync(path, token);
    }

    public async Task LoadVaultAsync(string path, CancellationToken token)
    {
        try
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            var vault = await _vaultManager.OpenAsync(path, token);

            var newTree = new List<NoteNode>();

            await Task.Run(() =>
            {
                _vaultTreeReader.ReadDirectory(path, newTree);
            }, token);

            Tree = newTree;

            CurrentVault = vault;
            SelectedFolderPath = CurrentVault.Path;
            IsFolderSelected = true;

            await _configStorage.ReplaceFieldAsync(ConfigField.VaultPath, CurrentVault.Path, token);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task InitializeAsync(CancellationToken token = default)
    {
        try
        {
            ErrorMessage = string.Empty;

            var config = await _configStorage.GetConfigAsync(token);

            if (string.IsNullOrEmpty(config.LastOpenVault))
                return;

            await LoadVaultAsync(config.LastOpenVault, token);

            _config = config;
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception e)
        {
            IsFolderSelected = false;
            ErrorMessage = e.Message;
        }
    }
}
