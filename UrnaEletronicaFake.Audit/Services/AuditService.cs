using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Data.Repositories;
using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Shared.Constants;

namespace UrnaEletronicaFake.Audit.Services;

public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;
    private readonly IAuditoriaRepository _auditoriaRepository;

    public AuditService(
        ILogger<AuditService> logger,
        IAuditoriaRepository auditoriaRepository)
    {
        _logger = logger;
        _auditoriaRepository = auditoriaRepository;
    }

    public async Task LogAuditEventAsync(AuditEntry auditEntry)
    {
        try
        {
            var auditoria = new UrnaEletronicaFake.Shared.Models.Auditoria
            {
                Acao = auditEntry.Action.ToString(),
                UsuarioId = auditEntry.UserId,
                Descricao = auditEntry.Description,
                IpAddress = auditEntry.IpAddress ?? string.Empty,
                UserAgent = auditEntry.UserAgent ?? string.Empty,
                DataHora = auditEntry.Timestamp,
                TerminalId = auditEntry.TerminalId ?? string.Empty,
                SessaoId = auditEntry.SessionId ?? string.Empty,
                Sucesso = auditEntry.Success,
                MensagemErro = auditEntry.ErrorMessage ?? string.Empty
            };

            await _auditoriaRepository.AddAsync(auditoria);
            
            _logger.LogDebug("Audit event logged: {Action} by {UserId} at {Timestamp}", 
                auditEntry.Action, auditEntry.UserId, auditEntry.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging audit event for user {UserId}, action {Action}", 
                auditEntry.UserId, auditEntry.Action);
        }
    }

    public async Task LogAuditEventAsync(AuditAction action, string userId, string? description = null, bool success = true)
    {
        var auditEntry = new AuditEntry
        {
            Action = action,
            UserId = userId,
            Description = description ?? GetDefaultDescription(action),
            Timestamp = DateTime.Now,
            Success = success
        };

        await LogAuditEventAsync(auditEntry);
    }

    public async Task<IEnumerable<AuditEntry>> GetAuditLogsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var auditorias = await _auditoriaRepository.GetAuditoriasPorPeriodoAsync(start, end);
            
            return auditorias.Select(a => new AuditEntry
            {
                Id = a.Id,
                Action = Enum.Parse<AuditAction>(a.Acao),
                UserId = a.UsuarioId,
                Description = a.Descricao,
                IpAddress = a.IpAddress,
                UserAgent = a.UserAgent,
                Timestamp = a.DataHora,
                TerminalId = a.TerminalId,
                SessionId = a.SessaoId,
                Success = a.Sucesso,
                ErrorMessage = a.MensagemErro
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs");
            return new List<AuditEntry>();
        }
    }

    public async Task<IEnumerable<AuditEntry>> GetAuditLogsByUserAsync(string userId)
    {
        try
        {
            var auditorias = await _auditoriaRepository.GetAuditoriasPorUsuarioAsync(userId);
            
            return auditorias.Select(a => new AuditEntry
            {
                Id = a.Id,
                Action = Enum.Parse<AuditAction>(a.Acao),
                UserId = a.UsuarioId,
                Description = a.Descricao,
                IpAddress = a.IpAddress,
                UserAgent = a.UserAgent,
                Timestamp = a.DataHora,
                TerminalId = a.TerminalId,
                SessionId = a.SessaoId,
                Success = a.Sucesso,
                ErrorMessage = a.MensagemErro
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for user {UserId}", userId);
            return new List<AuditEntry>();
        }
    }

    public async Task<IEnumerable<AuditEntry>> GetAuditLogsByActionAsync(AuditAction action)
    {
        try
        {
            var auditorias = await _auditoriaRepository.GetAuditoriasPorAcaoAsync(action.ToString());
            
            return auditorias.Select(a => new AuditEntry
            {
                Id = a.Id,
                Action = Enum.Parse<AuditAction>(a.Acao),
                UserId = a.UsuarioId,
                Description = a.Descricao,
                IpAddress = a.IpAddress,
                UserAgent = a.UserAgent,
                Timestamp = a.DataHora,
                TerminalId = a.TerminalId,
                SessionId = a.SessaoId,
                Success = a.Sucesso,
                ErrorMessage = a.MensagemErro
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for action {Action}", action);
            return new List<AuditEntry>();
        }
    }

    public async Task<IEnumerable<AuditEntry>> GetFailedAuditLogsAsync()
    {
        try
        {
            var auditorias = await _auditoriaRepository.GetAuditoriasComFalhaAsync();
            
            return auditorias.Select(a => new AuditEntry
            {
                Id = a.Id,
                Action = Enum.Parse<AuditAction>(a.Acao),
                UserId = a.UsuarioId,
                Description = a.Descricao,
                IpAddress = a.IpAddress,
                UserAgent = a.UserAgent,
                Timestamp = a.DataHora,
                TerminalId = a.TerminalId,
                SessionId = a.SessaoId,
                Success = a.Sucesso,
                ErrorMessage = a.MensagemErro
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving failed audit logs");
            return new List<AuditEntry>();
        }
    }

    public async Task<IEnumerable<AuditEntry>> GetAuditLogsByTerminalAsync(string terminalId)
    {
        try
        {
            var auditorias = await _auditoriaRepository.GetAuditoriasPorTerminalAsync(terminalId);
            
            return auditorias.Select(a => new AuditEntry
            {
                Id = a.Id,
                Action = Enum.Parse<AuditAction>(a.Acao),
                UserId = a.UsuarioId,
                Description = a.Descricao,
                IpAddress = a.IpAddress,
                UserAgent = a.UserAgent,
                Timestamp = a.DataHora,
                TerminalId = a.TerminalId,
                SessionId = a.SessaoId,
                Success = a.Sucesso,
                ErrorMessage = a.MensagemErro
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for terminal {TerminalId}", terminalId);
            return new List<AuditEntry>();
        }
    }

    public async Task<Dictionary<string, int>> GetAuditStatisticsAsync()
    {
        try
        {
            return await _auditoriaRepository.GetEstatisticasAuditoriaAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit statistics");
            return new Dictionary<string, int>();
        }
    }

    public async Task<int> GetTotalAuditLogsAsync()
    {
        try
        {
            return await _auditoriaRepository.GetTotalAuditoriasAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving total audit logs count");
            return 0;
        }
    }

    public async Task<int> GetTotalAuditLogsByUserAsync(string userId)
    {
        try
        {
            return await _auditoriaRepository.GetTotalAuditoriasPorUsuarioAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving total audit logs count for user {UserId}", userId);
            return 0;
        }
    }

    public async Task<int> GetTotalFailedAuditLogsAsync()
    {
        try
        {
            return await _auditoriaRepository.GetTotalAuditoriasComFalhaAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving total failed audit logs count");
            return 0;
        }
    }

    public async Task<bool> ExportAuditLogsAsync(string filePath, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var auditLogs = await GetAuditLogsAsync(startDate, endDate);
            
            using var writer = new StreamWriter(filePath);
            await writer.WriteLineAsync("Id,Action,UserId,Description,IpAddress,UserAgent,Timestamp,TerminalId,SessionId,Success,ErrorMessage");
            
            foreach (var log in auditLogs)
            {
                await writer.WriteLineAsync($"{log.Id},{log.Action},{log.UserId},{log.Description},{log.IpAddress},{log.UserAgent},{log.Timestamp:yyyy-MM-dd HH:mm:ss},{log.TerminalId},{log.SessionId},{log.Success},{log.ErrorMessage}");
            }
            
            _logger.LogInformation("Audit logs exported to {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting audit logs to {FilePath}", filePath);
            return false;
        }
    }

    public async Task<bool> CleanupOldAuditLogsAsync(int retentionDays = 2555)
    {
        try
        {
            var cutoffDate = DateTime.Now.AddDays(-retentionDays);
            var oldLogs = await _auditoriaRepository.GetAuditoriasPorPeriodoAsync(DateTime.MinValue, cutoffDate);
            
            foreach (var log in oldLogs)
            {
                await _auditoriaRepository.DeleteAsync(log);
            }
            
            _logger.LogInformation("Cleaned up {Count} old audit logs older than {RetentionDays} days", 
                oldLogs.Count(), retentionDays);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old audit logs");
            return false;
        }
    }

    private static string GetDefaultDescription(AuditAction action)
    {
        return action switch
        {
            AuditAction.Login => "Usuário fez login no sistema",
            AuditAction.Logout => "Usuário fez logout do sistema",
            AuditAction.Vote => "Voto registrado",
            AuditAction.TerminalUnlock => "Terminal desbloqueado",
            AuditAction.TerminalLock => "Terminal bloqueado",
            AuditAction.ConfigurationChange => "Configuração alterada",
            AuditAction.DataExport => "Dados exportados",
            AuditAction.SecurityViolation => "Violação de segurança detectada",
            AuditAction.SystemError => "Erro do sistema",
            _ => "Ação não especificada"
        };
    }
}

