using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Administration.Services;
using UrnaEletronicaFake.Data.Repositories;
using UrnaEletronicaFake.Shared.Models;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Administration.Managers;

public class ElectionManager : IElectionManager
{
    private readonly ILogger<ElectionManager> _logger;
    private readonly IAdministrationService _administrationService;
    private readonly IRepository<Eleicao> _eleicaoRepository;
    private readonly ICandidatoRepository _candidatoRepository;
    private readonly IVotoRepository _votoRepository;
    private readonly Dictionary<int, ElectionStatus> _electionStatuses = new();

    public ElectionManager(
        ILogger<ElectionManager> logger,
        IAdministrationService administrationService,
        IRepository<Eleicao> eleicaoRepository,
        ICandidatoRepository candidatoRepository,
        IVotoRepository votoRepository)
    {
        _logger = logger;
        _administrationService = administrationService;
        _eleicaoRepository = eleicaoRepository;
        _candidatoRepository = candidatoRepository;
        _votoRepository = votoRepository;
    }

    public async Task<bool> StartElectionAsync(int eleicaoId)
    {
        try
        {
            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                _logger.LogWarning("Election not found: {ElectionId}", eleicaoId);
                return false;
            }

            if (!await CanStartElectionAsync(eleicaoId))
            {
                _logger.LogWarning("Cannot start election: {ElectionId}", eleicaoId);
                return false;
            }

            eleicao.Ativa = true;
            await _eleicaoRepository.UpdateAsync(eleicao);
            
            _electionStatuses[eleicaoId] = ElectionStatus.InProgress;
            
            _logger.LogInformation("Election started successfully: {ElectionName} ({ElectionId})", 
                eleicao.Nome, eleicaoId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting election: {ElectionId}", eleicaoId);
            return false;
        }
    }

    public async Task<bool> StopElectionAsync(int eleicaoId)
    {
        try
        {
            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                _logger.LogWarning("Election not found: {ElectionId}", eleicaoId);
                return false;
            }

            if (!await CanStopElectionAsync(eleicaoId))
            {
                _logger.LogWarning("Cannot stop election: {ElectionId}", eleicaoId);
                return false;
            }

            eleicao.Ativa = false;
            await _eleicaoRepository.UpdateAsync(eleicao);
            
            _electionStatuses[eleicaoId] = ElectionStatus.Completed;
            
            _logger.LogInformation("Election stopped successfully: {ElectionName} ({ElectionId})", 
                eleicao.Nome, eleicaoId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping election: {ElectionId}", eleicaoId);
            return false;
        }
    }

    public async Task<bool> PauseElectionAsync(int eleicaoId)
    {
        try
        {
            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                _logger.LogWarning("Election not found: {ElectionId}", eleicaoId);
                return false;
            }

            if (_electionStatuses.GetValueOrDefault(eleicaoId) != ElectionStatus.InProgress)
            {
                _logger.LogWarning("Cannot pause election that is not in progress: {ElectionId}", eleicaoId);
                return false;
            }

            _electionStatuses[eleicaoId] = ElectionStatus.Paused;
            
            _logger.LogInformation("Election paused: {ElectionName} ({ElectionId})", 
                eleicao.Nome, eleicaoId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pausing election: {ElectionId}", eleicaoId);
            return false;
        }
    }

    public async Task<bool> ResumeElectionAsync(int eleicaoId)
    {
        try
        {
            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                _logger.LogWarning("Election not found: {ElectionId}", eleicaoId);
                return false;
            }

            if (_electionStatuses.GetValueOrDefault(eleicaoId) != ElectionStatus.Paused)
            {
                _logger.LogWarning("Cannot resume election that is not paused: {ElectionId}", eleicaoId);
                return false;
            }

            _electionStatuses[eleicaoId] = ElectionStatus.InProgress;
            
            _logger.LogInformation("Election resumed: {ElectionName} ({ElectionId})", 
                eleicao.Nome, eleicaoId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming election: {ElectionId}", eleicaoId);
            return false;
        }
    }

    public async Task<ElectionStatus> GetElectionStatusAsync(int eleicaoId)
    {
        try
        {
            if (_electionStatuses.TryGetValue(eleicaoId, out var status))
            {
                return status;
            }

            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                return ElectionStatus.Error;
            }

            if (eleicao.Ativa)
            {
                return ElectionStatus.InProgress;
            }

            if (eleicao.DataFim < DateTime.Now)
            {
                return ElectionStatus.Completed;
            }

            if (eleicao.DataInicio > DateTime.Now)
            {
                return ElectionStatus.Scheduled;
            }

            return ElectionStatus.NotStarted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting election status: {ElectionId}", eleicaoId);
            return ElectionStatus.Error;
        }
    }

    public async Task<bool> CanStartElectionAsync(int eleicaoId)
    {
        try
        {
            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                return false;
            }

            var currentStatus = await GetElectionStatusAsync(eleicaoId);
            if (currentStatus != ElectionStatus.NotStarted && currentStatus != ElectionStatus.Scheduled)
            {
                return false;
            }

            if (eleicao.DataInicio > DateTime.Now)
            {
                return false;
            }

            if (eleicao.DataFim < DateTime.Now)
            {
                return false;
            }

            return await ValidateElectionConfigurationAsync(eleicaoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if election can start: {ElectionId}", eleicaoId);
            return false;
        }
    }

    public async Task<bool> CanStopElectionAsync(int eleicaoId)
    {
        try
        {
            var currentStatus = await GetElectionStatusAsync(eleicaoId);
            return currentStatus == ElectionStatus.InProgress || currentStatus == ElectionStatus.Paused;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if election can stop: {ElectionId}", eleicaoId);
            return false;
        }
    }

    public async Task<Dictionary<string, object>> GetElectionStatisticsAsync(int eleicaoId)
    {
        try
        {
            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                return new Dictionary<string, object>();
            }

            var candidates = await _candidatoRepository.GetAllAsync();
            var totalVotes = await _votoRepository.GetTotalVotosAsync();
            var status = await GetElectionStatusAsync(eleicaoId);

            return new Dictionary<string, object>
            {
                ["ElectionId"] = eleicaoId,
                ["ElectionName"] = eleicao.Nome,
                ["Status"] = status.ToString(),
                ["TotalCandidates"] = candidates.Count(),
                ["TotalVotes"] = totalVotes,
                ["StartDate"] = eleicao.DataInicio,
                ["EndDate"] = eleicao.DataFim,
                ["IsActive"] = eleicao.Ativa,
                ["LastUpdate"] = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting election statistics: {ElectionId}", eleicaoId);
            return new Dictionary<string, object>();
        }
    }

    public async Task<bool> ValidateElectionConfigurationAsync(int eleicaoId)
    {
        try
        {
            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                return false;
            }

            var candidates = await _candidatoRepository.GetAllAsync();
            var positions = await _eleicaoRepository.GetAllAsync();

            if (!candidates.Any())
            {
                _logger.LogWarning("No candidates found for election: {ElectionId}", eleicaoId);
                return false;
            }

            if (!positions.Any())
            {
                _logger.LogWarning("No electoral positions found for election: {ElectionId}", eleicaoId);
                return false;
            }

            if (eleicao.DataInicio >= eleicao.DataFim)
            {
                _logger.LogWarning("Invalid election dates: {ElectionId}", eleicaoId);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating election configuration: {ElectionId}", eleicaoId);
            return false;
        }
    }

    public async Task<bool> ScheduleElectionAsync(int eleicaoId, DateTime startTime, DateTime endTime)
    {
        try
        {
            var eleicao = await _eleicaoRepository.GetByIdAsync(eleicaoId);
            if (eleicao == null)
            {
                _logger.LogWarning("Election not found: {ElectionId}", eleicaoId);
                return false;
            }

            if (startTime >= endTime)
            {
                _logger.LogWarning("Invalid schedule times for election: {ElectionId}", eleicaoId);
                return false;
            }

            eleicao.DataInicio = startTime;
            eleicao.DataFim = endTime;
            await _eleicaoRepository.UpdateAsync(eleicao);
            
            _electionStatuses[eleicaoId] = ElectionStatus.Scheduled;
            
            _logger.LogInformation("Election scheduled: {ElectionName} ({ElectionId}) from {StartTime} to {EndTime}", 
                eleicao.Nome, eleicaoId, startTime, endTime);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling election: {ElectionId}", eleicaoId);
            return false;
        }
    }
}

