using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Shared.DTOs;

public class TerminalStatusResponse
{
    public TerminalState State { get; set; }
    public string? CurrentEleitorId { get; set; }
    public DateTime? LastActivity { get; set; }
    public string? SessionId { get; set; }
    public bool IsAvailable { get; set; }
    public string? ErrorMessage { get; set; }
}

