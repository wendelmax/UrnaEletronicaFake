using UrnaEletronicaFake.Shared.Models;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Data.Repositories;

public interface IVotoRepository : IRepository<Voto>
{
    Task<bool> EleitorJaVotouAsync(string eleitorId, string cargo);
    Task<IEnumerable<Voto>> GetVotosPorEleitorAsync(string eleitorId);
    Task<IEnumerable<Voto>> GetVotosPorCandidatoAsync(string candidatoNumero);
    Task<IEnumerable<Voto>> GetVotosPorCargoAsync(string cargo);
    Task<IEnumerable<Voto>> GetVotosPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<Voto>> GetVotosPorTerminalAsync(string terminalId);
    Task<Dictionary<string, int>> GetEstatisticasVotacaoAsync();
    Task<int> GetTotalVotosAsync();
    Task<int> GetTotalVotosPorCargoAsync(string cargo);
    Task<int> GetTotalVotosPorCandidatoAsync(string candidatoNumero);
    Task<bool> ExisteVotoAsync(string eleitorId, string cargo);
}

