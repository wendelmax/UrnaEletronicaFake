using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrnaEletronicaFake.Models;

public class Candidato
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Partido { get; set; } = string.Empty;
    
    [Required]
    public string Numero { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Foto { get; set; }
    
    [StringLength(500)]
    public string? Biografia { get; set; }
    
    [StringLength(200)]
    public string? FotoUrl { get; set; }
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    public int EleicaoId { get; set; }
    public Eleicao Eleicao { get; set; } = null!;
    
    public int CargoEleitoralId { get; set; }
    public CargoEleitoral CargoEleitoral { get; set; } = null!;
    
    public bool Ativo { get; set; } = true;
    
    public ICollection<Voto> Votos { get; set; } = new List<Voto>();
} 