using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Shared.DTOs;

public class VotingResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public VotingStatus Status { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? VoteId { get; set; }
    public string? ErrorCode { get; set; }
}

