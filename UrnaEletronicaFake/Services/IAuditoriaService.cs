using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrnaEletronicaFake.Models;

namespace UrnaEletronicaFake.Services;

public interface IAuditoriaService
{
    Task<Auditoria> RegistrarAcaoAsync(string tipoAcao, string entidade, int? entidadeId, 
        string descricao, string? dadosAnteriores = null, string? dadosNovos = null);
    Task<IEnumerable<Auditoria>> ObterAuditoriasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
    Task<IEnumerable<Auditoria>> ObterAuditoriasPorEleicaoAsync(int eleicaoId);
    Task<IEnumerable<Auditoria>> ObterAuditoriasPorEntidadeAsync(string entidade, int? entidadeId = null);
} 