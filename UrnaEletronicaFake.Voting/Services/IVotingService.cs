using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Voting.Services;

public interface IVotingService
{
    Task<VotingResult> ProcessVoteAsync(VotingRequest request);
    Task<bool> ValidateEleitorAsync(string eleitorId);
    Task<bool> ValidateCandidatoAsync(string candidatoNumero, string cargo);
    Task<bool> CanVoteAsync(string eleitorId, string cargo);
    Task<VotingResult> CancelVoteAsync(string eleitorId, string reason);
    Task<IEnumerable<VotingRequest>> GetVotingHistoryAsync(string eleitorId);
    Task<Dictionary<string, int>> GetVotingStatisticsAsync();
    Task<bool> IsVotingSessionActiveAsync(string eleitorId);
}

