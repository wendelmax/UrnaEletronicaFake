using System.ComponentModel.DataAnnotations;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Shared.DTOs;

public class VotingRequest
{
    [Required]
    [StringLength(12, MinimumLength = 4)]
    public string EleitorId { get; set; } = string.Empty;
    
    [Required]
    public string CandidateNumber { get; set; } = string.Empty;
    
    [Required]
    public ElectionType ElectionType { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    public string? TerminalId { get; set; }
    
    public string? SessionId { get; set; }
    
    // Propriedades adicionais necessárias para os serviços
    public string CandidatoNumero { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
}
