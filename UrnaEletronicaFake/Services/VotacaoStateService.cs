using System;

namespace UrnaEletronicaFake.Services;

public class VotacaoStateService : IVotacaoStateService
{
    public event Action? OnTerminalStateChanged;

    private bool _isLocked = true;
    public bool IsTerminalLocked => _isLocked;

    public void UnlockTerminal()
    {
        if (_isLocked)
        {
            _isLocked = false;
            OnTerminalStateChanged?.Invoke();
        }
    }

    public void LockTerminal()
    {
        if (!_isLocked)
        {
            _isLocked = true;
            OnTerminalStateChanged?.Invoke();
        }
    }
} 