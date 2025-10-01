using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using UrnaEletronicaFake.UI.Services;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.UI.ViewModels;

public partial class MesaViewModel : ViewModelBase
{
    private readonly IVotacaoStateService _votacaoStateService;
    private readonly ITerminalLogService _terminalLogService;

    [ObservableProperty]
    private string _statusMessage = "Aguardando identificação do eleitor.";
    
    [ObservableProperty]
    private string _identificacaoEleitor = "";

    [ObservableProperty]
    private string _mesarioId = "";

    [ObservableProperty]
    private string _nomeMesario = "";

    [ObservableProperty]
    private string _nomeEleitor = "";

    [ObservableProperty]
    private string _terminalId = "MESA-001";

    [ObservableProperty]
    private DateTime? _ultimaLiberacao;

    [ObservableProperty]
    private DateTime? _ultimoBloqueio;

    public ICommand LiberarUrnaCommand { get; }
    public ICommand BloquearUrnaCommand { get; }
    public ICommand LimparLogCommand { get; }

    public MesaViewModel(IVotacaoStateService votacaoStateService, ITerminalLogService terminalLogService, ILogger<MesaViewModel> logger) : base(logger)
    {
        _votacaoStateService = votacaoStateService;
        _terminalLogService = terminalLogService;

        _votacaoStateService.OnTerminalStateChanged += OnTerminalStateChanged;
        UpdateStatusMessage();
        
        // Comandos
        LiberarUrnaCommand = new RelayCommand(LiberarUrna, CanLiberarUrna);
        BloquearUrnaCommand = new RelayCommand(BloquearUrna, CanBloquearUrna);
        LimparLogCommand = new RelayCommand(LimparLog);
    }
    
    private void OnTerminalStateChanged()
    {
        ((RelayCommand)LiberarUrnaCommand).NotifyCanExecuteChanged();
        ((RelayCommand)BloquearUrnaCommand).NotifyCanExecuteChanged();
        UpdateStatusMessage();
        
        // Limpa o campo de identificação quando a urna for bloqueada novamente
        if (_votacaoStateService.IsTerminalLocked)
        {
            IdentificacaoEleitor = "";
            NomeEleitor = "";
            UltimoBloqueio = DateTime.Now;
            _terminalLogService.Registrar("Urna bloqueada. Aguardando nova identificação de eleitor.");
        }
        else
        {
            UltimaLiberacao = DateTime.Now;
        }
    }
    
    private void UpdateStatusMessage()
    {
        if (_votacaoStateService.IsTerminalLocked)
        {
            StatusMessage = "Aguardando identificação do eleitor.";
        }
        else
        {
            StatusMessage = $"Urna liberada para o eleitor: {_votacaoStateService.EleitorAutenticadoId}.\nAguardando finalização do voto.";
        }
    }

    private bool CanLiberarUrna()
    {
        return _votacaoStateService.IsTerminalLocked && 
               !string.IsNullOrWhiteSpace(IdentificacaoEleitor) &&
               !string.IsNullOrWhiteSpace(MesarioId);
    }

    private bool CanBloquearUrna()
    {
        return !_votacaoStateService.IsTerminalLocked;
    }

    private void LiberarUrna()
    {
        _votacaoStateService.UnlockTerminal(IdentificacaoEleitor);
        UltimaLiberacao = DateTime.Now;
        _terminalLogService.Registrar($"Eleitor identificado: {IdentificacaoEleitor} ({NomeEleitor}). Urna liberada para votação pelo mesário: {MesarioId} ({NomeMesario}).");
    }

    private void BloquearUrna()
    {
        _votacaoStateService.LockTerminal();
        UltimoBloqueio = DateTime.Now;
        _terminalLogService.Registrar($"Urna bloqueada pelo mesário: {MesarioId} ({NomeMesario}).");
    }

    private void LimparLog()
    {
        LogInformation("Log do terminal limpo");
    }
} 