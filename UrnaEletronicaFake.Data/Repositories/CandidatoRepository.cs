using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Data.DbContext;
using UrnaEletronicaFake.Shared.Models;

namespace UrnaEletronicaFake.Data.Repositories;

public class CandidatoRepository : Repository<Candidato>, ICandidatoRepository
{
    public CandidatoRepository(UrnaDbContext context) : base(context)
    {
    }

    public async Task<Candidato?> GetByNumeroAsync(string numero)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Numero == numero);
    }

    public async Task<IEnumerable<Candidato>> GetCandidatosPorCargoAsync(string cargo)
    {
        return await _dbSet
            .Where(c => c.Cargo == cargo && c.Ativo)
            .OrderBy(c => c.Numero)
            .ToListAsync();
    }

    public async Task<IEnumerable<Candidato>> GetCandidatosAtivosAsync()
    {
        return await _dbSet
            .Where(c => c.Ativo)
            .OrderBy(c => c.Cargo)
            .ThenBy(c => c.Numero)
            .ToListAsync();
    }

    public async Task<IEnumerable<Candidato>> GetCandidatosPorPartidoAsync(string partido)
    {
        return await _dbSet
            .Where(c => c.Partido == partido && c.Ativo)
            .OrderBy(c => c.Cargo)
            .ThenBy(c => c.Numero)
            .ToListAsync();
    }

    public async Task<Candidato?> GetCandidatoPorNumeroECargoAsync(string numero, string cargo)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Numero == numero && c.Cargo == cargo);
    }

    public async Task<bool> ExisteCandidatoAsync(string numero, string cargo)
    {
        return await _dbSet.AnyAsync(c => c.Numero == numero && c.Cargo == cargo);
    }

    public async Task<Dictionary<string, int>> GetEstatisticasCandidatosAsync()
    {
        var stats = await _dbSet
            .GroupBy(c => c.Cargo)
            .Select(g => new { Cargo = g.Key, Total = g.Count() })
            .ToListAsync();

        return stats.ToDictionary(s => s.Cargo, s => s.Total);
    }

    public async Task<int> GetTotalCandidatosAsync()
    {
        return await _dbSet.CountAsync(c => c.Ativo);
    }

    public async Task<int> GetTotalCandidatosPorCargoAsync(string cargo)
    {
        return await _dbSet.CountAsync(c => c.Cargo == cargo && c.Ativo);
    }

    public async Task<int> GetTotalCandidatosPorPartidoAsync(string partido)
    {
        return await _dbSet.CountAsync(c => c.Partido == partido && c.Ativo);
    }
}

