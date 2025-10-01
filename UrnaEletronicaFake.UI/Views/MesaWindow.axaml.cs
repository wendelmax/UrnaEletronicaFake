using Avalonia.Controls;
using Avalonia.Interactivity;

namespace UrnaEletronicaFake.UI.Views;

public partial class MesaWindow : Window
{
    public MesaWindow()
    {
        InitializeComponent();
    }

    private void FecharJanela_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}