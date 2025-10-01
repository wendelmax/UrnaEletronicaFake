using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Core.Interfaces;
using UrnaEletronicaFake.Core.Events;
using UrnaEletronicaFake.Voting.Services;

namespace UrnaEletronicaFake.Voting.Events;

public class VotingEventHandlers : 
    INotificationHandler<VotingSessionStartedEvent>,
    INotificationHandler<VotingSessionEndedEvent>,
    INotificationHandler<VoteValidationRequestedEvent>,
    INotificationHandler<VoteValidationCompletedEvent>
{
    private readonly ILogger<VotingEventHandlers> _logger;
    private readonly IVotingService _votingService;

    public VotingEventHandlers(
        ILogger<VotingEventHandlers> logger,
        IVotingService votingService)
    {
        _logger = logger;
        _votingService = votingService;
    }

    public async Task Handle(VotingSessionStartedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Voting session started for eleitor {EleitorId} on terminal {TerminalId} at {StartTime}", 
            notification.EleitorId, notification.TerminalId ?? "default", notification.StartTime);
        
        await Task.CompletedTask;
    }

    public async Task Handle(VotingSessionEndedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Voting session ended for eleitor {EleitorId} on terminal {TerminalId}. " +
            "Duration: {Duration}ms, Final Status: {Status}", 
            notification.EleitorId, 
            notification.TerminalId ?? "default", 
            (notification.EndTime - notification.StartTime).TotalMilliseconds,
            notification.FinalStatus);
        
        await Task.CompletedTask;
    }

    public async Task Handle(VoteValidationRequestedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Vote validation requested for eleitor {EleitorId}, candidato {CandidatoNumero}, cargo {Cargo}", 
            notification.VoteRequest.EleitorId, 
            notification.VoteRequest.CandidateNumber, 
            notification.VoteRequest.Cargo);

        try
        {
            var isValidEleitor = await _votingService.ValidateEleitorAsync(notification.VoteRequest.EleitorId);
            var isValidCandidato = await _votingService.ValidateCandidatoAsync(
                notification.VoteRequest.CandidateNumber, 
                notification.VoteRequest.Cargo);
            var canVote = await _votingService.CanVoteAsync(
                notification.VoteRequest.EleitorId, 
                notification.VoteRequest.Cargo);

            var isValid = isValidEleitor && isValidCandidato && canVote;
            var validationMessage = isValid ? "Voto válido" : GetValidationErrorMessage(isValidEleitor, isValidCandidato, canVote);

            _logger.LogInformation("Vote validation completed for eleitor {EleitorId}. Valid: {IsValid}, Message: {Message}", 
                notification.VoteRequest.EleitorId, isValid, validationMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during vote validation for eleitor {EleitorId}", 
                notification.VoteRequest.EleitorId);
        }

        await Task.CompletedTask;
    }

    public async Task Handle(VoteValidationCompletedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Vote validation completed for eleitor {EleitorId}. Valid: {IsValid}, Message: {Message}", 
            notification.VoteRequest.EleitorId, 
            notification.IsValid, 
            notification.ValidationMessage);
        
        await Task.CompletedTask;
    }

    private static string GetValidationErrorMessage(bool isValidEleitor, bool isValidCandidato, bool canVote)
    {
        if (!isValidEleitor) return "Eleitor inválido";
        if (!isValidCandidato) return "Candidato inválido";
        if (!canVote) return "Eleitor já votou para este cargo";
        return "Voto inválido";
    }
}

