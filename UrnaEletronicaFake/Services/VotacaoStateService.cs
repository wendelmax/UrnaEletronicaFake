using System;

namespace UrnaEletronicaFake.Services;

public class VotacaoStateService : IVotacaoStateService
{
    public event Action? OnTerminalStateChanged;

    private bool _isLocked = true;
    public bool IsTerminalLocked => _isLocked;

    private string? _eleitorId;
    public string? EleitorAutenticadoId => _eleitorId;

    public void UnlockTerminal(string eleitorId)
    {
        if (_isLocked && !string.IsNullOrWhiteSpace(eleitorId))
        {
            _isLocked = false;
            _eleitorId = eleitorId;
            OnTerminalStateChanged?.Invoke();
        }
    }

    public void LockTerminal()
    {
        if (!_isLocked)
        {
            _isLocked = true;
            _eleitorId = null;
            OnTerminalStateChanged?.Invoke();
        }
    }
} 