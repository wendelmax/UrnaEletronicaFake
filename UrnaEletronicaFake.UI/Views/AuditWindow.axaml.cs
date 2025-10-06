using Avalonia.Controls;
using Avalonia.Interactivity;
using UrnaEletronicaFake.UI.ViewModels;

namespace UrnaEletronicaFake.UI.Views;

public partial class AuditWindow : Window
{
    public AuditWindow()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
