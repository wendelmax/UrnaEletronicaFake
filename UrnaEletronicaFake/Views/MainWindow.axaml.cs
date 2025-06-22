using Avalonia.Controls;
using UrnaEletronicaFake.ViewModels;
using System;
using Avalonia;

namespace UrnaEletronicaFake.Views;

public partial class MainWindow : Window
{
    private VotacaoWindow? _votacaoWindow;
    private VotacaoViewModel? _votacaoVmConectado;

    public MainWindow()
    {
        InitializeComponent();
        this.PropertyChanged += MainWindow_PropertyChanged;
        if (DataContext != null)
        {
            ConectarDataContext();
        }
    }

    private void MainWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == DataContextProperty && DataContext != null)
        {
            ConectarDataContext();
        }
    }

    private void ConectarDataContext()
    {
        if (DataContext is MainWindowViewModel mainVm)
        {
            mainVm.PropertyChanged -= MainVm_PropertyChanged; // Evita múltiplas inscrições
            mainVm.PropertyChanged += MainVm_PropertyChanged;
            ConectarEventosVotacao(mainVm.CurrentView);
        }
    }

    private void MainVm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.CurrentView) && DataContext is MainWindowViewModel mainVm)
        {
            ConectarEventosVotacao(mainVm.CurrentView);
        }
    }

    private void ConectarEventosVotacao(object? viewModel)
    {
        if (_votacaoVmConectado != null)
        {
            _votacaoVmConectado.OnDesanexarUrna -= AbrirUrnaEmNovaJanela;
            _votacaoVmConectado.OnAnexarUrna -= FecharUrnaNovaJanela;
        }

        if (viewModel is VotacaoViewModel votacaoVm)
        {
            votacaoVm.OnDesanexarUrna += AbrirUrnaEmNovaJanela;
            votacaoVm.OnAnexarUrna += FecharUrnaNovaJanela;
            _votacaoVmConectado = votacaoVm;
        }
        else
        {
            _votacaoVmConectado = null;
        }
    }

    private async void AbrirUrnaEmNovaJanela()
    {
        if (_votacaoWindow == null && _votacaoVmConectado != null)
        {
            _votacaoWindow = new VotacaoWindow(_votacaoVmConectado);
            _votacaoWindow.Closed += (s, e) =>
            {
                if (DataContext is MainWindowViewModel mainVm && mainVm.CurrentView is VotacaoViewModel votacaoVm)
                {
                    if (votacaoVm.UrnaDesanexada)
                    {
                        votacaoVm.AlternarUrnaCommand.Execute(null);
                    }
                }
                _votacaoWindow = null;
            };
            await _votacaoWindow.ShowDialog(this);
        }
    }

    private void FecharUrnaNovaJanela()
    {
        _votacaoWindow?.Close();
    }
}