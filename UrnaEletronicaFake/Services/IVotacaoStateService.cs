using System;

namespace UrnaEletronicaFake.Services;

public interface IVotacaoStateService
{
    event Action? OnTerminalStateChanged;
    bool IsTerminalLocked { get; }
    string? EleitorAutenticadoId { get; }
    void UnlockTerminal(string eleitorId);
    void LockTerminal();
} 