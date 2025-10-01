using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Mesa.Services;

public interface IMesaService
{
    Task<bool> LoginAsync(string mesarioId, string senha);
    Task<bool> LogoutAsync(string mesarioId);
    Task<bool> UnlockTerminalAsync(string mesarioId, string eleitorId);
    Task<bool> LockTerminalAsync(string mesarioId);
    Task<TerminalStatusResponse> GetTerminalStatusAsync();
    Task<bool> IsLoggedInAsync(string mesarioId);
    Task<DateTime?> GetSessionStartTimeAsync(string mesarioId);
    Task<bool> ExtendSessionAsync(string mesarioId);
    Task<IEnumerable<AuditEntry>> GetMesaAuditLogAsync(string mesarioId);
    Task<bool> CanPerformActionAsync(string mesarioId, string action);
}

