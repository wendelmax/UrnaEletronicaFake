using UrnaEletronicaFake.Shared.Models;
using UrnaEletronicaFake.Shared.DTOs;

namespace UrnaEletronicaFake.Data.Repositories;

public interface IAuditoriaRepository : IRepository<Auditoria>
{
    Task<IEnumerable<Auditoria>> GetAuditoriasPorUsuarioAsync(string usuarioId);
    Task<IEnumerable<Auditoria>> GetAuditoriasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<Auditoria>> GetAuditoriasPorTerminalAsync(string terminalId);
    Task<IEnumerable<Auditoria>> GetAuditoriasPorAcaoAsync(string acao);
    Task<IEnumerable<Auditoria>> GetAuditoriasComFalhaAsync();
    Task<IEnumerable<Auditoria>> GetAuditoriasPorSessaoAsync(string sessaoId);
    Task<Dictionary<string, int>> GetEstatisticasAuditoriaAsync();
    Task<int> GetTotalAuditoriasAsync();
    Task<int> GetTotalAuditoriasPorUsuarioAsync(string usuarioId);
    Task<int> GetTotalAuditoriasComFalhaAsync();
}

