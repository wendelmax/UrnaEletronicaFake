using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Data.DbContext;
using UrnaEletronicaFake.Shared.Constants;
using System.Text.Json;

namespace UrnaEletronicaFake.Audit.Storage;

public class AuditStorageService : IAuditStorageService
{
    private readonly ILogger<AuditStorageService> _logger;
    private readonly UrnaDbContext _context;

    public AuditStorageService(
        ILogger<AuditStorageService> logger,
        UrnaDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> BackupAuditDataAsync(string backupPath)
    {
        try
        {
            var auditData = await _context.Auditorias.ToListAsync();
            
            var backup = new
            {
                BackupDate = DateTime.Now,
                RecordCount = auditData.Count,
                Data = auditData
            };

            var json = JsonSerializer.Serialize(backup, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await File.WriteAllTextAsync(backupPath, json);
            
            _logger.LogInformation("Audit data backed up to {BackupPath}. Records: {RecordCount}", 
                backupPath, auditData.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error backing up audit data to {BackupPath}", backupPath);
            return false;
        }
    }

    public async Task<bool> RestoreAuditDataAsync(string backupPath)
    {
        try
        {
            if (!File.Exists(backupPath))
            {
                _logger.LogError("Backup file not found: {BackupPath}", backupPath);
                return false;
            }

            var json = await File.ReadAllTextAsync(backupPath);
            var backup = JsonSerializer.Deserialize<JsonElement>(json);

            if (backup.TryGetProperty("data", out var dataElement))
            {
                var auditData = JsonSerializer.Deserialize<List<UrnaEletronicaFake.Shared.Models.Auditoria>>(dataElement.GetRawText());
                
                if (auditData != null)
                {
                    await _context.Auditorias.AddRangeAsync(auditData);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Audit data restored from {BackupPath}. Records: {RecordCount}", 
                        backupPath, auditData.Count);
                    return true;
                }
            }

            _logger.LogError("Invalid backup file format: {BackupPath}", backupPath);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring audit data from {BackupPath}", backupPath);
            return false;
        }
    }

    public async Task<bool> ArchiveOldAuditDataAsync(int retentionDays)
    {
        try
        {
            var cutoffDate = DateTime.Now.AddDays(-retentionDays);
            var oldRecords = await _context.Auditorias
                .Where(a => a.DataHora < cutoffDate)
                .ToListAsync();

            if (oldRecords.Count == 0)
            {
                _logger.LogInformation("No old audit records to archive");
                return true;
            }

            var archivePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "UrnaEletronicaFake", "Archives", $"audit_archive_{DateTime.Now:yyyyMMdd_HHmmss}.json");

            Directory.CreateDirectory(Path.GetDirectoryName(archivePath)!);

            var archive = new
            {
                ArchiveDate = DateTime.Now,
                RecordCount = oldRecords.Count,
                Data = oldRecords
            };

            var json = JsonSerializer.Serialize(archive, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await File.WriteAllTextAsync(archivePath, json);

            _context.Auditorias.RemoveRange(oldRecords);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Archived {RecordCount} old audit records to {ArchivePath}", 
                oldRecords.Count, archivePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving old audit data");
            return false;
        }
    }

    public async Task<long> GetAuditDataSizeAsync()
    {
        try
        {
            var auditData = await _context.Auditorias.ToListAsync();
            var json = JsonSerializer.Serialize(auditData);
            return json.Length;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating audit data size");
            return 0;
        }
    }

    public async Task<int> GetAuditDataCountAsync()
    {
        try
        {
            return await _context.Auditorias.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audit data count");
            return 0;
        }
    }

    public async Task<bool> OptimizeAuditDatabaseAsync()
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("VACUUM");
            _logger.LogInformation("Audit database optimized successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing audit database");
            return false;
        }
    }

    public async Task<bool> ValidateAuditDataIntegrityAsync()
    {
        try
        {
            var totalRecords = await _context.Auditorias.CountAsync();
            var validRecords = await _context.Auditorias
                .Where(a => !string.IsNullOrEmpty(a.Acao) && 
                           !string.IsNullOrEmpty(a.UsuarioId) && 
                           a.DataHora != default)
                .CountAsync();

            var integrityRate = totalRecords > 0 ? (double)validRecords / totalRecords : 1.0;
            
            _logger.LogInformation("Audit data integrity check completed. Valid records: {ValidRecords}/{TotalRecords} ({IntegrityRate:P2})", 
                validRecords, totalRecords, integrityRate);

            return integrityRate >= 0.95;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating audit data integrity");
            return false;
        }
    }

    public async Task<Dictionary<string, object>> GetStorageStatisticsAsync()
    {
        try
        {
            var statistics = new Dictionary<string, object>
            {
                ["TotalRecords"] = await _context.Auditorias.CountAsync(),
                ["OldestRecord"] = await _context.Auditorias.MinAsync(a => (DateTime?)a.DataHora) ?? DateTime.MinValue,
                ["NewestRecord"] = await _context.Auditorias.MaxAsync(a => (DateTime?)a.DataHora) ?? DateTime.MinValue,
                ["SuccessfulRecords"] = await _context.Auditorias.CountAsync(a => a.Sucesso),
                ["FailedRecords"] = await _context.Auditorias.CountAsync(a => !a.Sucesso),
                ["UniqueUsers"] = await _context.Auditorias.Select(a => a.UsuarioId).Distinct().CountAsync(),
                ["UniqueTerminals"] = await _context.Auditorias.Where(a => !string.IsNullOrEmpty(a.TerminalId)).Select(a => a.TerminalId).Distinct().CountAsync(),
                ["DataSize"] = await GetAuditDataSizeAsync()
            };

            _logger.LogInformation("Storage statistics retrieved successfully");
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting storage statistics");
            return new Dictionary<string, object>();
        }
    }
}

