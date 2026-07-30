using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MyNote.App.ViewModels;

namespace MyNote.App.Views;

public partial class TopMenuView : UserControl
{
    public TopMenuView()
    {
        InitializeComponent();
    }

    private async void MenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel model)
        {
            await model.Refresh(model.SelectedFolderPath, default);
        }
    }
}