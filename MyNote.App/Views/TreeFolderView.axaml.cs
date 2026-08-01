using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MyNote.App.ViewModels;
using MyNote.Domain.Vaults;

namespace MyNote.App.Views;

public partial class TreeFolderView : UserControl
{
    public TreeFolderView()
    {
        InitializeComponent();
    }

    private async void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not TreeView positionSender)
            return;

        if (positionSender.SelectedItem is NoteNode node && node.TypeNode == TypeNode.Note)
        {
            if (DataContext is MainViewModel model)
            {
                try
                {
                    model.ErrorMessage = string.Empty;

                    await model.EditorTabsViewModel.OpenNote(node.Path);
                }
                catch (Exception exception)
                {
                    model.ErrorMessage = exception.Message;
                }
            }
        }
    }
}