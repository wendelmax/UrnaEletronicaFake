using Avalonia.Controls;
using Avalonia.Interactivity;

namespace UrnaEletronicaFake.UI.Views;

public partial class ConfirmationDialog : Window
{
    public ConfirmationDialog()
    {
        InitializeComponent();
    }
    
    private void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }
    
    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}


