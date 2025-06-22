using System;

namespace UrnaEletronicaFake.Services;

public interface IVotacaoStateService
{
    event Action? OnTerminalStateChanged;
    bool IsTerminalLocked { get; }
    void UnlockTerminal();
    void LockTerminal();
} 