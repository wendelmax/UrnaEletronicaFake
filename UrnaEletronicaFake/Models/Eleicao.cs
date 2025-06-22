using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrnaEletronicaFake.Models;

public class Eleicao
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Descricao { get; set; }
    
    public DateTime DataInicio { get; set; }
    
    public DateTime DataFim { get; set; }
    
    public bool Ativa { get; set; } = false;
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    // Relacionamentos
    public ICollection<CargoEleitoral> CargosEleitorais { get; set; } = new List<CargoEleitoral>();
    public ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
    public ICollection<Voto> Votos { get; set; } = new List<Voto>();
    public virtual ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
} 