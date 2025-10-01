using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Core.Interfaces;
using UrnaEletronicaFake.Core.Events;
using UrnaEletronicaFake.Data.Repositories;
using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Shared.Constants;
using UrnaEletronicaFake.Shared.Models;

namespace UrnaEletronicaFake.Voting.Services;

public class VotingService : IVotingService
{
    private readonly ILogger<VotingService> _logger;
    private readonly IEventBus _eventBus;
    private readonly IVotoRepository _votoRepository;
    private readonly ICandidatoRepository _candidatoRepository;
    private readonly Dictionary<string, VotingSession> _activeSessions = new();

    public VotingService(
        ILogger<VotingService> logger,
        IEventBus eventBus,
        IVotoRepository votoRepository,
        ICandidatoRepository candidatoRepository)
    {
        _logger = logger;
        _eventBus = eventBus;
        _votoRepository = votoRepository;
        _candidatoRepository = candidatoRepository;
    }

    public async Task<VotingResult> ProcessVoteAsync(VotingRequest request)
    {
        try
        {
            _logger.LogInformation("Processing vote for eleitor {EleitorId}, candidato {CandidatoNumero}, cargo {Cargo}", 
                request.EleitorId, request.CandidatoNumero, request.Cargo);

            // Validar eleitor
            if (!await ValidateEleitorAsync(request.EleitorId))
            {
                return new VotingResult
                {
                    Success = false,
                    Message = "Eleitor inválido",
                    Status = VotingStatus.Error,
                    ErrorCode = "INVALID_ELEITOR"
                };
            }

            // Verificar se já votou
            if (!await CanVoteAsync(request.EleitorId, request.Cargo))
            {
                return new VotingResult
                {
                    Success = false,
                    Message = "Eleitor já votou para este cargo",
                    Status = VotingStatus.Error,
                    ErrorCode = "ALREADY_VOTED"
                };
            }

            // Validar candidato
            if (!await ValidateCandidatoAsync(request.CandidatoNumero, request.Cargo))
            {
                return new VotingResult
                {
                    Success = false,
                    Message = "Candidato inválido",
                    Status = VotingStatus.Error,
                    ErrorCode = "INVALID_CANDIDATE"
                };
            }

            // Criar voto
            var voto = new Voto
            {
                EleitorId = request.EleitorId,
                CandidatoNumero = request.CandidatoNumero,
                Cargo = request.Cargo,
                DataVoto = request.Timestamp,
                TerminalId = request.TerminalId ?? string.Empty,
                SessionId = request.SessionId ?? string.Empty,
                Status = VotingStatus.Completed.ToString()
            };

            // Salvar voto
            await _votoRepository.AddAsync(voto);

            // Atualizar sessão ativa
            if (_activeSessions.ContainsKey(request.EleitorId))
            {
                _activeSessions[request.EleitorId].LastActivity = DateTime.Now;
                _activeSessions[request.EleitorId].VotesCount++;
            }

            var result = new VotingResult
            {
                Success = true,
                Message = "Voto registrado com sucesso",
                Status = VotingStatus.Completed,
                VoteId = voto.Id.ToString()
            };

            // Publicar evento
            await _eventBus.PublishAsync(new VoteCompletedEvent
            {
                VoteRequest = request,
                VoteResult = result
            });

            _logger.LogInformation("Vote processed successfully for eleitor {EleitorId}", request.EleitorId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing vote for eleitor {EleitorId}", request.EleitorId);
            
            var result = new VotingResult
            {
                Success = false,
                Message = "Erro interno do sistema",
                Status = VotingStatus.Error,
                ErrorCode = "INTERNAL_ERROR"
            };

            await _eventBus.PublishAsync(new VoteCancelledEvent
            {
                EleitorId = request.EleitorId,
                Reason = "System error",
                TerminalId = request.TerminalId
            });

            return result;
        }
    }

    public Task<bool> ValidateEleitorAsync(string eleitorId)
    {
        if (string.IsNullOrWhiteSpace(eleitorId))
            return Task.FromResult(false);

        if (eleitorId.Length < VotingConstants.MIN_ELEITOR_ID_LENGTH || 
            eleitorId.Length > VotingConstants.MAX_ELEITOR_ID_LENGTH)
            return Task.FromResult(false);

        return Task.FromResult(eleitorId.All(char.IsDigit));
    }

    public async Task<bool> ValidateCandidatoAsync(string candidatoNumero, string cargo)
    {
        if (string.IsNullOrWhiteSpace(candidatoNumero) || string.IsNullOrWhiteSpace(cargo))
            return false;

        // Permitir votos brancos e nulos
        if (candidatoNumero == VotingConstants.BLANK_VOTE_CODE || 
            candidatoNumero == VotingConstants.NULL_VOTE_CODE)
            return true;

        return await _candidatoRepository.ExisteCandidatoAsync(candidatoNumero, cargo);
    }

    public async Task<bool> CanVoteAsync(string eleitorId, string cargo)
    {
        return !await _votoRepository.EleitorJaVotouAsync(eleitorId, cargo);
    }

    public async Task<VotingResult> CancelVoteAsync(string eleitorId, string reason)
    {
        try
        {
            if (_activeSessions.ContainsKey(eleitorId))
            {
                _activeSessions.Remove(eleitorId);
            }

            await _eventBus.PublishAsync(new VoteCancelledEvent
            {
                EleitorId = eleitorId,
                Reason = reason
            });

            return new VotingResult
            {
                Success = true,
                Message = "Voto cancelado com sucesso",
                Status = VotingStatus.Cancelled
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling vote for eleitor {EleitorId}", eleitorId);
            return new VotingResult
            {
                Success = false,
                Message = "Erro ao cancelar voto",
                Status = VotingStatus.Error,
                ErrorCode = "CANCEL_ERROR"
            };
        }
    }

    public async Task<IEnumerable<VotingRequest>> GetVotingHistoryAsync(string eleitorId)
    {
        var votos = await _votoRepository.GetVotosPorEleitorAsync(eleitorId);
        
        return votos.Select(v => new VotingRequest
        {
            EleitorId = v.EleitorId,
            CandidateNumber = v.CandidatoNumero,
            Cargo = v.Cargo,
            Timestamp = v.DataVoto,
            TerminalId = v.TerminalId,
            SessionId = v.SessionId
        });
    }

    public async Task<Dictionary<string, int>> GetVotingStatisticsAsync()
    {
        return await _votoRepository.GetEstatisticasVotacaoAsync();
    }

    public Task<bool> IsVotingSessionActiveAsync(string eleitorId)
    {
        if (!_activeSessions.TryGetValue(eleitorId, out var session))
            return Task.FromResult(false);

        var timeout = TimeSpan.FromMinutes(VotingConstants.SESSION_TIMEOUT_MINUTES);
        return Task.FromResult(DateTime.Now - session.LastActivity < timeout);
    }

    private class VotingSession
    {
        public string EleitorId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime LastActivity { get; set; } = DateTime.Now;
        public int VotesCount { get; set; } = 0;
        public string? TerminalId { get; set; }
        public string? SessionId { get; set; }
    }
}
