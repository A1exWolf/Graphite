using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MyNote.App.ViewModels;

namespace MyNote.App.Views;

public partial class TabControlView : UserControl
{
    public TabControlView()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var b = (Button)sender;
        
        if (DataContext is MainViewModel model)
            model.CloseTab((int)b.Tag);
    }
}