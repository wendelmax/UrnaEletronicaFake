using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Modules.Core.Services;

namespace UrnaEletronicaFake.Services;

public class LegacyIntegrationService : IVotacaoStateService
{
    private readonly ITerminalStateService _newTerminalStateService;
    private readonly ILogger<LegacyIntegrationService> _logger;

    public event Action? OnTerminalStateChanged;

    public LegacyIntegrationService(ITerminalStateService newTerminalStateService, ILogger<LegacyIntegrationService> logger)
    {
        _newTerminalStateService = newTerminalStateService;
        _logger = logger;

        SubscribeToNewEvents();
    }

    private void SubscribeToNewEvents()
    {
        _newTerminalStateService.OnTerminalUnlocked += async (e) => 
        {
            _logger.LogDebug("Propagating terminal unlocked event from new service to legacy interface");
            OnTerminalStateChanged?.Invoke();
        };

        _newTerminalStateService.OnTerminalLocked += async (e) => 
        {
            _logger.LogDebug("Propagating terminal locked event from new service to legacy interface");
            OnTerminalStateChanged?.Invoke();
        };
    }

    public bool IsTerminalLocked => _newTerminalStateService.IsTerminalLocked;

    public string? EleitorAutenticadoId => _newTerminalStateService.EleitorAutenticadoId;

    public void UnlockTerminal(string eleitorId)
    {
        var mesarioId = Environment.UserName;
        _logger.LogInformation("Legacy UnlockTerminal called. Converting to async call with MesarioId: {MesarioId}", mesarioId);
        
        Task.Run(async () => 
        {
            try 
            {
                await _newTerminalStateService.UnlockTerminalAsync(eleitorId, mesarioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in legacy unlock terminal operation");
                await _newTerminalStateService.HandleTerminalErrorAsync($"Legacy unlock error: {ex.Message}", ex.ToString());
            }
        });
    }

    public void LockTerminal()
    {
        _logger.LogInformation("Legacy LockTerminal called. Converting to async call");
        
        Task.Run(async () => 
        {
            try 
            {
                await _newTerminalStateService.LockTerminalAsync("Legacy lock operation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in legacy lock terminal operation");
                await _newTerminalStateService.HandleTerminalErrorAsync($"Legacy lock error: {ex.Message}", ex.ToString());
            }
        });
    }
}