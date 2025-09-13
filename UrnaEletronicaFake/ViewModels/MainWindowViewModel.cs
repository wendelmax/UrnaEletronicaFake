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
    private readonly IWindowManagerService? _windowManagerService;
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly MesaViewModel _mesaViewModel;
    private readonly VotacaoViewModel _votacaoViewModel;

    [ObservableProperty]
    private ViewModelBase? _currentView;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private bool _dashboardWindowEstaAberta;
    
    [ObservableProperty]
    private bool _mesaWindowEstaAberta;
    
    [ObservableProperty]
    private bool _votacaoWindowEstaAberta;

    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowAdminCommand { get; }
    public ICommand ShowVotacaoCommand { get; }
    public ICommand ShowResultadosCommand { get; }
    public ICommand ShowAuditoriaCommand { get; }
    public ICommand ShowMesaCommand { get; }
    public ICommand AbrirDashboardWindowCommand { get; }
    public ICommand AbrirMesaWindowCommand { get; }
    public ICommand AbrirVotacaoWindowCommand { get; }
    public ICommand TrazerDashboardWindowCommand { get; }
    public ICommand TrazerMesaWindowCommand { get; }
    public ICommand TrazerVotacaoWindowCommand { get; }
    public ICommand FecharTodasJanelasCommand { get; }

    public MainWindowViewModel(
        DashboardView dashboardView, DashboardViewModel dashboardViewModel,
        AdminView adminView, AdminViewModel adminViewModel,
        VotacaoView votacaoView, VotacaoViewModel votacaoViewModel,
        ResultadosView resultadosView, ResultadosViewModel resultadosViewModel,
        AuditoriaView auditoriaView, AuditoriaViewModel auditoriaViewModel,
        MesaView mesaView, MesaViewModel mesaViewModel,
        IWindowManagerService? windowManagerService = null)
    {
        _windowManagerService = windowManagerService;
        _dashboardViewModel = dashboardViewModel;
        _mesaViewModel = mesaViewModel;
        _votacaoViewModel = votacaoViewModel;
        
        // Vincular Views aos seus ViewModels
        dashboardView.DataContext = dashboardViewModel;
        adminView.DataContext = adminViewModel;
        votacaoView.DataContext = votacaoViewModel;
        resultadosView.DataContext = resultadosViewModel;
        auditoriaView.DataContext = auditoriaViewModel;
        mesaView.DataContext = mesaViewModel;

        // Comandos de navegação
        ShowDashboardCommand = new RelayCommand(() => { CurrentView = dashboardViewModel; StatusMessage = "Dashboard de Monitoramento"; });
        ShowAdminCommand = new RelayCommand(() => { CurrentView = adminViewModel; StatusMessage = "Painel Administrativo"; });
        ShowVotacaoCommand = new RelayCommand(() => { CurrentView = votacaoViewModel; StatusMessage = "Terminal de Votação"; });
        ShowResultadosCommand = new RelayCommand(() => { CurrentView = resultadosViewModel; StatusMessage = "Resultados da Eleição"; });
        ShowAuditoriaCommand = new RelayCommand(() => { CurrentView = auditoriaViewModel; StatusMessage = "Log de Auditoria"; });
        ShowMesaCommand = new RelayCommand(() => { CurrentView = mesaViewModel; StatusMessage = "Mesa Receptora de Votos"; });
        
        // Comandos para abrir janelas separadas
        AbrirDashboardWindowCommand = new RelayCommand(() => AbrirDashboardWindow());
        AbrirMesaWindowCommand = new RelayCommand(() => AbrirMesaWindow());
        AbrirVotacaoWindowCommand = new RelayCommand(() => AbrirVotacaoWindow());
        
        // Comandos para controlar janelas
        TrazerDashboardWindowCommand = new RelayCommand(() => TrazerDashboardWindow());
        TrazerMesaWindowCommand = new RelayCommand(() => TrazerMesaWindow());
        TrazerVotacaoWindowCommand = new RelayCommand(() => TrazerVotacaoWindow());
        FecharTodasJanelasCommand = new RelayCommand(() => FecharTodasJanelas());
        
        // View inicial - Painel de Controle
        CurrentView = null;
        StatusMessage = "Painel de Controle Central";
    }
    
    private void AbrirDashboardWindow()
    {
        if (_windowManagerService != null && !DashboardWindowEstaAberta)
        {
            _windowManagerService.AbrirDashboardWindow(_dashboardViewModel);
            DashboardWindowEstaAberta = true;
            StatusMessage = "Dashboard aberto em janela separada";
        }
    }
    
    private void AbrirMesaWindow()
    {
        if (_windowManagerService != null && !MesaWindowEstaAberta)
        {
            _windowManagerService.AbrirMesaWindow(_mesaViewModel);
            MesaWindowEstaAberta = true;
            StatusMessage = "Mesa Receptora aberta em janela separada";
        }
    }
    
    private void AbrirVotacaoWindow()
    {
        if (_windowManagerService != null && !VotacaoWindowEstaAberta)
        {
            _windowManagerService.AbrirVotacaoWindow(_votacaoViewModel);
            VotacaoWindowEstaAberta = true;
            StatusMessage = "Terminal de Votação aberto em janela separada";
        }
    }
    
    public void AtualizarStatusDashboardWindow(bool aberta)
    {
        DashboardWindowEstaAberta = aberta;
        if (!aberta)
        {
            StatusMessage = "Dashboard fechado";
        }
    }
    
    public void AtualizarStatusMesaWindow(bool aberta)
    {
        MesaWindowEstaAberta = aberta;
        if (!aberta)
        {
            StatusMessage = "Mesa Receptora fechada";
        }
    }
    
    public void AtualizarStatusVotacaoWindow(bool aberta)
    {
        VotacaoWindowEstaAberta = aberta;
        if (!aberta)
        {
            StatusMessage = "Terminal de Votação fechado";
        }
    }
    
    private void TrazerDashboardWindow()
    {
        if (_windowManagerService != null && DashboardWindowEstaAberta)
        {
            _windowManagerService.TrazerDashboardWindowParaFrente();
            StatusMessage = "Dashboard trazido para frente";
        }
    }
    
    private void TrazerMesaWindow()
    {
        if (_windowManagerService != null && MesaWindowEstaAberta)
        {
            _windowManagerService.TrazerMesaWindowParaFrente();
            StatusMessage = "Mesa Receptora trazida para frente";
        }
    }
    
    private void TrazerVotacaoWindow()
    {
        if (_windowManagerService != null && VotacaoWindowEstaAberta)
        {
            _windowManagerService.TrazerVotacaoWindowParaFrente();
            StatusMessage = "Terminal de Votação trazido para frente";
        }
    }
    
    private void FecharTodasJanelas()
    {
        if (_windowManagerService != null)
        {
            _windowManagerService.FecharTodasAsJanelas();
            DashboardWindowEstaAberta = false;
            MesaWindowEstaAberta = false;
            VotacaoWindowEstaAberta = false;
            StatusMessage = "Todas as janelas foram fechadas";
        }
    }
}
