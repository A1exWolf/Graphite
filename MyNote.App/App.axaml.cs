using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MyNote.App.ViewModels;
using MyNote.App.Views;
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
            
            var mainView = new MainViewModel(noteStorage, vaultManager, configManager, vaultReader);

            var mainWindow = new MainWindow
            {
                DataContext = mainView,
            };

            mainWindow.Opened += async (_, _) =>
            {
                await mainView.InitializeAsync();
            };

            desktop.MainWindow = mainWindow;

        }

        base.OnFrameworkInitializationCompleted();
    }
}