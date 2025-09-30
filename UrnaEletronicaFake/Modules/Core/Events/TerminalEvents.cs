using System;
using MediatR;

namespace UrnaEletronicaFake.Modules.Core.Events;

public sealed record TerminalStartedEvent : INotification
{
    public DateTime StartTime { get; init; } = DateTime.Now;
    public string TerminalId { get; init; } = string.Empty;
}

public sealed record TerminalStoppedEvent : INotification
{
    public DateTime StopTime { get; init; } = DateTime.Now;
    public string TerminalId { get; init; } = string.Empty;
}

public sealed record TerminalStateChangedEvent : INotification
{
    public string TerminalId { get; init; } = string.Empty;
    public string PreviousState { get; init; } = string.Empty;
    public string CurrentState { get; init; } = string.Empty;
    public DateTime ChangedAt { get; init; } = DateTime.Now;
}

public sealed record TerminalErrorEvent : INotification
{
    public string TerminalId { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
    public Exception? Exception { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.Now;
}

public sealed record TerminalLogEvent : INotification
{
    public string TerminalId { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Level { get; init; } = "Info";
    public DateTime LoggedAt { get; init; } = DateTime.Now;
}