using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MyNote.App.Models;
using MyNote.App.ViewModels;

namespace MyNote.App.Views;

public partial class TreeFolderView : UserControl
{
    public TreeFolderView()
    {
        InitializeComponent();
    }

    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var t = (TreeView)sender;
        
        if (DataContext is MainViewModel model)
            model.OpenTab((TreeFolderModel)t.SelectedItem);
    }
}