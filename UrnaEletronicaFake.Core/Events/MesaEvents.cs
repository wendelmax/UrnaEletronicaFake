using UrnaEletronicaFake.Shared.Enums;
using MediatR;

namespace UrnaEletronicaFake.Core.Events;

public class MesaActivityEvent : INotification
{
    public string Activity { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
    public bool Success { get; set; }
    public string? Details { get; set; }
}

public class TerminalStateRequestEvent
{
    public string RequestingUserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
}

public class MesaSecurityViolationEvent
{
    public string UserId { get; set; } = string.Empty;
    public string ViolationType { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
    public string? IpAddress { get; set; }
}

public class MesaSessionExpiredEvent
{
    public string UserId { get; set; } = string.Empty;
    public DateTime SessionStartTime { get; set; }
    public DateTime ExpirationTime { get; set; } = DateTime.Now;
    public string? TerminalId { get; set; }
}
