using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MyNote.App.ViewModels;
using MyNote.App.Views;
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
            var noteStorage = new FileNoteStorage();
            var vaultManager = new FileVaultManager();
            
            var mainView = new MainViewModel(noteStorage, vaultManager);
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainView,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}