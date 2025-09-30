using System;

namespace UrnaEletronicaFake.Modules.Core.Services;

public interface ITerminalStateService
{
    bool IsTerminalLocked { get; }
    string? CurrentEleitorId { get; }
    DateTime? LastStateChange { get; }
    
    Task UnlockTerminalAsync(string eleitorId);
    Task LockTerminalAsync(string? reason = null);
    Task<bool> ValidateEleitorAsync(string eleitorId);
}