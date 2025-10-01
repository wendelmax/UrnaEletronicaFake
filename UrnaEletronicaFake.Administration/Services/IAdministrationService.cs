using UrnaEletronicaFake.Shared.Models;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Administration.Services;

public interface IAdministrationService
{
    Task<bool> CreateElectionAsync(Eleicao eleicao);
    Task<bool> UpdateElectionAsync(Eleicao eleicao);
    Task<bool> DeleteElectionAsync(int eleicaoId);
    Task<Eleicao?> GetElectionAsync(int eleicaoId);
    Task<IEnumerable<Eleicao>> GetAllElectionsAsync();
    Task<IEnumerable<Eleicao>> GetActiveElectionsAsync();
    
    Task<bool> CreateCandidateAsync(Candidato candidato);
    Task<bool> UpdateCandidateAsync(Candidato candidato);
    Task<bool> DeleteCandidateAsync(int candidatoId);
    Task<Candidato?> GetCandidateAsync(int candidatoId);
    Task<IEnumerable<Candidato>> GetAllCandidatesAsync();
    Task<IEnumerable<Candidato>> GetCandidatesByElectionAsync(int eleicaoId);
    
    Task<bool> CreateElectoralPositionAsync(CargoEleitoral cargo);
    Task<bool> UpdateElectoralPositionAsync(CargoEleitoral cargo);
    Task<bool> DeleteElectoralPositionAsync(int cargoId);
    Task<CargoEleitoral?> GetElectoralPositionAsync(int cargoId);
    Task<IEnumerable<CargoEleitoral>> GetAllElectoralPositionsAsync();
    
    Task<bool> ValidateSystemConfigurationAsync();
    Task<Dictionary<string, object>> GetSystemStatusAsync();
    Task<bool> InitializeSystemAsync();
    Task<bool> ResetSystemAsync();
}

