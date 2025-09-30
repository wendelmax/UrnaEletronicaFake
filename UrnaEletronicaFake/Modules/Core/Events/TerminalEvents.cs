using MediatR;

namespace UrnaEletronicaFake.Modules.Core.Events;

public abstract class TerminalEvent : INotification
{
    public DateTime TimeStamp { get; } = DateTime.Now;
    public string TerminalId { get; init; } = Environment.MachineName;
}

public class TerminalUnlockedEvent : TerminalEvent
{
    public string EleitorId { get; init; } = "";
    public string MesarioId { get; init; } = "";
}

public class TerminalLockedEvent : TerminalEvent
{
    public string? PreviousEleitorId { get; init; }
    public string Reason { get; init; } = "";
}

public class VoteStartedEvent : TerminalEvent
{
    public string EleitorId { get; init; } = "";
}

public class VoteCompletedEvent : TerminalEvent
{
    public string EleitorId { get; init; } = "";
    public bool VoteConfirmed { get; init; }
}

public class VoteAbortedEvent : TerminalEvent
{
    public string EleitorId { get; init; } = "";
    public string Reason { get; init; } = "";
}

public class TerminalErrorEvent : TerminalEvent
{
    public string ErrorMessage { get; init; } = "";
    public string? ErrorDetails { get; init; }
    public string? EleitorId { get; init; }
}