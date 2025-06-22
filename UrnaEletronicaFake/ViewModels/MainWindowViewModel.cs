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

namespace UrnaEletronicaFake.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IEleicaoService _eleicaoService;
    private readonly IVotoService _votoService;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IServiceProvider _serviceProvider;
    
    private UserControl? _currentView;
    private string _statusMessage = "Sistema de Urna Eletrônica";
    private bool _isLoading;

    public MainWindowViewModel(IEleicaoService eleicaoService, IVotoService votoService, IAuditoriaService auditoriaService, IServiceProvider serviceProvider)
    {
        _eleicaoService = eleicaoService;
        _votoService = votoService;
        _auditoriaService = auditoriaService;
        _serviceProvider = serviceProvider;
        
        // Inicializar comandos
        ShowAdminCommand = new RelayCommand(ShowAdmin);
        ShowVotacaoCommand = new RelayCommand(ShowVotacao);
        ShowResultadosCommand = new RelayCommand(ShowResultados);
        ShowAuditoriaCommand = new RelayCommand(ShowAuditoria);
        
        // Inicializar com a tela de votação
        ShowVotacao();
    }

    public UserControl? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand ShowAdminCommand { get; }
    public ICommand ShowVotacaoCommand { get; }
    public ICommand ShowResultadosCommand { get; }
    public ICommand ShowAuditoriaCommand { get; }

    private void ShowAdmin()
    {
        var adminView = _serviceProvider.GetRequiredService<AdminView>();
        adminView.DataContext = _serviceProvider.GetRequiredService<AdminViewModel>();
        CurrentView = adminView;
        StatusMessage = "Painel Administrativo";
    }

    private void ShowVotacao()
    {
        var votacaoView = _serviceProvider.GetRequiredService<VotacaoView>();
        votacaoView.DataContext = _serviceProvider.GetRequiredService<VotacaoViewModel>();
        CurrentView = votacaoView;
        StatusMessage = "Tela de Votação";
    }

    private void ShowResultados()
    {
        var resultadosView = _serviceProvider.GetRequiredService<ResultadosView>();
        resultadosView.DataContext = _serviceProvider.GetRequiredService<ResultadosViewModel>();
        CurrentView = resultadosView;
        StatusMessage = "Resultados da Eleição";
    }

    private void ShowAuditoria()
    {
        var auditoriaView = _serviceProvider.GetRequiredService<AuditoriaView>();
        auditoriaView.DataContext = _serviceProvider.GetRequiredService<AuditoriaViewModel>();
        CurrentView = auditoriaView;
        StatusMessage = "Log de Auditoria";
    }
}
