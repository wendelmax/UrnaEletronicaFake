using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using UrnaEletronicaFake.Services;

namespace UrnaEletronicaFake.ViewModels;

public partial class MesaViewModel : ViewModelBase
{
    private readonly IVotacaoStateService _votacaoStateService;
    private readonly ITerminalLogService _terminalLogService;

    [ObservableProperty]
    private string _statusMessage = "Aguardando identificação do eleitor.";
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LiberarUrnaCommand))]
    private string _identificacaoEleitor = "";

    public IRelayCommand LiberarUrnaCommand { get; }

    public MesaViewModel(IVotacaoStateService votacaoStateService, ITerminalLogService terminalLogService)
    {
        _votacaoStateService = votacaoStateService;
        _terminalLogService = terminalLogService;
        LiberarUrnaCommand = new RelayCommand(LiberarUrna, CanLiberarUrna);

        _votacaoStateService.OnTerminalStateChanged += OnTerminalStateChanged;
        UpdateStatusMessage();
    }
    
    private void OnTerminalStateChanged()
    {
        LiberarUrnaCommand.NotifyCanExecuteChanged();
        UpdateStatusMessage();
        
        // Limpa o campo de identificação quando a urna for bloqueada novamente
        if (_votacaoStateService.IsTerminalLocked)
        {
            IdentificacaoEleitor = "";
            _terminalLogService.Registrar("Urna bloqueada. Aguardando nova identificação de eleitor.");
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
        return _votacaoStateService.IsTerminalLocked && !string.IsNullOrWhiteSpace(IdentificacaoEleitor);
    }

    private void LiberarUrna()
    {
        _votacaoStateService.UnlockTerminal(IdentificacaoEleitor);
        _terminalLogService.Registrar($"Eleitor identificado: {IdentificacaoEleitor}. Urna liberada para votação.");
    }
} 