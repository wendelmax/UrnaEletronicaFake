using Avalonia.Controls;
using UrnaEletronicaFake.UI.ViewModels;

namespace UrnaEletronicaFake.UI.Views;

public partial class VotacaoWindow : Window
{
    public VotacaoWindow()
    {
        InitializeComponent();
    }
    
    public VotacaoWindow(VotacaoViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
    
    private void FecharJanela_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
} 