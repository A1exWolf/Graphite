using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyNote.Domain.Notes;

namespace MyNote.App.ViewModels;

public partial class DeleteNoteViewModel : ViewModelBase
{
    public DeleteNoteViewModel(INoteStorage noteStorage, string pathVault, string pathNote, EditorTabsViewModel editorTabsViewModel)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathVault);
        ArgumentException.ThrowIfNullOrEmpty(pathNote);
        ArgumentNullException.ThrowIfNull(noteStorage);
        ArgumentNullException.ThrowIfNull(editorTabsViewModel);

        PathVault = pathVault;
        PathNote = pathNote;
        NoteStorage = noteStorage;
        IsDeleteInProgress = false;
        EditorTabsViewModel = editorTabsViewModel;

        Text = $"Are you sure you want to delete file {System.IO.Path.GetFileNameWithoutExtension(PathNote)}?";
    }

    private string PathVault { get; }
    private string PathNote { get; }
    private INoteStorage NoteStorage { get; }
    private EditorTabsViewModel EditorTabsViewModel { get; }

    /// <summary>
    /// Indicator on deleting
    /// </summary>
    [ObservableProperty]
    public partial bool IsDeleteInProgress { get; set; }

    [ObservableProperty] public partial string ErrorMessage { get; set; }
    [ObservableProperty] public partial string Text { get; set; }

    public delegate void OnCloseRequested(bool statusDeleted);

    public event OnCloseRequested CloseRequested;

    [RelayCommand]
    public async Task Delete()
    {
        if (IsDeleteInProgress)
            return;

        IsDeleteInProgress = true;

        try
        {
            var searchDeletingTab = EditorTabsViewModel.OpenNotes.FirstOrDefault(x => x.Path == PathNote);

            if (searchDeletingTab != null)
            {
                await EditorTabsViewModel.CloseTab(searchDeletingTab);

                if (EditorTabsViewModel.OpenNotes.Contains(searchDeletingTab))
                {
                    ErrorMessage = "The note could not be saved before deletion";
                    return;
                }
            }

            await NoteStorage.DeleteAsync(PathVault, PathNote, default);

            CloseRequested?.Invoke(true);
        }
        catch (Exception e)
        {
            ErrorMessage = "An error occurred during deletion";
        }
        finally
        {
            IsDeleteInProgress = false;
        }
    }

    [RelayCommand]
    public void Cancel()
    {
        if (!IsDeleteInProgress)
            CloseRequested?.Invoke(false);
        else
            ErrorMessage = "Deleting in procces";
    }
}
