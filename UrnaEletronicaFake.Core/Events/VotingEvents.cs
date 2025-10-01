using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Shared.DTOs;
using MediatR;

namespace UrnaEletronicaFake.Core.Events;

public class VotingSessionStartedEvent
{
    public string EleitorId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
    public string? SessionId { get; set; }
}

public class VotingSessionEndedEvent
{
    public string EleitorId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; } = DateTime.Now;
    public VotingStatus FinalStatus { get; set; }
    public string? TerminalId { get; set; }
    public string? SessionId { get; set; }
}

public class VoteValidationRequestedEvent
{
    public VotingRequest VoteRequest { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
}

public class VoteValidationCompletedEvent
{
    public VotingRequest VoteRequest { get; set; } = new();
    public bool IsValid { get; set; }
    public string? ValidationMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class VoteStartedEvent : INotification
{
    public string EleitorId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
    public string? SessionId { get; set; }
}

public class VoteAbortedEvent : INotification
{
    public string EleitorId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
    public string? SessionId { get; set; }
}

public class TerminalErrorEvent : INotification
{
    public string ErrorMessage { get; set; } = string.Empty;
    public string? TerminalId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? StackTrace { get; set; }
}
