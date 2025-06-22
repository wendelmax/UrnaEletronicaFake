using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrnaEletronicaFake.Models;

public class CargoEleitoral
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    public int QuantidadeDigitos { get; set; }
    
    public int EleicaoId { get; set; }
    public Eleicao Eleicao { get; set; } = null!;
    
    public int Ordem { get; set; }
    
    public bool Ativo { get; set; } = true;
    
    public ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
} 