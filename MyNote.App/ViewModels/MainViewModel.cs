using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNote.App.Models;
using MyNote.Domain.Config;
using MyNote.Domain.Notes;
using MyNote.Domain.Vaults;

namespace MyNote.App.ViewModels;

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
    [ObservableProperty] private ObservableCollection<TreeFolderModel> tree = [];
    [ObservableProperty] public ObservableCollection<Note> _openNotes = [];
    [ObservableProperty] public ObservableCollection<NoteInfo> _notes = [];

    [ObservableProperty] public partial Note? SelectedNote { get; set; }

    [ObservableProperty] public partial bool IsFolderSelected { get; set; }
    [ObservableProperty] public partial string SelectedFolderPath { get; set; } = string.Empty;

    [ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty] public partial bool IsLoading { get; set; }

    VaultInfo? CurrentVault { get; set; }
    private Config? _config { get; set; }

    [RelayCommand]
    public void OpenTab(TreeFolderModel folder)
    {
        var searchTab = OpenNotes.FirstOrDefault(x => x.Path == folder.Path);

        if (searchTab != null)
        {
            // TODO: Доделывается в след день
        }
        
        OpenNotes.Add(new Note()
        {
            Id = Random.Shared.Next(1, 9999),
            Title = folder.Name,
            Content = File.ReadAllText(folder.Path),
            Path = folder.Path,
            //OpenIndex = OpenNotes.Count Убрал из модели это состояние а не описание
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

    //todo: скорее всего с индексами не нужно работать можно проще
    [Obsolete]
    public void CloseTab(int bTag)
    {
        
    }
}
