using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Voting.Services;
using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Core.Interfaces;

namespace UrnaEletronicaFake.Voting.ViewModels;

public partial class VotingViewModel : ObservableObject
{
    private readonly IVotingService _votingService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<VotingViewModel> _logger;

    [ObservableProperty]
    private string _eleitorId = string.Empty;

    [ObservableProperty]
    private string _candidateNumber = string.Empty;

    [ObservableProperty]
    private ElectionType _selectedElectionType = ElectionType.Presidential;

    [ObservableProperty]
    private bool _isVotingEnabled = true;

    [ObservableProperty]
    private bool _isProcessingVote = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private VotingStatus _currentVotingStatus = VotingStatus.NotStarted;

    [ObservableProperty]
    private string _terminalId = "TERMINAL-001";

    [ObservableProperty]
    private string _sessionId = string.Empty;

    public VotingViewModel(
        IVotingService votingService,
        IEventBus eventBus,
        ILogger<VotingViewModel> logger)
    {
        _votingService = votingService;
        _eventBus = eventBus;
        _logger = logger;
        
        _sessionId = Guid.NewGuid().ToString();
        
    }


    [RelayCommand]
    private async Task VoteAsync()
    {
        try
        {
            IsProcessingVote = true;
            StatusMessage = "Processando voto...";
            CurrentVotingStatus = VotingStatus.InProgress;

            var request = new VotingRequest
            {
                EleitorId = EleitorId,
                CandidateNumber = CandidateNumber,
                ElectionType = SelectedElectionType,
                Timestamp = DateTime.Now,
                TerminalId = TerminalId,
                SessionId = SessionId
            };

            var result = await _votingService.ProcessVoteAsync(request);

            if (result.Success)
            {
                StatusMessage = result.Message;
                CurrentVotingStatus = result.Status;
                IsVotingEnabled = false;
                
                _logger.LogInformation("Vote processed successfully for eleitor {EleitorId}", EleitorId);
            }
            else
            {
                StatusMessage = result.Message;
                CurrentVotingStatus = result.Status;
                
                _logger.LogWarning("Vote processing failed for eleitor {EleitorId}: {Message}", 
                    EleitorId, result.Message);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro interno do sistema";
            CurrentVotingStatus = VotingStatus.Error;
            
            _logger.LogError(ex, "Error processing vote for eleitor {EleitorId}", EleitorId);
        }
        finally
        {
            IsProcessingVote = false;
        }
    }

    [RelayCommand]
    private async Task CancelVoteAsync()
    {
        try
        {
            IsProcessingVote = true;
            StatusMessage = "Cancelando voto...";

            var result = await _votingService.CancelVoteAsync(EleitorId, "Cancelado pelo usuário");

            if (result.Success)
            {
                StatusMessage = result.Message;
                CurrentVotingStatus = result.Status;
                ClearForm();
                
                _logger.LogInformation("Vote cancelled for eleitor {EleitorId}", EleitorId);
            }
            else
            {
                StatusMessage = result.Message;
                CurrentVotingStatus = result.Status;
                
                _logger.LogWarning("Vote cancellation failed for eleitor {EleitorId}: {Message}", 
                    EleitorId, result.Message);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro ao cancelar voto";
            CurrentVotingStatus = VotingStatus.Error;
            
            _logger.LogError(ex, "Error cancelling vote for eleitor {EleitorId}", EleitorId);
        }
        finally
        {
            IsProcessingVote = false;
        }
    }

    [RelayCommand]
    private void ClearForm()
    {
        EleitorId = string.Empty;
        CandidateNumber = string.Empty;
        SelectedElectionType = ElectionType.Presidential;
        StatusMessage = string.Empty;
        CurrentVotingStatus = VotingStatus.NotStarted;
        IsVotingEnabled = true;
        SessionId = Guid.NewGuid().ToString();
    }

    private bool CanVote()
    {
        return !string.IsNullOrWhiteSpace(EleitorId) && 
               !string.IsNullOrWhiteSpace(CandidateNumber) && 
               !IsProcessingVote && 
               IsVotingEnabled;
    }

    private bool CanCancelVote()
    {
        return !IsProcessingVote && 
               CurrentVotingStatus != VotingStatus.NotStarted && 
               CurrentVotingStatus != VotingStatus.Completed;
    }

    partial void OnEleitorIdChanged(string value)
    {
        VoteCommand.NotifyCanExecuteChanged();
    }

    partial void OnCandidateNumberChanged(string value)
    {
        VoteCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsProcessingVoteChanged(bool value)
    {
        VoteCommand.NotifyCanExecuteChanged();
        CancelVoteCommand.NotifyCanExecuteChanged();
    }

    partial void OnCurrentVotingStatusChanged(VotingStatus value)
    {
        CancelVoteCommand.NotifyCanExecuteChanged();
    }
}