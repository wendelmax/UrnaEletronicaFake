using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Shared.DTOs;
using MediatR;

namespace UrnaEletronicaFake.Core.Events;

public class TerminalUnlockedEvent : INotification
{
    public string EleitorId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
    public string? SessionId { get; set; }
}

public class TerminalLockedEvent : INotification
{
    public string? EleitorId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
    public string? SessionId { get; set; }
    public string? Reason { get; set; }
}

public class VoteCompletedEvent : INotification
{
    public VotingRequest VoteRequest { get; set; } = new();
    public VotingResult VoteResult { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class VoteCancelledEvent : INotification
{
    public string EleitorId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? Reason { get; set; }
    public string? TerminalId { get; set; }
}

public class TerminalStateChangedEvent : INotification
{
    public TerminalState PreviousState { get; set; }
    public TerminalState NewState { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
    public string? Reason { get; set; }
}