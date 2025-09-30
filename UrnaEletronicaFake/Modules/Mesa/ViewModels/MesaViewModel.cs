using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Modules.Core.Events;
using UrnaEletronicaFake.Modules.Core.Services;
using UrnaEletronicaFake.Services;
using UrnaEletronicaFake.ViewModels;

namespace UrnaEletronicaFake.Modules.Mesa.ViewModels;

public partial class MesaViewModel : ViewModelBase, IDisposable
{
    private readonly ITerminalStateService _terminalStateService;
    private readonly ITerminalLogService _terminalLogService;
    private readonly ILogger<MesaViewModel> _logger;
    private readonly IEventBus _eventBus;
    private bool _disposed = false;

    [ObservableProperty]
    private string _statusMessage = "Aguardando identificação do eleitor.";
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LiberarUrnaCommand))]
    [NotifyCanExecuteChangedFor(nameof(BloquearUrnaCommand))]
    private string _identificacaoEleitor = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LiberarUrnaCommand))]
    private string _mesarioId = Environment.UserName;

    [ObservableProperty]
    private bool _isTerminalLocked = true;

    [ObservableProperty]
    private string? _eleitorAtualId;

    [ObservableProperty]
    private DateTime? _ultimaLiberacao;

    [ObservableProperty]
    private DateTime? _ultimoBloqueio;

    [ObservableProperty]
    private string _terminalId = Environment.MachineName;

    public IAsyncRelayCommand LiberarUrnaCommand { get; }
    public IAsyncRelayCommand BloquearUrnaCommand { get; }
    public IRelayCommand LimparLogCommand { get; }

    public MesaViewModel(
        ITerminalStateService terminalStateService,
        ITerminalLogService terminalLogService,
        IEventBus eventBus,
        ILogger<MesaViewModel> logger)
    {
        _terminalStateService = terminalStateService;
        _terminalLogService = terminalLogService;
        _eventBus = eventBus;
        _logger = logger;

        LiberarUrnaCommand = new AsyncRelayCommand(LiberarUrnaAsync, CanLiberarUrna);
        BloquearUrnaCommand = new AsyncRelayCommand(BloquearUrnaAsync, CanBloquearUrna);
        LimparLogCommand = new RelayCommand(LimparLog);

        SubscribeToEvents();
        UpdateUIFromState();
        
        _logger.LogInformation("MesaViewModel inicializado para Terminal {TerminalId}", _terminalId);
        _terminalLogService.Registrar($"Sistema da Mesa iniciado - Terminal: {_terminalId}");
    }

    private void SubscribeToEvents()
    {
        _terminalStateService.OnTerminalUnlocked += OnTerminalUnlockedAsync;
        _terminalStateService.OnTerminalLocked += OnTerminalLockedAsync;
        _terminalStateService.OnVoteStarted += OnVoteStartedAsync;
        _terminalStateService.OnVoteCompleted += OnVoteCompletedAsync;
        _terminalStateService.OnVoteAborted += OnVoteAbortedAsync;
        _terminalStateService.OnTerminalError += OnTerminalErrorAsync;
    }

    private void UnsubscribeFromEvents()
    {
        _terminalStateService.OnTerminalUnlocked -= OnTerminalUnlockedAsync;
        _terminalStateService.OnTerminalLocked -= OnTerminalLockedAsync;
        _terminalStateService.OnVoteStarted -= OnVoteStartedAsync;
        _terminalStateService.OnVoteCompleted -= OnVoteCompletedAsync;
        _terminalStateService.OnVoteAborted -= OnVoteAbortedAsync;
        _terminalStateService.OnTerminalError -= OnTerminalErrorAsync;
    }

    private void UpdateUIFromState()
    {
        IsTerminalLocked = _terminalStateService.IsTerminalLocked;
        EleitorAtualId = _terminalStateService.EleitorAutenticadoId;
        UltimaLiberacao = _terminalStateService.UltimaLiberacao;
        UltimoBloqueio = _terminalStateService.UltimoBloqueio;
        
        UpdateStatusMessage();
        UpdateCommandStates();
    }

    private void UpdateStatusMessage()
    {
        if (IsTerminalLocked)
        {
            if (UltimoBloqueio.HasValue)
            {
                StatusMessage = $"Terminal bloqueado desde {UltimoBloqueio:HH:mm:ss}.\nAguardando identificação do próximo eleitor.";
            }
            else
            {
                StatusMessage = "Aguardando identificação do eleitor.";
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(EleitorAtualId) && UltimaLiberacao.HasValue)
            {
                StatusMessage = $"Urna liberada para: {EleitorAtualId}\nDesde: {UltimaLiberacao:HH:mm:ss}\nAguardando finalização do voto.";
            }
            else
            {
                StatusMessage = "Terminal liberado. Aguardando início da votação.";
            }
        }
    }

    private void UpdateCommandStates()
    {
        LiberarUrnaCommand.NotifyCanExecuteChanged();
        BloquearUrnaCommand.NotifyCanExecuteChanged();
    }

    private bool CanLiberarUrna()
    {
        return IsTerminalLocked && 
               !string.IsNullOrWhiteSpace(IdentificacaoEleitor) && 
               !string.IsNullOrWhiteSpace(MesarioId);
    }

    private bool CanBloquearUrna()
    {
        return !IsTerminalLocked;
    }

    private async Task LiberarUrnaAsync()
    {
        try
        {
            if (!CanLiberarUrna()) 
                return;

            _logger.LogInformation("Tentativa de liberação do terminal. EleitorId: {EleitorId}, MesarioId: {MesarioId}", 
                IdentificacaoEleitor, MesarioId);

            await _terminalStateService.UnlockTerminalAsync(IdentificacaoEleitor, MesarioId);
            
            IdentificacaoEleitor = "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao liberar terminal para eleitor {EleitorId}", IdentificacaoEleitor);
            await _terminalStateService.HandleTerminalErrorAsync($"Erro ao liberar terminal: {ex.Message}", ex.ToString());
        }
    }

    private async Task BloquearUrnaAsync()
    {
        try
        {
            if (!CanBloquearUrna()) 
                return;

            _logger.LogInformation("Bloqueio manual do terminal solicitado pelo mesário {MesarioId}", MesarioId);
            
            await _terminalStateService.LockTerminalAsync("Bloqueio manual pelo mesário");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao bloquear terminal");
            await _terminalStateService.HandleTerminalErrorAsync($"Erro ao bloquear terminal: {ex.Message}", ex.ToString());
        }
    }

    private void LimparLog()
    {
        _terminalLogService.Limpar();
        _terminalLogService.Registrar($"Log limpo pelo mesário - Terminal: {_terminalId}");
        _logger.LogInformation("Log limpo pelo mesário no terminal {TerminalId}", _terminalId);
    }

    private async Task OnTerminalUnlockedAsync(TerminalUnlockedEvent eventArgs)
    {
        await Task.Run(() => 
        {
            UpdateUIFromState();
        });
    }

    private async Task OnTerminalLockedAsync(TerminalLockedEvent eventArgs)
    {
        await Task.Run(() => 
        {
            UpdateUIFromState();
        });
    }

    private async Task OnVoteStartedAsync(VoteStartedEvent eventArgs)
    {
        await Task.Run(() => 
        {
            UpdateStatusMessage();
        });
    }

    private async Task OnVoteCompletedAsync(VoteCompletedEvent eventArgs)
    {
        await Task.Run(() => 
        {
            UpdateUIFromState();
        });
    }

    private async Task OnVoteAbortedAsync(VoteAbortedEvent eventArgs)
    {
        await Task.Run(() => 
        {
            UpdateUIFromState();
        });
    }

    private async Task OnTerminalErrorAsync(TerminalErrorEvent eventArgs)
    {
        await Task.Run(() => 
        {
            _logger.LogWarning("Erro recebido via evento: {ErrorMessage}", eventArgs.ErrorMessage);
            UpdateStatusMessage();
        });
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            UnsubscribeFromEvents();
            _disposed = true;
            _logger.LogInformation("MesaViewModel disposed para Terminal {TerminalId}", _terminalId);
        }
    }
}