using Avalonia.Controls;
using UrnaEletronicaFake.ViewModels;

namespace UrnaEletronicaFake.Views;

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
} 