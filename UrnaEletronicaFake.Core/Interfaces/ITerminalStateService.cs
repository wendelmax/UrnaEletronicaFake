using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Shared.DTOs;

namespace UrnaEletronicaFake.Core.Interfaces;

public interface ITerminalStateService
{
    Task<TerminalState> GetCurrentStateAsync(string? terminalId = null);
    Task<bool> TryLockTerminalAsync(string eleitorId, string? terminalId = null);
    Task<bool> UnlockTerminalAsync(string? terminalId = null);
    Task<bool> SetTerminalStateAsync(TerminalState state, string? terminalId = null, string? reason = null);
    Task<TerminalStatusResponse> GetTerminalStatusAsync(string? terminalId = null);
    Task<bool> IsTerminalAvailableAsync(string? terminalId = null);
    Task<string?> GetCurrentEleitorAsync(string? terminalId = null);
    Task<DateTime?> GetLastActivityAsync(string? terminalId = null);
}

