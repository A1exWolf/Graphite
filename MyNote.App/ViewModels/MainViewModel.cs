using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNote.Domain.Config;
using MyNote.Domain.Notes;
using MyNote.Domain.Vaults;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyNote.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IVaultManager _vaultManager;
    private readonly IConfigStorage _configStorage;
    private readonly IVaultTreeReader _vaultTreeReader;
    private readonly INoteStorage _noteStorage;

    public MainViewModel(
        IVaultManager vaultManager,
        IConfigStorage configStorage,
        IVaultTreeReader vaultTreeReader,
        INoteStorage noteStorage)
    {
        _vaultManager = vaultManager ?? throw new ArgumentNullException(nameof(vaultManager));
        _configStorage = configStorage ?? throw new ArgumentNullException(nameof(configStorage));
        _vaultTreeReader = vaultTreeReader ?? throw new ArgumentNullException(nameof(vaultTreeReader));
        _noteStorage = noteStorage ?? throw new ArgumentNullException(nameof(noteStorage));

        EditorTabsViewModel = new EditorTabsViewModel(_noteStorage);
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

    #region Create Note

    [ObservableProperty] public partial bool IsNewNoteOpen { get; set; }

    [ObservableProperty] public partial NewNoteViewModel? NewNoteViewModel { get; set; }

    [RelayCommand]
    public void OpenNewNote(string path = "")
    {
        if (GetStatus()) return;

        var folders = GetFolders();

        NewNoteViewModel = new NewNoteViewModel(
            _noteStorage,
            folders,
            string.IsNullOrEmpty(path) ? SelectedFolderPath : path);

        NewNoteViewModel.CloseRequested += OnNewNoteClosed;
        IsNewNoteOpen = true;
    }

    private async void OnNewNoteClosed(Note? note)
    {
        IsNewNoteOpen = false;

        if (NewNoteViewModel != null)
            NewNoteViewModel.CloseRequested -= OnNewNoteClosed;

        NewNoteViewModel = null;

        if (note != null)
        {
            await Refresh(SelectedFolderPath, default);
            await EditorTabsViewModel.OpenNote(note.Path, default);
        }
    }

    #endregion

    #region Rename Note

    [ObservableProperty] public partial bool IsRenameNoteOpen { get; set; }

    [ObservableProperty] public partial RenameNoteViewModel? RenameNoteViewModel { get; set; }

    [RelayCommand]
    public async Task RenemeNote(NoteNode? node)
    {
        if (GetStatus()) return;

        if (node == null)
        {
            return;
        }

        RenameNoteViewModel = new RenameNoteViewModel(
            node.Name,
            node.Path,
            _noteStorage);

        RenameNoteViewModel.CloseRequested += RenameNoteViewModelOnCloseRequested;
        IsRenameNoteOpen = true;
    }

    private async void RenameNoteViewModelOnCloseRequested(string? oldPath, string? newPath)
    {
        IsRenameNoteOpen = false;
        if (RenameNoteViewModel != null)
            RenameNoteViewModel.CloseRequested -= RenameNoteViewModelOnCloseRequested;

        RenameNoteViewModel = null;

        if (newPath != null)
        {
            await Refresh(SelectedFolderPath);
            // await EditorTabsViewModel.OpenNote();
        }
    }

    #endregion

    #region Delete Note

    [ObservableProperty] public partial bool IsDeleteNoteOpen { get; set; }
    [ObservableProperty] public partial DeleteNoteViewModel? DeleteNoteViewModel { get; set; }

    [RelayCommand]
    public async Task DeleteNote(NoteNode? node)
    {
        if (GetStatus()) return;
        IsDeleteNoteOpen = true;

        if (node == null)
        {
            var activeTab = EditorTabsViewModel.SelectedNote; 
            
            if (activeTab != null)
            {
                node = new NoteNode()
                {
                    Path = activeTab.Path,
                    TypeNode = TypeNode.Note,
                    Name = activeTab.Title
                };
            }
            else
            {
                IsDeleteNoteOpen = false;
                return;
            }
        }

        DeleteNoteViewModel =
            new DeleteNoteViewModel(_noteStorage, SelectedFolderPath, node.Path, EditorTabsViewModel);
        DeleteNoteViewModel.CloseRequested += DeleteNoteOnCloseRequested;
    }

    private async void DeleteNoteOnCloseRequested(bool statusDeleted)
    {
        IsDeleteNoteOpen = false;

        if (DeleteNoteViewModel != null)
            DeleteNoteViewModel.CloseRequested -= DeleteNoteOnCloseRequested;

        DeleteNoteViewModel = null;
        
        if (statusDeleted)
        {
            await Refresh(SelectedFolderPath, default);
        }
    }

    #endregion

    public async Task Refresh(string path, CancellationToken token = default)
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

            await Task.Run(() => { _vaultTreeReader.ReadDirectory(path, newTree); }, token);

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

    private bool GetStatus()
    {
        if (IsRenameNoteOpen || IsNewNoteOpen || IsDeleteNoteOpen)
            return true;
        return false;
    }
    
    /// <summary>
    /// Get all allow folders
    /// </summary>
    /// <returns></returns>
    private List<string> GetFolders()
    {
        var folders = new List<string> { SelectedFolderPath };

        var trashRelativePath = $".trash{Path.DirectorySeparatorChar}";

        folders.AddRange(Directory.GetDirectories(
                SelectedFolderPath,
                "*",
                new EnumerationOptions { RecurseSubdirectories = true })
            .Where(path =>
            {
                var relativePath = Path.GetRelativePath(SelectedFolderPath, path);

                return !string.Equals(relativePath, ".trash", StringComparison.OrdinalIgnoreCase) &&
                       !relativePath.StartsWith(trashRelativePath, StringComparison.OrdinalIgnoreCase);
            }));

        return folders;
    }
}
