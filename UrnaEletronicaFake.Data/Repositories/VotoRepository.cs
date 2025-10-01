using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Data.DbContext;
using UrnaEletronicaFake.Shared.Models;

namespace UrnaEletronicaFake.Data.Repositories;

public class VotoRepository : Repository<Voto>, IVotoRepository
{
    public VotoRepository(UrnaDbContext context) : base(context)
    {
    }

    public async Task<bool> EleitorJaVotouAsync(string eleitorId, string cargo)
    {
        return await _dbSet.AnyAsync(v => v.EleitorId == eleitorId && v.Cargo == cargo);
    }

    public async Task<IEnumerable<Voto>> GetVotosPorEleitorAsync(string eleitorId)
    {
        return await _dbSet
            .Where(v => v.EleitorId == eleitorId)
            .OrderBy(v => v.DataVoto)
            .ToListAsync();
    }

    public async Task<IEnumerable<Voto>> GetVotosPorCandidatoAsync(string candidatoNumero)
    {
        return await _dbSet
            .Where(v => v.CandidatoNumero == candidatoNumero)
            .OrderBy(v => v.DataVoto)
            .ToListAsync();
    }

    public async Task<IEnumerable<Voto>> GetVotosPorCargoAsync(string cargo)
    {
        return await _dbSet
            .Where(v => v.Cargo == cargo)
            .OrderBy(v => v.DataVoto)
            .ToListAsync();
    }

    public async Task<IEnumerable<Voto>> GetVotosPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _dbSet
            .Where(v => v.DataVoto >= dataInicio && v.DataVoto <= dataFim)
            .OrderBy(v => v.DataVoto)
            .ToListAsync();
    }

    public async Task<IEnumerable<Voto>> GetVotosPorTerminalAsync(string terminalId)
    {
        return await _dbSet
            .Where(v => v.TerminalId == terminalId)
            .OrderBy(v => v.DataVoto)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetEstatisticasVotacaoAsync()
    {
        var stats = await _dbSet
            .GroupBy(v => v.Cargo)
            .Select(g => new { Cargo = g.Key, Total = g.Count() })
            .ToListAsync();

        return stats.ToDictionary(s => s.Cargo, s => s.Total);
    }

    public async Task<int> GetTotalVotosAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<int> GetTotalVotosPorCargoAsync(string cargo)
    {
        return await _dbSet.CountAsync(v => v.Cargo == cargo);
    }

    public async Task<int> GetTotalVotosPorCandidatoAsync(string candidatoNumero)
    {
        return await _dbSet.CountAsync(v => v.CandidatoNumero == candidatoNumero);
    }

    public async Task<bool> ExisteVotoAsync(string eleitorId, string cargo)
    {
        return await _dbSet.AnyAsync(v => v.EleitorId == eleitorId && v.Cargo == cargo);
    }
}

