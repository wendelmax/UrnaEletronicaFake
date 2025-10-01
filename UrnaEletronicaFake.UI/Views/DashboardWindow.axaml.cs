using Avalonia.Controls;
using Avalonia.Interactivity;

namespace UrnaEletronicaFake.UI.Views;

public partial class DashboardWindow : Window
{
    public DashboardWindow()
    {
        InitializeComponent();
    }

    private void FecharJanela_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}