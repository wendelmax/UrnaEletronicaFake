using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrnaEletronicaFake.Models;

public class Eleicao
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Titulo { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500)]
    public string Descricao { get; set; } = string.Empty;
    
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public bool Ativa { get; set; } = false;
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    // Relacionamentos
    public virtual ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
    public virtual ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
} 