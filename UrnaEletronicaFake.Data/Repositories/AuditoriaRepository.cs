using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Data.DbContext;
using UrnaEletronicaFake.Shared.Models;

namespace UrnaEletronicaFake.Data.Repositories;

public class AuditoriaRepository : Repository<Auditoria>, IAuditoriaRepository
{
    public AuditoriaRepository(UrnaDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Auditoria>> GetAuditoriasPorUsuarioAsync(string usuarioId)
    {
        return await _dbSet
            .Where(a => a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auditoria>> GetAuditoriasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _dbSet
            .Where(a => a.DataHora >= dataInicio && a.DataHora <= dataFim)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auditoria>> GetAuditoriasPorTerminalAsync(string terminalId)
    {
        return await _dbSet
            .Where(a => a.TerminalId == terminalId)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auditoria>> GetAuditoriasPorAcaoAsync(string acao)
    {
        return await _dbSet
            .Where(a => a.Acao == acao)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auditoria>> GetAuditoriasComFalhaAsync()
    {
        return await _dbSet
            .Where(a => !a.Sucesso)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auditoria>> GetAuditoriasPorSessaoAsync(string sessaoId)
    {
        return await _dbSet
            .Where(a => a.SessaoId == sessaoId)
            .OrderBy(a => a.DataHora)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetEstatisticasAuditoriaAsync()
    {
        var stats = await _dbSet
            .GroupBy(a => a.Acao)
            .Select(g => new { Acao = g.Key, Total = g.Count() })
            .ToListAsync();

        return stats.ToDictionary(s => s.Acao, s => s.Total);
    }

    public async Task<int> GetTotalAuditoriasAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<int> GetTotalAuditoriasPorUsuarioAsync(string usuarioId)
    {
        return await _dbSet.CountAsync(a => a.UsuarioId == usuarioId);
    }

    public async Task<int> GetTotalAuditoriasComFalhaAsync()
    {
        return await _dbSet.CountAsync(a => !a.Sucesso);
    }
}

