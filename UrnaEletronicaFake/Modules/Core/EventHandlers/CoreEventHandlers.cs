using MediatR;
using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Modules.Core.Events;
using UrnaEletronicaFake.Services;

namespace UrnaEletronicaFake.Modules.Core.EventHandlers;

public class TerminalUnlockedEventHandler : INotificationHandler<TerminalUnlockedEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<TerminalUnlockedEventHandler> _logger;

    public TerminalUnlockedEventHandler(ITerminalLogService logService, ILogger<TerminalUnlockedEventHandler> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    public Task Handle(TerminalUnlockedEvent notification, CancellationToken cancellationToken)
    {
        var message = $"Terminal {notification.TerminalId} LIBERADO - Eleitor: {notification.EleitorId} | Mesário: {notification.MesarioId}";
        _logService.Registrar(message);
        _logger.LogInformation("Terminal unlocked. EleitorId: {EleitorId}, MesarioId: {MesarioId}, TerminalId: {TerminalId}", 
            notification.EleitorId, notification.MesarioId, notification.TerminalId);
        
        return Task.CompletedTask;
    }
}

public class TerminalLockedEventHandler : INotificationHandler<TerminalLockedEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<TerminalLockedEventHandler> _logger;

    public TerminalLockedEventHandler(ITerminalLogService logService, ILogger<TerminalLockedEventHandler> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    public Task Handle(TerminalLockedEvent notification, CancellationToken cancellationToken)
    {
        var message = $"Terminal {notification.TerminalId} BLOQUEADO - Motivo: {notification.Reason}";
        if (!string.IsNullOrEmpty(notification.PreviousEleitorId))
            message += $" | Eleitor anterior: {notification.PreviousEleitorId}";
            
        _logService.Registrar(message);
        _logger.LogInformation("Terminal locked. Reason: {Reason}, PreviousEleitorId: {PreviousEleitorId}, TerminalId: {TerminalId}", 
            notification.Reason, notification.PreviousEleitorId, notification.TerminalId);
        
        return Task.CompletedTask;
    }
}

public class VoteStartedEventHandler : INotificationHandler<VoteStartedEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<VoteStartedEventHandler> _logger;

    public VoteStartedEventHandler(ITerminalLogService logService, ILogger<VoteStartedEventHandler> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    public Task Handle(VoteStartedEvent notification, CancellationToken cancellationToken)
    {
        var message = $"VOTAÇÃO INICIADA - Eleitor: {notification.EleitorId} | Terminal: {notification.TerminalId}";
        _logService.Registrar(message);
        _logger.LogInformation("Vote started. EleitorId: {EleitorId}, TerminalId: {TerminalId}", 
            notification.EleitorId, notification.TerminalId);
        
        return Task.CompletedTask;
    }
}

public class VoteCompletedEventHandler : INotificationHandler<VoteCompletedEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<VoteCompletedEventHandler> _logger;

    public VoteCompletedEventHandler(ITerminalLogService logService, ILogger<VoteCompletedEventHandler> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    public Task Handle(VoteCompletedEvent notification, CancellationToken cancellationToken)
    {
        var status = notification.VoteConfirmed ? "CONFIRMADO" : "NÃO CONFIRMADO";
        var message = $"VOTAÇÃO FINALIZADA - Eleitor: {notification.EleitorId} | Status: {status} | Terminal: {notification.TerminalId}";
        _logService.Registrar(message);
        _logger.LogInformation("Vote completed. EleitorId: {EleitorId}, Confirmed: {Confirmed}, TerminalId: {TerminalId}", 
            notification.EleitorId, notification.VoteConfirmed, notification.TerminalId);
        
        return Task.CompletedTask;
    }
}

public class VoteAbortedEventHandler : INotificationHandler<VoteAbortedEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<VoteAbortedEventHandler> _logger;

    public VoteAbortedEventHandler(ITerminalLogService logService, ILogger<VoteAbortedEventHandler> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    public Task Handle(VoteAbortedEvent notification, CancellationToken cancellationToken)
    {
        var message = $"VOTAÇÃO CANCELADA - Eleitor: {notification.EleitorId} | Motivo: {notification.Reason} | Terminal: {notification.TerminalId}";
        _logService.Registrar(message);
        _logger.LogWarning("Vote aborted. EleitorId: {EleitorId}, Reason: {Reason}, TerminalId: {TerminalId}", 
            notification.EleitorId, notification.Reason, notification.TerminalId);
        
        return Task.CompletedTask;
    }
}

public class TerminalErrorEventHandler : INotificationHandler<TerminalErrorEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<TerminalErrorEventHandler> _logger;

    public TerminalErrorEventHandler(ITerminalLogService logService, ILogger<TerminalErrorEventHandler> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    public Task Handle(TerminalErrorEvent notification, CancellationToken cancellationToken)
    {
        var message = $"ERRO NO TERMINAL - {notification.ErrorMessage}";
        if (!string.IsNullOrEmpty(notification.EleitorId))
            message += $" | Eleitor: {notification.EleitorId}";
        message += $" | Terminal: {notification.TerminalId}";
            
        _logService.Registrar(message);
        _logger.LogError("Terminal error occurred. Error: {ErrorMessage}, Details: {ErrorDetails}, EleitorId: {EleitorId}, TerminalId: {TerminalId}", 
            notification.ErrorMessage, notification.ErrorDetails, notification.EleitorId, notification.TerminalId);
        
        return Task.CompletedTask;
    }
}