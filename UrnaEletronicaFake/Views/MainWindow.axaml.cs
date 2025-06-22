using Avalonia.Controls;
using UrnaEletronicaFake.ViewModels;
using System;

namespace UrnaEletronicaFake.Views;

public partial class MainWindow : Window
{
    private VotacaoWindow? _votacaoWindow;

    public MainWindow()
    {
        InitializeComponent();

        // Acessar o ViewModel da urna
        if (DataContext is MainWindowViewModel mainVm)
        {
            if (mainVm.CurrentView is VotacaoViewModel votacaoVm)
            {
                votacaoVm.OnDesanexarUrna += AbrirUrnaEmNovaJanela;
                votacaoVm.OnAnexarUrna += FecharUrnaNovaJanela;
            }
        }
    }

    private void AbrirUrnaEmNovaJanela()
    {
        if (_votacaoWindow == null)
        {
            _votacaoWindow = new VotacaoWindow();
            _votacaoWindow.Closed += (s, e) =>
            {
                if (DataContext is MainWindowViewModel mainVm && mainVm.CurrentView is VotacaoViewModel votacaoVm)
                {
                    votacaoVm.UrnaDesanexada = false;
                }
                _votacaoWindow = null;
            };
            _votacaoWindow.Show();
        }
    }

    private void FecharUrnaNovaJanela()
    {
        _votacaoWindow?.Close();
        _votacaoWindow = null;
    }
}