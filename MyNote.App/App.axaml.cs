using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MyNote.App.Models;
using MyNote.App.ViewModels;
using MyNote.App.Views;
using MyNote.Domain.Notes;
using MyNote.Infrastructure.Config;
using MyNote.Infrastructure.Storage;
using MyNote.Infrastructure.Vault;

namespace MyNote.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var configManager = new ConfigManager();
            var noteStorage = new FileNoteStorage();
            var vaultManager = new FileVaultManager();
            var vaultReader = new FileVaultTreeReader();
            var editorTabsViewModel = new EditorTabsViewModel(noteStorage);

            var mainView = new MainViewModel(
                vaultManager, 
                configManager, 
                vaultReader,
                editorTabsViewModel);

            var mainWindow = new MainWindow
            {
                DataContext = mainView,
            };

            mainView.Owner = mainWindow;

            mainWindow.Opened += async (_, _) =>
            {
                await mainView.InitializeAsync();
            };

            desktop.MainWindow = mainWindow;

            desktop.MainWindow.Closing += async (sender, args) =>
            {
                if (!mainWindow.AllowCloseWindow)
                {
                    var openNoteNotSaving = mainView.EditorTabsViewModel.OpenNotes.Where(x => x.State is StateNote.Saving or StateNote.Modified or StateNote.Error).ToList();

                    if (openNoteNotSaving.Count > 0)
                    {
                        args.Cancel = true;
                    }

                    foreach (var note in openNoteNotSaving)
                    {
                        await note.SaveFile();
                    }

                    if (openNoteNotSaving.Count(x => x.State == StateNote.Error) == 0)
                    {
                        mainWindow.AllowCloseWindow = true;
                        mainWindow.Close();
                    }
                }
            };

        }

        base.OnFrameworkInitializationCompleted();
    }
}