using Avalonia.Controls;
using UrnaEletronicaFake.ViewModels;

namespace UrnaEletronicaFake.Views;

public partial class MesaWindow : Window
{
    public MesaWindow()
    {
        InitializeComponent();
    }
    
    public MesaWindow(MesaViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
