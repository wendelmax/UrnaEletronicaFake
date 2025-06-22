using System;
using System.ComponentModel.DataAnnotations;

namespace UrnaEletronicaFake.Models;

public class Voto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(11)]
    public string CpfEleitor { get; set; } = string.Empty;
    
    public DateTime DataVoto { get; set; } = DateTime.Now;
    
    [StringLength(50)]
    public string? SessaoId { get; set; }
    
    [StringLength(100)]
    public string? IpAddress { get; set; }
    
    [StringLength(200)]
    public string? UserAgent { get; set; }
    
    // Hash do voto para auditoria
    [Required]
    [StringLength(64)]
    public string HashVoto { get; set; } = string.Empty;
    
    // Relacionamentos
    public int EleicaoId { get; set; }
    public virtual Eleicao Eleicao { get; set; } = null!;
    
    public int? CandidatoId { get; set; }
    public virtual Candidato? Candidato { get; set; }
    
    // Voto nulo ou branco
    public bool VotoNulo { get; set; } = false;
    public bool VotoBranco { get; set; } = false;
} 