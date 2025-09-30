using UrnaEletronicaFake.Modules.Core.Events;

namespace UrnaEletronicaFake.Modules.Core.Services;

public interface ITerminalStateService
{
    bool IsTerminalLocked { get; }
    string? EleitorAutenticadoId { get; }
    string? MesarioAtualId { get; }
    DateTime? UltimaLiberacao { get; }
    DateTime? UltimoBloqueio { get; }
    
    Task UnlockTerminalAsync(string eleitorId, string mesarioId);
    Task LockTerminalAsync(string reason = "Voto finalizado");
    Task HandleVoteStartedAsync(string eleitorId);
    Task HandleVoteCompletedAsync(string eleitorId, bool confirmed);
    Task HandleVoteAbortedAsync(string eleitorId, string reason);
    Task HandleTerminalErrorAsync(string errorMessage, string? details = null);
    
    event Func<TerminalUnlockedEvent, Task>? OnTerminalUnlocked;
    event Func<TerminalLockedEvent, Task>? OnTerminalLocked;
    event Func<VoteStartedEvent, Task>? OnVoteStarted;
    event Func<VoteCompletedEvent, Task>? OnVoteCompleted;
    event Func<VoteAbortedEvent, Task>? OnVoteAborted;
    event Func<TerminalErrorEvent, Task>? OnTerminalError;
}

public class TerminalStateService : ITerminalStateService
{
    private readonly IEventBus _eventBus;
    private bool _isLocked = true;
    private string? _eleitorId;
    private string? _mesarioId;
    private DateTime? _ultimaLiberacao;
    private DateTime? _ultimoBloqueio;

    public TerminalStateService(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _ultimoBloqueio = DateTime.Now;
    }

    public bool IsTerminalLocked => _isLocked;
    public string? EleitorAutenticadoId => _eleitorId;
    public string? MesarioAtualId => _mesarioId;
    public DateTime? UltimaLiberacao => _ultimaLiberacao;
    public DateTime? UltimoBloqueio => _ultimoBloqueio;

    public event Func<TerminalUnlockedEvent, Task>? OnTerminalUnlocked;
    public event Func<TerminalLockedEvent, Task>? OnTerminalLocked;
    public event Func<VoteStartedEvent, Task>? OnVoteStarted;
    public event Func<VoteCompletedEvent, Task>? OnVoteCompleted;
    public event Func<VoteAbortedEvent, Task>? OnVoteAborted;
    public event Func<TerminalErrorEvent, Task>? OnTerminalError;

    public async Task UnlockTerminalAsync(string eleitorId, string mesarioId)
    {
        if (_isLocked && !string.IsNullOrWhiteSpace(eleitorId) && !string.IsNullOrWhiteSpace(mesarioId))
        {
            _isLocked = false;
            _eleitorId = eleitorId;
            _mesarioId = mesarioId;
            _ultimaLiberacao = DateTime.Now;

            var eventObj = new TerminalUnlockedEvent { EleitorId = eleitorId, MesarioId = mesarioId };
            await _eventBus.PublishAsync(eventObj);
            
            if (OnTerminalUnlocked != null)
                await OnTerminalUnlocked(eventObj);
        }
    }

    public async Task LockTerminalAsync(string reason = "Voto finalizado")
    {
        if (!_isLocked)
        {
            var previousEleitorId = _eleitorId;
            _isLocked = true;
            _eleitorId = null;
            _mesarioId = null;
            _ultimoBloqueio = DateTime.Now;

            var eventObj = new TerminalLockedEvent { PreviousEleitorId = previousEleitorId, Reason = reason };
            await _eventBus.PublishAsync(eventObj);
            
            if (OnTerminalLocked != null)
                await OnTerminalLocked(eventObj);
        }
    }

    public async Task HandleVoteStartedAsync(string eleitorId)
    {
        var eventObj = new VoteStartedEvent { EleitorId = eleitorId };
        await _eventBus.PublishAsync(eventObj);
        
        if (OnVoteStarted != null)
            await OnVoteStarted(eventObj);
    }

    public async Task HandleVoteCompletedAsync(string eleitorId, bool confirmed)
    {
        var eventObj = new VoteCompletedEvent { EleitorId = eleitorId, VoteConfirmed = confirmed };
        await _eventBus.PublishAsync(eventObj);
        
        if (OnVoteCompleted != null)
            await OnVoteCompleted(eventObj);

        if (confirmed)
        {
            await LockTerminalAsync("Voto confirmado");
        }
    }

    public async Task HandleVoteAbortedAsync(string eleitorId, string reason)
    {
        var eventObj = new VoteAbortedEvent { EleitorId = eleitorId, Reason = reason };
        await _eventBus.PublishAsync(eventObj);
        
        if (OnVoteAborted != null)
            await OnVoteAborted(eventObj);

        await LockTerminalAsync($"Voto cancelado: {reason}");
    }

    public async Task HandleTerminalErrorAsync(string errorMessage, string? details = null)
    {
        var eventObj = new TerminalErrorEvent 
        { 
            ErrorMessage = errorMessage, 
            ErrorDetails = details,
            EleitorId = _eleitorId 
        };
        await _eventBus.PublishAsync(eventObj);
        
        if (OnTerminalError != null)
            await OnTerminalError(eventObj);
    }
}