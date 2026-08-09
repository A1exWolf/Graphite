using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNote.Domain.Notes;

namespace MyNote.App.ViewModels;

public partial class RenameNoteViewModel : ViewModelBase
{
    public RenameNoteViewModel(string name, string path, INoteStorage noteStorage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(noteStorage);

        Name = name;
        OldName = Name;
        FolderPath = Path.GetDirectoryName(path)!;
        NoteStorage = noteStorage;
        PathToFile = path;
    }

    [ObservableProperty] public partial string Name { get; set; }
    [ObservableProperty] public partial string ErrorMessage { get; set; }
    [ObservableProperty] public partial bool IsSaveInProcces { get; set; }
    private string FolderPath { get; init; }
    private string PathToFile { get; set; }
    private string OldName { get; init; }
    private INoteStorage NoteStorage { get; init; }

    public delegate void CloseRequestDelegate(string? oldPath, string? newPath);

    public event CloseRequestDelegate CloseRequested;

    [RelayCommand]
    public async Task Rename()
    {
        try
        {
            var newPath = await NoteStorage.Rename(OldName, Name, FolderPath);
            CloseRequested?.Invoke(PathToFile, newPath);
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
        }
    }

    [RelayCommand]
    public async Task Cancel()
    {
        CloseRequested?.Invoke(null, null);
    }
}