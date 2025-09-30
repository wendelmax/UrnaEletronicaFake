using UrnaEletronicaFake.Modules.Core.Events;
using UrnaEletronicaFake.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UrnaEletronicaFake.Modules.Core.Services;

public class TerminalStateService : ITerminalStateService
{
    private readonly IEventBus _eventBus;
    private readonly ITerminalLogService _terminalLogService;
    
    private bool _isLocked = true;
    private string? _currentEleitorId;
    private DateTime? _lastStateChange;

    public bool IsTerminalLocked => _isLocked;
    public string? CurrentEleitorId => _currentEleitorId;
    public DateTime? LastStateChange => _lastStateChange;

    public TerminalStateService(IEventBus eventBus, ITerminalLogService terminalLogService)
    {
        _eventBus = eventBus;
        _terminalLogService = terminalLogService;
        _lastStateChange = DateTime.Now;
    }

    public async Task UnlockTerminalAsync(string eleitorId)
    {
        if (_isLocked && await ValidateEleitorAsync(eleitorId))
        {
            _isLocked = false;
            _currentEleitorId = eleitorId;
            _lastStateChange = DateTime.Now;
            
            _terminalLogService.Registrar($"[TERMINAL] Terminal desbloqueado para eleitor: {eleitorId}");
            
            await _eventBus.PublishAsync(new TerminalUnlockedEvent(eleitorId, _lastStateChange.Value));
        }
    }

    public async Task LockTerminalAsync(string? reason = null)
    {
        if (!_isLocked)
        {
            var previousEleitorId = _currentEleitorId;
            _isLocked = true;
            _currentEleitorId = null;
            _lastStateChange = DateTime.Now;
            
            var logMessage = reason != null 
                ? $"[TERMINAL] Terminal bloqueado - {reason}"
                : $"[TERMINAL] Terminal bloqueado";
                
            _terminalLogService.Registrar(logMessage);
            
            await _eventBus.PublishAsync(new TerminalLockedEvent(previousEleitorId, _lastStateChange.Value));
        }
    }

    public Task<bool> ValidateEleitorAsync(string eleitorId)
    {
        if (string.IsNullOrWhiteSpace(eleitorId))
            return Task.FromResult(false);
            
        if (!eleitorId.All(char.IsDigit))
            return Task.FromResult(false);
            
        if (eleitorId.Length < 4 || eleitorId.Length > 12)
            return Task.FromResult(false);
            
        return Task.FromResult(true);
    }
}