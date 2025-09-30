using UrnaEletronicaFake.Modules.Core.Services;

namespace UrnaEletronicaFake.Modules.Core.Events;

public record TerminalUnlockedEvent(string EleitorId, DateTime Timestamp) : INotification;

public record TerminalLockedEvent(string? EleitorId, DateTime Timestamp) : INotification;

public record TerminalStateRequestEvent(DateTime Timestamp) : INotification;

public record VoteCompletedEvent(string EleitorId, string VoteId, DateTime Timestamp) : INotification;

public record VoteCancelledEvent(string EleitorId, string Reason, DateTime Timestamp) : INotification;

public record MesaActivityEvent(string Activity, string Details, DateTime Timestamp) : INotification;