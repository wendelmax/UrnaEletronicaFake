using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Data.Repositories;
using UrnaEletronicaFake.Shared.Models;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Administration.Services;

public class AdministrationService : IAdministrationService
{
    private readonly ILogger<AdministrationService> _logger;
    private readonly IRepository<Eleicao> _eleicaoRepository;
    private readonly IRepository<CargoEleitoral> _cargoRepository;
    private readonly ICandidatoRepository _candidatoRepository;

    public AdministrationService(
        ILogger<AdministrationService> logger,
        IRepository<Eleicao> eleicaoRepository,
        IRepository<CargoEleitoral> cargoRepository,
        ICandidatoRepository candidatoRepository)
    {
        _logger = logger;
        _eleicaoRepository = eleicaoRepository;
        _cargoRepository = cargoRepository;
        _candidatoRepository = candidatoRepository;
    }

    public async Task<bool> CreateElectionAsync(Eleicao eleicao)
    {
        try
        {
            if (eleicao == null)
            {
                _logger.LogWarning("Attempted to create null election");
                return false;
            }

            await _eleicaoRepository.AddAsync(eleicao);
            _logger.LogInformation("Election created successfully: {ElectionName}", eleicao.Nome);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating election: {ElectionName}", eleicao?.Nome);
            return false;
        }
    }

    public async Task<bool> UpdateElectionAsync(Eleicao eleicao)
    {
        try
        {
            if (eleicao == null)
            {
                _logger.LogWarning("Attempted to update null election");
                return false;
            }

            await _eleicaoRepository.UpdateAsync(eleicao);
            _logger.LogInformation("Election updated successfully: {ElectionName}", eleicao.Nome);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating election: {ElectionName}", eleicao?.Nome);
            return false;
        }
    }

    public async Task<bool> DeleteElectionAsync(int eleicaoId)
    {
        try
        {
            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                _logger.LogWarning("Election not found for deletion: {ElectionId}", eleicaoId);
                return false;
            }

            await _eleicaoRepository.DeleteAsync(eleicao);
            _logger.LogInformation("Election deleted successfully: {ElectionName}", eleicao.Nome);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting election: {ElectionId}", eleicaoId);
            return false;
        }
    }

    public async Task<Eleicao?> GetElectionAsync(int eleicaoId)
    {
        try
        {
            return await _eleicaoRepository.GetByIdAsync(eleicaoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting election: {ElectionId}", eleicaoId);
            return null;
        }
    }

    public async Task<IEnumerable<Eleicao>> GetAllElectionsAsync()
    {
        try
        {
            return await _eleicaoRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all elections");
            return new List<Eleicao>();
        }
    }

    public async Task<IEnumerable<Eleicao>> GetActiveElectionsAsync()
    {
        try
        {
            return await _eleicaoRepository.FindAsync(e => e.Ativa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active elections");
            return new List<Eleicao>();
        }
    }

    public async Task<bool> CreateCandidateAsync(Candidato candidato)
    {
        try
        {
            if (candidato == null)
            {
                _logger.LogWarning("Attempted to create null candidate");
                return false;
            }

            await _candidatoRepository.AddAsync(candidato);
            _logger.LogInformation("Candidate created successfully: {CandidateName} - {CandidateNumber}", 
                candidato.Nome, candidato.Numero);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating candidate: {CandidateName}", candidato?.Nome);
            return false;
        }
    }

    public async Task<bool> UpdateCandidateAsync(Candidato candidato)
    {
        try
        {
            if (candidato == null)
            {
                _logger.LogWarning("Attempted to update null candidate");
                return false;
            }

            await _candidatoRepository.UpdateAsync(candidato);
            _logger.LogInformation("Candidate updated successfully: {CandidateName} - {CandidateNumber}", 
                candidato.Nome, candidato.Numero);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating candidate: {CandidateName}", candidato?.Nome);
            return false;
        }
    }

    public async Task<bool> DeleteCandidateAsync(int candidatoId)
    {
        try
        {
            var candidato = await _candidatoRepository.GetByIdAsync(candidatoId);
            if (candidato == null)
            {
                _logger.LogWarning("Candidate not found for deletion: {CandidateId}", candidatoId);
                return false;
            }

            await _candidatoRepository.DeleteAsync(candidato);
            _logger.LogInformation("Candidate deleted successfully: {CandidateName}", candidato.Nome);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting candidate: {CandidateId}", candidatoId);
            return false;
        }
    }

    public async Task<Candidato?> GetCandidateAsync(int candidatoId)
    {
        try
        {
            return await _candidatoRepository.GetByIdAsync(candidatoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting candidate: {CandidateId}", candidatoId);
            return null;
        }
    }

    public async Task<IEnumerable<Candidato>> GetAllCandidatesAsync()
    {
        try
        {
            return await _candidatoRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all candidates");
            return new List<Candidato>();
        }
    }

    public async Task<IEnumerable<Candidato>> GetCandidatesByElectionAsync(int eleicaoId)
    {
        try
        {
            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                _logger.LogWarning("Election not found: {ElectionId}", eleicaoId);
                return new List<Candidato>();
            }

            return await _candidatoRepository.GetCandidatosAtivosAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting candidates for election: {ElectionId}", eleicaoId);
            return new List<Candidato>();
        }
    }

    public async Task<bool> CreateElectoralPositionAsync(CargoEleitoral cargo)
    {
        try
        {
            if (cargo == null)
            {
                _logger.LogWarning("Attempted to create null electoral position");
                return false;
            }

            await _cargoRepository.AddAsync(cargo);
            _logger.LogInformation("Electoral position created successfully: {PositionName}", cargo.Nome);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating electoral position: {PositionName}", cargo?.Nome);
            return false;
        }
    }

    public async Task<bool> UpdateElectoralPositionAsync(CargoEleitoral cargo)
    {
        try
        {
            if (cargo == null)
            {
                _logger.LogWarning("Attempted to update null electoral position");
                return false;
            }

            await _cargoRepository.UpdateAsync(cargo);
            _logger.LogInformation("Electoral position updated successfully: {PositionName}", cargo.Nome);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating electoral position: {PositionName}", cargo?.Nome);
            return false;
        }
    }

    public async Task<bool> DeleteElectoralPositionAsync(int cargoId)
    {
        try
        {
            var cargo = await _cargoRepository.GetByIdAsync(cargoId);
            if (cargo == null)
            {
                _logger.LogWarning("Electoral position not found for deletion: {PositionId}", cargoId);
                return false;
            }

            await _cargoRepository.DeleteAsync(cargo);
            _logger.LogInformation("Electoral position deleted successfully: {PositionName}", cargo.Nome);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting electoral position: {PositionId}", cargoId);
            return false;
        }
    }

    public async Task<CargoEleitoral?> GetElectoralPositionAsync(int cargoId)
    {
        try
        {
            return await _cargoRepository.GetByIdAsync(cargoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting electoral position: {PositionId}", cargoId);
            return null;
        }
    }

    public async Task<IEnumerable<CargoEleitoral>> GetAllElectoralPositionsAsync()
    {
        try
        {
            return await _cargoRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all electoral positions");
            return new List<CargoEleitoral>();
        }
    }

    public async Task<bool> ValidateSystemConfigurationAsync()
    {
        try
        {
            var elections = await _eleicaoRepository.GetAllAsync();
            var candidates = await _candidatoRepository.GetAllAsync();
            var positions = await _cargoRepository.GetAllAsync();

            var isValid = elections.Any() && candidates.Any() && positions.Any();
            
            _logger.LogInformation("System configuration validation completed. Valid: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating system configuration");
            return false;
        }
    }

    public async Task<Dictionary<string, object>> GetSystemStatusAsync()
    {
        try
        {
            var elections = await _eleicaoRepository.GetAllAsync();
            var candidates = await _candidatoRepository.GetAllAsync();
            var positions = await _cargoRepository.GetAllAsync();

            return new Dictionary<string, object>
            {
                ["TotalElections"] = elections.Count(),
                ["ActiveElections"] = elections.Count(e => e.Ativa),
                ["TotalCandidates"] = candidates.Count(),
                ["ActiveCandidates"] = candidates.Count(c => c.Ativo),
                ["TotalPositions"] = positions.Count(),
                ["ActivePositions"] = positions.Count(p => p.Ativo),
                ["SystemConfigured"] = elections.Any() && candidates.Any() && positions.Any(),
                ["LastUpdate"] = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system status");
            return new Dictionary<string, object>();
        }
    }

    public async Task<bool> InitializeSystemAsync()
    {
        try
        {
            _logger.LogInformation("Initializing system...");
            
            var isValid = await ValidateSystemConfigurationAsync();
            if (!isValid)
            {
                _logger.LogWarning("System configuration is invalid");
                return false;
            }

            _logger.LogInformation("System initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing system");
            return false;
        }
    }

    public async Task<bool> ResetSystemAsync()
    {
        try
        {
            _logger.LogWarning("Resetting system...");
            
            var elections = await _eleicaoRepository.GetAllAsync();
            var candidates = await _candidatoRepository.GetAllAsync();
            var positions = await _cargoRepository.GetAllAsync();

            foreach (var election in elections)
            {
                await _eleicaoRepository.DeleteAsync(election);
            }

            foreach (var candidate in candidates)
            {
                await _candidatoRepository.DeleteAsync(candidate);
            }

            foreach (var position in positions)
            {
                await _cargoRepository.DeleteAsync(position);
            }

            _logger.LogWarning("System reset completed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting system");
            return false;
        }
    }
}

