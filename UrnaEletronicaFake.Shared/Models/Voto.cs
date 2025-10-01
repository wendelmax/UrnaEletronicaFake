using System;
using System.ComponentModel.DataAnnotations;

namespace UrnaEletronicaFake.Shared.Models;

public class Voto
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string CpfEleitor { get; set; } = string.Empty;
    
    public int EleicaoId { get; set; }
    public Eleicao Eleicao { get; set; } = null!;
    
    public int CargoEleitoralId { get; set; }
    public CargoEleitoral CargoEleitoral { get; set; } = null!;
    
    public int? CandidatoId { get; set; }
    public Candidato? Candidato { get; set; }
    
    public bool VotoNulo { get; set; } = false;
    
    public bool VotoBranco { get; set; } = false;
    
    public DateTime DataVoto { get; set; } = DateTime.Now;
    
    [Required]
    [MaxLength(500)]
    public string HashVoto { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? SessaoId { get; set; }
    
    [StringLength(100)]
    public string? IpAddress { get; set; }
    
    [StringLength(200)]
    public string? UserAgent { get; set; }
    
    // Propriedades adicionais necessárias para os repositories
    public string EleitorId { get; set; } = string.Empty;
    public string CandidatoNumero { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string TerminalId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
} 