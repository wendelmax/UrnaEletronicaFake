using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Core.Interfaces;
using UrnaEletronicaFake.Core.Events;

namespace UrnaEletronicaFake.Core.EventHandlers;

public class CoreEventHandlers : 
    INotificationHandler<TerminalUnlockedEvent>,
    INotificationHandler<TerminalLockedEvent>,
    INotificationHandler<VoteCompletedEvent>,
    INotificationHandler<VoteCancelledEvent>
{
    private readonly ILogger<CoreEventHandlers> _logger;

    public CoreEventHandlers(ILogger<CoreEventHandlers> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TerminalUnlockedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Terminal unlocked for eleitor {EleitorId} on terminal {TerminalId} at {Timestamp}", 
            notification.EleitorId, notification.TerminalId ?? "default", notification.Timestamp);
        
        await Task.CompletedTask;
    }

    public async Task Handle(TerminalLockedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Terminal locked for eleitor {EleitorId} on terminal {TerminalId} at {Timestamp}. Reason: {Reason}", 
            notification.EleitorId ?? "unknown", notification.TerminalId ?? "default", notification.Timestamp, notification.Reason ?? "unknown");
        
        await Task.CompletedTask;
    }

    public async Task Handle(VoteCompletedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Vote completed for eleitor {EleitorId} on terminal {TerminalId} at {Timestamp}. Success: {Success}", 
            notification.VoteRequest.EleitorId, notification.VoteRequest.TerminalId ?? "default", notification.Timestamp, notification.VoteResult.Success);
        
        await Task.CompletedTask;
    }

    public async Task Handle(VoteCancelledEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Vote cancelled for eleitor {EleitorId} on terminal {TerminalId} at {Timestamp}. Reason: {Reason}", 
            notification.EleitorId, notification.TerminalId ?? "default", notification.Timestamp, notification.Reason ?? "unknown");
        
        await Task.CompletedTask;
    }
}