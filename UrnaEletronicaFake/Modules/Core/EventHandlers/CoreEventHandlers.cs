using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Modules.Core.Events;
using UrnaEletronicaFake.Services;

namespace UrnaEletronicaFake.Modules.Core.EventHandlers;

public class TerminalStartedEventHandler : INotificationHandler<TerminalStartedEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<TerminalStartedEventHandler> _logger;

    public TerminalStartedEventHandler(
        ITerminalLogService logService,
        ILogger<TerminalStartedEventHandler> logger)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TerminalStartedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Terminal {TerminalId} iniciado em {StartTime}", 
            notification.TerminalId, notification.StartTime);
            
        _logService.Registrar($"Terminal {notification.TerminalId} iniciado em {notification.StartTime:HH:mm:ss}");
        
        return Task.CompletedTask;
    }
}

public class TerminalStoppedEventHandler : INotificationHandler<TerminalStoppedEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<TerminalStoppedEventHandler> _logger;

    public TerminalStoppedEventHandler(
        ITerminalLogService logService,
        ILogger<TerminalStoppedEventHandler> logger)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TerminalStoppedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Terminal {TerminalId} parado em {StopTime}", 
            notification.TerminalId, notification.StopTime);
            
        _logService.Registrar($"Terminal {notification.TerminalId} parado em {notification.StopTime:HH:mm:ss}");
        
        return Task.CompletedTask;
    }
}

public class TerminalStateChangedEventHandler : INotificationHandler<TerminalStateChangedEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<TerminalStateChangedEventHandler> _logger;

    public TerminalStateChangedEventHandler(
        ITerminalLogService logService,
        ILogger<TerminalStateChangedEventHandler> logger)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TerminalStateChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Terminal {TerminalId} mudou estado de {PreviousState} para {CurrentState}",
            notification.TerminalId, notification.PreviousState, notification.CurrentState);
            
        _logService.Registrar($"Estado alterado: {notification.PreviousState} → {notification.CurrentState}");
        
        return Task.CompletedTask;
    }
}

public class TerminalErrorEventHandler : INotificationHandler<TerminalErrorEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<TerminalErrorEventHandler> _logger;

    public TerminalErrorEventHandler(
        ITerminalLogService logService,
        ILogger<TerminalErrorEventHandler> logger)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TerminalErrorEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogError(notification.Exception, "Erro no terminal {TerminalId}: {ErrorMessage}",
            notification.TerminalId, notification.ErrorMessage);
            
        _logService.Registrar($"ERRO: {notification.ErrorMessage}");
        
        return Task.CompletedTask;
    }
}

public class TerminalLogEventHandler : INotificationHandler<TerminalLogEvent>
{
    private readonly ITerminalLogService _logService;
    private readonly ILogger<TerminalLogEventHandler> _logger;

    public TerminalLogEventHandler(
        ITerminalLogService logService,
        ILogger<TerminalLogEventHandler> logger)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TerminalLogEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Terminal {TerminalId} - {Level}: {Message}",
            notification.TerminalId, notification.Level, notification.Message);
            
        _logService.Registrar($"[{notification.Level}] {notification.Message}");
        
        return Task.CompletedTask;
    }
}