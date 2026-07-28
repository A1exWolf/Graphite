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
using MyNote.Domain.Config;
using MyNote.Domain.Notes;
using MyNote.Domain.Vaults;

namespace MyNote.App.ViewModels;

//TODO: 
//Лучше:
// 1. Переименовать метод в InitializeAsync.
// 2. Не вызывать его из конструктора.
// 3. После создания окна вызвать его через событие Opened.
// 4. Внутри использовать уже существующий LoadVaultAsync, чтобы не дублировать загрузку заметок.
// 5. Устанавливать IsFolderSelected только после успешного открытия.
public partial class MainViewModel : ViewModelBase
{
    private readonly INoteStorage _noteStorage;
    private readonly IVaultManager _vaultManager;
    private readonly IConfigStorage _configStorage;
    
    public MainViewModel(INoteStorage noteStorage, IVaultManager vaultManager, IConfigStorage configStorage)
    {
        _noteStorage = noteStorage ?? throw new ArgumentNullException(nameof(noteStorage));
        _vaultManager = vaultManager ?? throw new ArgumentNullException(nameof(vaultManager));
        _configStorage = configStorage ?? throw new ArgumentNullException(nameof(configStorage));
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
    private Config? _config { get; set; }

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
