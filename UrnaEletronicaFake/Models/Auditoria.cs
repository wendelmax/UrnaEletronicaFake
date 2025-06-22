using System;
using System.ComponentModel.DataAnnotations;

namespace UrnaEletronicaFake.Models;

public class Auditoria
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string TipoAcao { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Entidade { get; set; } = string.Empty;
    
    public int? EntidadeId { get; set; }
    
    [StringLength(500)]
    public string? Descricao { get; set; }
    
    [StringLength(100)]
    public string? Usuario { get; set; }
    
    [StringLength(100)]
    public string? IpAddress { get; set; }
    
    public DateTime DataAcao { get; set; } = DateTime.Now;
    
    [StringLength(1000)]
    public string? DadosAnteriores { get; set; }
    
    [StringLength(1000)]
    public string? DadosNovos { get; set; }
    
    // Relacionamentos
    public int? EleicaoId { get; set; }
    public virtual Eleicao? Eleicao { get; set; }
} 