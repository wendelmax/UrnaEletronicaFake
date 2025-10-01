using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Core.Interfaces;

namespace UrnaEletronicaFake.UI.Services;

public class LegacyIntegrationService : IVotacaoStateService
{
    private readonly ITerminalStateService _newTerminalStateService;
    private readonly ILogger<LegacyIntegrationService> _logger;

    public event Action? OnTerminalStateChanged;

    public LegacyIntegrationService(ITerminalStateService newTerminalStateService, ILogger<LegacyIntegrationService> logger)
    {
        _newTerminalStateService = newTerminalStateService;
        _logger = logger;
    }

    public bool IsTerminalLocked { get; private set; } = true;

    public string? EleitorAutenticadoId { get; private set; }

    public async void UnlockTerminal(string eleitorId)
    {
        try
        {
            _logger.LogInformation("Legacy UnlockTerminal called - delegating to ITerminalStateService.TryLockTerminalAsync");
            var success = await _newTerminalStateService.TryLockTerminalAsync(eleitorId);
            if (success)
            {
                IsTerminalLocked = false;
                EleitorAutenticadoId = eleitorId;
                OnTerminalStateChanged?.Invoke();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in legacy unlock terminal operation");
        }
    }

    public async void LockTerminal()
    {
        try
        {
            _logger.LogInformation("Legacy LockTerminal called - delegating to ITerminalStateService.SetTerminalStateAsync");
            var ok = await _newTerminalStateService.SetTerminalStateAsync(Shared.Enums.TerminalState.Locked, reason: "Legacy lock operation");
            if (ok)
            {
                IsTerminalLocked = true;
                EleitorAutenticadoId = null;
                OnTerminalStateChanged?.Invoke();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in legacy lock terminal operation");
        }
    }
}
