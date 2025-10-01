using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Audit.Services;

public interface IAuditService
{
    Task LogAuditEventAsync(AuditEntry auditEntry);
    Task LogAuditEventAsync(AuditAction action, string userId, string? description = null, bool success = true);
    Task<IEnumerable<AuditEntry>> GetAuditLogsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<AuditEntry>> GetAuditLogsByUserAsync(string userId);
    Task<IEnumerable<AuditEntry>> GetAuditLogsByActionAsync(AuditAction action);
    Task<IEnumerable<AuditEntry>> GetFailedAuditLogsAsync();
    Task<IEnumerable<AuditEntry>> GetAuditLogsByTerminalAsync(string terminalId);
    Task<Dictionary<string, int>> GetAuditStatisticsAsync();
    Task<int> GetTotalAuditLogsAsync();
    Task<int> GetTotalAuditLogsByUserAsync(string userId);
    Task<int> GetTotalFailedAuditLogsAsync();
    Task<bool> ExportAuditLogsAsync(string filePath, DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> CleanupOldAuditLogsAsync(int retentionDays = 2555);
}

