using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using UrnaEletronicaFake.Models;
using UrnaEletronicaFake.Services;
using UrnaEletronicaFake.ViewModels;
using UrnaEletronicaFake.Views;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace UrnaEletronicaFake.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase? _currentView;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isLoading;

    public ICommand ShowAdminCommand { get; }
    public ICommand ShowVotacaoCommand { get; }
    public ICommand ShowResultadosCommand { get; }
    public ICommand ShowAuditoriaCommand { get; }
    public ICommand ShowMesaCommand { get; }

    public MainWindowViewModel(
        AdminView adminView, AdminViewModel adminViewModel,
        VotacaoView votacaoView, VotacaoViewModel votacaoViewModel,
        ResultadosView resultadosView, ResultadosViewModel resultadosViewModel,
        AuditoriaView auditoriaView, AuditoriaViewModel auditoriaViewModel,
        MesaView mesaView, MesaViewModel mesaViewModel)
    {
        // Vincular Views aos seus ViewModels
        adminView.DataContext = adminViewModel;
        votacaoView.DataContext = votacaoViewModel;
        resultadosView.DataContext = resultadosViewModel;
        auditoriaView.DataContext = auditoriaViewModel;
        mesaView.DataContext = mesaViewModel;

        // Comandos de navegação
        ShowAdminCommand = new RelayCommand(() => { CurrentView = adminViewModel; StatusMessage = "Painel Administrativo"; });
        ShowVotacaoCommand = new RelayCommand(() => { CurrentView = votacaoViewModel; StatusMessage = "Terminal de Votação"; });
        ShowResultadosCommand = new RelayCommand(() => { CurrentView = resultadosViewModel; StatusMessage = "Resultados da Eleição"; });
        ShowAuditoriaCommand = new RelayCommand(() => { CurrentView = auditoriaViewModel; StatusMessage = "Log de Auditoria"; });
        ShowMesaCommand = new RelayCommand(() => { CurrentView = mesaViewModel; StatusMessage = "Mesa Receptora de Votos"; });
        
        // View inicial
        CurrentView = adminViewModel;
        StatusMessage = "Painel Administrativo";
    }
}
