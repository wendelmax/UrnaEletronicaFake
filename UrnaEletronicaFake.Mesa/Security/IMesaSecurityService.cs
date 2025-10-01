using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Mesa.Security;

public interface IMesaSecurityService
{
    Task<bool> ValidateEleitorAsync(string eleitorId);
    Task<bool> ValidateMesarioAsync(string mesarioId, string senha);
    Task<bool> CanUnlockTerminalAsync(string mesarioId);
    Task<bool> CanLockTerminalAsync(string mesarioId);
    Task<bool> IsSessionValidAsync(string mesarioId);
    Task<bool> IsTerminalAvailableAsync();
    Task<string?> GetCurrentEleitorAsync();
    Task<DateTime?> GetLastActivityAsync();
    Task<bool> HasPermissionAsync(string mesarioId, string action);
    Task LogSecurityEventAsync(string mesarioId, string action, bool success, string? details = null);
}

