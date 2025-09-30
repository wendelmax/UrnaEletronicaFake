using UrnaEletronicaFake.Modules.Core.Events;
using UrnaEletronicaFake.Modules.Core.Services;
using UrnaEletronicaFake.Services;
using System.Threading;
using System.Threading.Tasks;

namespace UrnaEletronicaFake.Modules.Core.EventHandlers;

public class TerminalEventHandler : 
    INotificationHandler<TerminalUnlockedEvent>,
    INotificationHandler<TerminalLockedEvent>,
    INotificationHandler<VoteCompletedEvent>,
    INotificationHandler<VoteCancelledEvent>,
    INotificationHandler<MesaActivityEvent>
{
    private readonly ITerminalLogService _terminalLogService;
    private readonly IVotacaoStateService _votacaoStateService;

    public TerminalEventHandler(ITerminalLogService terminalLogService, IVotacaoStateService votacaoStateService)
    {
        _terminalLogService = terminalLogService;
        _votacaoStateService = votacaoStateService;
    }

    public Task Handle(TerminalUnlockedEvent notification, CancellationToken cancellationToken = default)
    {
        _terminalLogService.Registrar($"Terminal desbloqueado para eleitor: {notification.EleitorId}");
        return Task.CompletedTask;
    }

    public Task Handle(TerminalLockedEvent notification, CancellationToken cancellationToken = default)
    {
        var message = notification.EleitorId != null 
            ? $"Terminal bloqueado após votação do eleitor: {notification.EleitorId}"
            : "Terminal bloqueado";
            
        _terminalLogService.Registrar(message);
        _votacaoStateService.LockTerminal();
        return Task.CompletedTask;
    }

    public Task Handle(VoteCompletedEvent notification, CancellationToken cancellationToken = default)
    {
        _terminalLogService.Registrar($"Voto concluído - Eleitor: {notification.EleitorId}, ID: {notification.VoteId}");
        _votacaoStateService.LockTerminal();
        return Task.CompletedTask;
    }

    public Task Handle(VoteCancelledEvent notification, CancellationToken cancellationToken = default)
    {
        _terminalLogService.Registrar($"Voto cancelado - Eleitor: {notification.EleitorId}, Motivo: {notification.Reason}");
        _votacaoStateService.LockTerminal();
        return Task.CompletedTask;
    }

    public Task Handle(MesaActivityEvent notification, CancellationToken cancellationToken = default)
    {
        _terminalLogService.Registrar($"Mesa: {notification.Activity} - {notification.Details}");
        return Task.CompletedTask;
    }
}