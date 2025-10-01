using UrnaEletronicaFake.Shared.Models;

namespace UrnaEletronicaFake.Data.Repositories;

public interface ICandidatoRepository : IRepository<Candidato>
{
    Task<Candidato?> GetByNumeroAsync(string numero);
    Task<IEnumerable<Candidato>> GetCandidatosPorCargoAsync(string cargo);
    Task<IEnumerable<Candidato>> GetCandidatosAtivosAsync();
    Task<IEnumerable<Candidato>> GetCandidatosPorPartidoAsync(string partido);
    Task<Candidato?> GetCandidatoPorNumeroECargoAsync(string numero, string cargo);
    Task<bool> ExisteCandidatoAsync(string numero, string cargo);
    Task<Dictionary<string, int>> GetEstatisticasCandidatosAsync();
    Task<int> GetTotalCandidatosAsync();
    Task<int> GetTotalCandidatosPorCargoAsync(string cargo);
    Task<int> GetTotalCandidatosPorPartidoAsync(string partido);
}

