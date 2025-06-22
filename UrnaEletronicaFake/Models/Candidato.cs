using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrnaEletronicaFake.Models;

public class Candidato
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Partido { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string Numero { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Biografia { get; set; }
    
    [StringLength(200)]
    public string? FotoUrl { get; set; }
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public bool Ativo { get; set; } = true;
    
    // Relacionamentos
    public int EleicaoId { get; set; }
    public virtual Eleicao Eleicao { get; set; } = null!;
    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
} 