using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNote.App.Models;
using MyNote.Domain.Notes;
using MyNote.Domain.Vaults;

namespace MyNote.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly INoteStorage _noteStorage;
    private readonly IVaultManager _vaultManager;
    
    public MainViewModel(INoteStorage noteStorage, IVaultManager vaultManager)
    {
        _noteStorage = noteStorage ?? throw new ArgumentNullException(nameof(noteStorage));
        _vaultManager = vaultManager ?? throw new ArgumentNullException(nameof(vaultManager));
    }

    [ObservableProperty] 
    private ObservableCollection<TreeFolderModel> tree = [];

    [ObservableProperty] public ObservableCollection<Note> _openNotes = [];
    [ObservableProperty] public ObservableCollection<NoteInfo> _notes = [];

    [ObservableProperty]
    public partial int SelectedIndexTab { get; set; }

    [ObservableProperty]
    public partial bool IsFolderSelected { get; set; }

    [ObservableProperty]
    public partial string SelectedFolderPath { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    VaultInfo? CurrentVault { get; set; }

    [RelayCommand]
    public void OpenTab(TreeFolderModel folder)
    {
        var searchTab = OpenNotes.FirstOrDefault(x => x.Path == folder.Path);

        if (searchTab != null)
        {
            SelectedIndexTab = searchTab.OpenIndex ?? 0;
            return;
        }
        
        OpenNotes.Add(new Note()
        {
            Id = Random.Shared.Next(1, 9999),
            Title = folder.Name,
            Content = File.ReadAllText(folder.Path),
            Path = folder.Path,
            OpenIndex = OpenNotes.Count
        });
    }

    public async Task LoadVaultAsync(string path, CancellationToken token)
    {
        try
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            var vault = await _vaultManager.OpenAsync(path, token);

            var notes = await _noteStorage.ListAsync(vault.Path, token);

            Notes.Clear();

            foreach (var note in notes)
            {
                Notes.Add(note);
            }

            CurrentVault = vault;
            SelectedFolderPath = CurrentVault.Path;
            IsFolderSelected = true;
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

    [Obsolete]
    private void ReadDirectory(string path)
    {
        var pathFolders = Directory.GetDirectories(path);

        foreach (var pathFolder in pathFolders)
        {
            var folderName = Path.GetFileName(pathFolder);

            var d = new TreeFolderModel()
            {
                Name = folderName,
                Path = pathFolder
            };

            foreach (var files in Directory.GetFiles(pathFolder))
            {
                d.Children.Add(new TreeFolderModel()
                {
                    Name = Path.GetFileName(files),
                    Path = files
                });
            }
            
            tree.Add(d);
        }
    }

    [Obsolete]
    public void CloseTab(int bTag)
    {
        var tab = OpenNotes.FirstOrDefault(x => bTag == x.Id);

        for (int i = tab.OpenIndex ?? 0; i < OpenNotes.Count; i++)
        {
            OpenNotes[i].OpenIndex--;
        }
        
        if (tab != null)
        {
            OpenNotes.Remove(tab);
        }
    }
}
