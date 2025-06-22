using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using UrnaEletronicaFake.Services;

namespace UrnaEletronicaFake.ViewModels;

public partial class MesaViewModel : ViewModelBase
{
    private readonly IVotacaoStateService _votacaoStateService;

    [ObservableProperty]
    private string _statusMessage = "Urna pronta para ser liberada.";

    public IRelayCommand UnlockTerminalCommand { get; }

    public MesaViewModel(IVotacaoStateService votacaoStateService)
    {
        _votacaoStateService = votacaoStateService;
        UnlockTerminalCommand = new RelayCommand(UnlockTerminal, () => _votacaoStateService.IsTerminalLocked);

        _votacaoStateService.OnTerminalStateChanged += OnTerminalStateChanged;
        UpdateStatusMessage();
    }
    
    private void OnTerminalStateChanged()
    {
        UnlockTerminalCommand.NotifyCanExecuteChanged();
        UpdateStatusMessage();
    }
    
    private void UpdateStatusMessage()
    {
        StatusMessage = _votacaoStateService.IsTerminalLocked 
            ? "Urna pronta para ser liberada para o próximo eleitor."
            : "Urna em uso. Aguardando finalização do voto.";
    }

    private void UnlockTerminal()
    {
        _votacaoStateService.UnlockTerminal();
    }
} 