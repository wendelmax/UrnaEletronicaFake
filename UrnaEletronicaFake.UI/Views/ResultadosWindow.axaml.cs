using Avalonia.Controls;
using Avalonia.Interactivity;
using UrnaEletronicaFake.UI.ViewModels;

namespace UrnaEletronicaFake.UI.Views;

public partial class ResultadosWindow : Window
{
    public ResultadosWindow()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
