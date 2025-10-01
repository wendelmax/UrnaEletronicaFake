using System;
using System.ComponentModel.DataAnnotations;

namespace UrnaEletronicaFake.Shared.Models;

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
    
    // Propriedades adicionais necessárias para os repositories
    public string UsuarioId { get; set; } = string.Empty;
    public DateTime DataHora { get; set; } = DateTime.Now;
    public string TerminalId { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public string SessaoId { get; set; } = string.Empty;
    public bool Sucesso { get; set; } = true;
    public string MensagemErro { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
} 