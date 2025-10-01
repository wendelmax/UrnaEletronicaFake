using UrnaEletronicaFake.Shared.DTOs;

namespace UrnaEletronicaFake.Audit.Storage;

public interface IAuditStorageService
{
    Task<bool> BackupAuditDataAsync(string backupPath);
    Task<bool> RestoreAuditDataAsync(string backupPath);
    Task<bool> ArchiveOldAuditDataAsync(int retentionDays);
    Task<long> GetAuditDataSizeAsync();
    Task<int> GetAuditDataCountAsync();
    Task<bool> OptimizeAuditDatabaseAsync();
    Task<bool> ValidateAuditDataIntegrityAsync();
    Task<Dictionary<string, object>> GetStorageStatisticsAsync();
}

