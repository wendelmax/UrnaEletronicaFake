using System.ComponentModel.DataAnnotations;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Shared.DTOs;

public class AuditEntry
{
    public int Id { get; set; }
    
    [Required]
    public AuditAction Action { get; set; }
    
    [Required]
    [StringLength(255)]
    public string UserId { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(45)]
    public string? IpAddress { get; set; }
    
    [StringLength(500)]
    public string? UserAgent { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    [StringLength(100)]
    public string? TerminalId { get; set; }
    
    [StringLength(50)]
    public string? SessionId { get; set; }
    
    public bool Success { get; set; }
    
    [StringLength(500)]
    public string? ErrorMessage { get; set; }
}

