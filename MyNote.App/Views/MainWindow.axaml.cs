using Avalonia.Controls;

namespace MyNote.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    public bool AllowCloseWindow { get; set; }

}