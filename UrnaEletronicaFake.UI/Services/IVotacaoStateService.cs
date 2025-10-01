using System;

namespace UrnaEletronicaFake.UI.Services;

public interface IVotacaoStateService
{
    event Action? OnTerminalStateChanged;
    bool IsTerminalLocked { get; }
    string? EleitorAutenticadoId { get; }
    void UnlockTerminal(string eleitorId);
    void LockTerminal();
} 