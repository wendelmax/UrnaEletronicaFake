using Avalonia.Controls;
using Avalonia.Interactivity;

namespace UrnaEletronicaFake.UI.Views;

public partial class AdminWindow : Window
{
    public AdminWindow()
    {
        InitializeComponent();
    }

    private void FecharJanela_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
