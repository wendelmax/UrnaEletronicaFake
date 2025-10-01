using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Core.Interfaces;
using UrnaEletronicaFake.Core.Events;
using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Shared.DTOs;
using System.Collections.Concurrent;

namespace UrnaEletronicaFake.Core.Services;

public class TerminalStateService : ITerminalStateService
{
    private readonly ILogger<TerminalStateService> _logger;
    private readonly IEventBus _eventBus;
    private readonly ConcurrentDictionary<string, TerminalStateInfo> _terminalStates = new();

    public TerminalStateService(ILogger<TerminalStateService> logger, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
    }

    public Task<TerminalState> GetCurrentStateAsync(string? terminalId = null)
    {
        var key = terminalId ?? "default";
        if (_terminalStates.TryGetValue(key, out var stateInfo))
        {
            return Task.FromResult(stateInfo.State);
        }
        return Task.FromResult(TerminalState.Locked);
    }

    public async Task<bool> TryLockTerminalAsync(string eleitorId, string? terminalId = null)
    {
        var key = terminalId ?? "default";
        var currentState = await GetCurrentStateAsync(terminalId);
        
        if (currentState != TerminalState.Locked)
        {
            _logger.LogWarning("Cannot lock terminal {TerminalId}. Current state: {State}", terminalId, currentState);
            return false;
        }

        var stateInfo = new TerminalStateInfo
        {
            State = TerminalState.InUse,
            CurrentEleitorId = eleitorId,
            LastActivity = DateTime.Now,
            SessionId = Guid.NewGuid().ToString(),
            TerminalId = terminalId
        };

        _terminalStates.AddOrUpdate(key, stateInfo, (_, _) => stateInfo);

        _logger.LogInformation("Terminal {TerminalId} locked for eleitor {EleitorId}", terminalId, eleitorId);

        await _eventBus.PublishAsync(new TerminalUnlockedEvent
        {
            EleitorId = eleitorId,
            TerminalId = terminalId,
            SessionId = stateInfo.SessionId
        });

        return true;
    }

    public async Task<bool> UnlockTerminalAsync(string? terminalId = null)
    {
        var key = terminalId ?? "default";
        
        if (!_terminalStates.TryGetValue(key, out var stateInfo))
        {
            _logger.LogWarning("Terminal {TerminalId} not found or already unlocked", terminalId);
            return false;
        }

        var previousState = stateInfo.State;
        var eleitorId = stateInfo.CurrentEleitorId;

        stateInfo.State = TerminalState.Locked;
        stateInfo.CurrentEleitorId = null;
        stateInfo.LastActivity = DateTime.Now;

        _logger.LogInformation("Terminal {TerminalId} unlocked from eleitor {EleitorId}", terminalId, eleitorId);

        await _eventBus.PublishAsync(new TerminalLockedEvent
        {
            EleitorId = eleitorId,
            TerminalId = terminalId,
            SessionId = stateInfo.SessionId
        });

        await _eventBus.PublishAsync(new TerminalStateChangedEvent
        {
            PreviousState = previousState,
            NewState = TerminalState.Locked,
            TerminalId = terminalId,
            Reason = "Manual unlock"
        });

        return true;
    }

    public async Task<bool> SetTerminalStateAsync(TerminalState state, string? terminalId = null, string? reason = null)
    {
        var key = terminalId ?? "default";
        
        if (!_terminalStates.TryGetValue(key, out var stateInfo))
        {
            stateInfo = new TerminalStateInfo
            {
                TerminalId = terminalId,
                LastActivity = DateTime.Now
            };
        }

        var previousState = stateInfo.State;
        stateInfo.State = state;
        stateInfo.LastActivity = DateTime.Now;

        _terminalStates.AddOrUpdate(key, stateInfo, (_, _) => stateInfo);

        _logger.LogInformation("Terminal {TerminalId} state changed from {PreviousState} to {NewState}. Reason: {Reason}", 
            terminalId, previousState, state, reason ?? "Unknown");

        await _eventBus.PublishAsync(new TerminalStateChangedEvent
        {
            PreviousState = previousState,
            NewState = state,
            TerminalId = terminalId,
            Reason = reason ?? "State change"
        });

        return true;
    }

    public Task<TerminalStatusResponse> GetTerminalStatusAsync(string? terminalId = null)
    {
        var key = terminalId ?? "default";
        
        if (_terminalStates.TryGetValue(key, out var stateInfo))
        {
            return Task.FromResult(new TerminalStatusResponse
            {
                State = stateInfo.State,
                CurrentEleitorId = stateInfo.CurrentEleitorId,
                LastActivity = stateInfo.LastActivity,
                SessionId = stateInfo.SessionId,
                IsAvailable = stateInfo.State == TerminalState.Locked
            });
        }

        return Task.FromResult(new TerminalStatusResponse
        {
            State = TerminalState.Locked,
            IsAvailable = true
        });
    }

    public async Task<bool> IsTerminalAvailableAsync(string? terminalId = null)
    {
        var status = await GetTerminalStatusAsync(terminalId);
        return status.IsAvailable;
    }

    public Task<string?> GetCurrentEleitorAsync(string? terminalId = null)
    {
        var key = terminalId ?? "default";
        if (_terminalStates.TryGetValue(key, out var stateInfo))
        {
            return Task.FromResult(stateInfo.CurrentEleitorId);
        }
        return Task.FromResult<string?>(null);
    }

    public Task<DateTime?> GetLastActivityAsync(string? terminalId = null)
    {
        var key = terminalId ?? "default";
        if (_terminalStates.TryGetValue(key, out var stateInfo))
        {
            return Task.FromResult(stateInfo.LastActivity);
        }
        return Task.FromResult<DateTime?>(null);
    }

    private class TerminalStateInfo
    {
        public TerminalState State { get; set; } = TerminalState.Locked;
        public string? CurrentEleitorId { get; set; }
        public DateTime? LastActivity { get; set; } = DateTime.Now;
        public string? SessionId { get; set; }
        public string? TerminalId { get; set; }
    }
}