using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Audit.Services;
using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;
using System.Text.Json;

namespace UrnaEletronicaFake.Audit.Reports;

public class AuditReportService : IAuditReportService
{
    private readonly ILogger<AuditReportService> _logger;
    private readonly IAuditService _auditService;

    public AuditReportService(
        ILogger<AuditReportService> logger,
        IAuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
    }

    public async Task<AuditSummaryReport> GenerateSummaryReportAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var auditLogs = await _auditService.GetAuditLogsAsync(startDate, endDate);
            var logsList = auditLogs.ToList();

            var report = new AuditSummaryReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalEvents = logsList.Count,
                SuccessfulEvents = logsList.Count(l => l.Success),
                FailedEvents = logsList.Count(l => !l.Success)
            };

            report.SuccessRate = report.TotalEvents > 0 ? (double)report.SuccessfulEvents / report.TotalEvents * 100 : 0;

            report.EventsByAction = logsList
                .GroupBy(l => l.Action.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            report.EventsByUser = logsList
                .GroupBy(l => l.UserId)
                .ToDictionary(g => g.Key, g => g.Count());

            report.EventsByTerminal = logsList
                .Where(l => !string.IsNullOrEmpty(l.TerminalId))
                .GroupBy(l => l.TerminalId!)
                .ToDictionary(g => g.Key, g => g.Count());

            _logger.LogInformation("Generated audit summary report for period {StartDate} to {EndDate}", startDate, endDate);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating audit summary report");
            return new AuditSummaryReport { StartDate = startDate, EndDate = endDate };
        }
    }

    public async Task<UserActivityReport> GenerateUserActivityReportAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var auditLogs = await _auditService.GetAuditLogsByUserAsync(userId);
            var logsList = auditLogs.Where(l => l.Timestamp >= start && l.Timestamp <= end).ToList();

            var report = new UserActivityReport
            {
                UserId = userId,
                StartDate = start,
                EndDate = end,
                TotalActivities = logsList.Count,
                SuccessfulActivities = logsList.Count(l => l.Success),
                FailedActivities = logsList.Count(l => !l.Success)
            };

            report.ActivitiesByAction = logsList
                .GroupBy(l => l.Action.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            report.RecentActivities = logsList
                .OrderByDescending(l => l.Timestamp)
                .Take(50)
                .ToList();

            var sessions = logsList
                .Where(l => l.Action == AuditAction.Login || l.Action == AuditAction.Logout)
                .OrderBy(l => l.Timestamp)
                .ToList();

            report.SessionCount = sessions.Count(l => l.Action == AuditAction.Login);

            _logger.LogInformation("Generated user activity report for user {UserId}", userId);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating user activity report for user {UserId}", userId);
            return new UserActivityReport { UserId = userId, StartDate = startDate ?? DateTime.Now, EndDate = endDate ?? DateTime.Now };
        }
    }

    public async Task<TerminalActivityReport> GenerateTerminalActivityReportAsync(string terminalId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var auditLogs = await _auditService.GetAuditLogsByTerminalAsync(terminalId);
            var logsList = auditLogs.Where(l => l.Timestamp >= start && l.Timestamp <= end).ToList();

            var report = new TerminalActivityReport
            {
                TerminalId = terminalId,
                StartDate = start,
                EndDate = end,
                TotalEvents = logsList.Count,
                SuccessfulEvents = logsList.Count(l => l.Success),
                FailedEvents = logsList.Count(l => !l.Success)
            };

            report.EventsByAction = logsList
                .GroupBy(l => l.Action.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            report.RecentEvents = logsList
                .OrderByDescending(l => l.Timestamp)
                .Take(50)
                .ToList();

            var unlockEvents = logsList.Where(l => l.Action == AuditAction.TerminalUnlock).ToList();
            var lockEvents = logsList.Where(l => l.Action == AuditAction.TerminalLock).ToList();

            report.TotalUptime = TimeSpan.FromMinutes(unlockEvents.Count * 30);
            report.TotalDowntime = TimeSpan.FromMinutes(lockEvents.Count * 30);
            report.AvailabilityRate = report.TotalUptime.TotalMinutes > 0 ? 
                (report.TotalUptime.TotalMinutes / (report.TotalUptime.TotalMinutes + report.TotalDowntime.TotalMinutes)) * 100 : 0;

            _logger.LogInformation("Generated terminal activity report for terminal {TerminalId}", terminalId);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating terminal activity report for terminal {TerminalId}", terminalId);
            return new TerminalActivityReport { TerminalId = terminalId, StartDate = startDate ?? DateTime.Now, EndDate = endDate ?? DateTime.Now };
        }
    }

    public async Task<SecurityViolationReport> GenerateSecurityViolationReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var failedLogs = await _auditService.GetFailedAuditLogsAsync();
            var violationLogs = failedLogs.Where(l => l.Timestamp >= start && l.Timestamp <= end).ToList();

            var report = new SecurityViolationReport
            {
                StartDate = start,
                EndDate = end,
                TotalViolations = violationLogs.Count
            };

            report.ViolationsByType = violationLogs
                .GroupBy(l => l.Action.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            report.ViolationsByUser = violationLogs
                .GroupBy(l => l.UserId)
                .ToDictionary(g => g.Key, g => g.Count());

            report.ViolationsByTerminal = violationLogs
                .Where(l => !string.IsNullOrEmpty(l.TerminalId))
                .GroupBy(l => l.TerminalId!)
                .ToDictionary(g => g.Key, g => g.Count());

            report.RecentViolations = violationLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(50)
                .ToList();

            report.SuspiciousUsers = report.ViolationsByUser
                .Where(kvp => kvp.Value > 5)
                .Select(kvp => kvp.Key)
                .ToList();

            report.SuspiciousTerminals = report.ViolationsByTerminal
                .Where(kvp => kvp.Value > 3)
                .Select(kvp => kvp.Key)
                .ToList();

            _logger.LogInformation("Generated security violation report for period {StartDate} to {EndDate}", start, end);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating security violation report");
            return new SecurityViolationReport { StartDate = startDate ?? DateTime.Now, EndDate = endDate ?? DateTime.Now };
        }
    }

    public async Task<SystemHealthReport> GenerateSystemHealthReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var auditLogs = await _auditService.GetAuditLogsAsync(start, end);
            var logsList = auditLogs.ToList();

            var report = new SystemHealthReport
            {
                StartDate = start,
                EndDate = end,
                TotalEvents = logsList.Count,
                SystemErrors = logsList.Count(l => l.Action == AuditAction.SystemError),
                SecurityViolations = logsList.Count(l => l.Action == AuditAction.SecurityViolation),
                FailedLogins = logsList.Count(l => l.Action == AuditAction.Login && !l.Success),
                SuccessfulLogins = logsList.Count(l => l.Action == AuditAction.Login && l.Success)
            };

            report.SystemUptime = report.TotalEvents > 0 ? 
                (double)(report.TotalEvents - report.SystemErrors) / report.TotalEvents * 100 : 100;

            report.SecurityScore = report.TotalEvents > 0 ? 
                (double)(report.TotalEvents - report.SecurityViolations - report.FailedLogins) / report.TotalEvents * 100 : 100;

            if (report.SecurityScore < 80)
            {
                report.CriticalIssues.Add("Pontuação de segurança baixa");
            }

            if (report.SystemErrors > report.TotalEvents * 0.1)
            {
                report.CriticalIssues.Add("Alto número de erros do sistema");
            }

            if (report.FailedLogins > report.SuccessfulLogins * 0.5)
            {
                report.CriticalIssues.Add("Alto número de tentativas de login falhadas");
            }

            if (report.SecurityScore > 90)
            {
                report.Recommendations.Add("Sistema estável e seguro");
            }
            else
            {
                report.Recommendations.Add("Revisar configurações de segurança");
                report.Recommendations.Add("Monitorar tentativas de acesso suspeitas");
            }

            _logger.LogInformation("Generated system health report for period {StartDate} to {EndDate}", start, end);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating system health report");
            return new SystemHealthReport { StartDate = startDate ?? DateTime.Now, EndDate = endDate ?? DateTime.Now };
        }
    }

    public async Task<bool> ExportReportToCsvAsync<T>(T report, string filePath) where T : class
    {
        try
        {
            using var writer = new StreamWriter(filePath);
            
            if (report is AuditSummaryReport summaryReport)
            {
                await writer.WriteLineAsync("StartDate,EndDate,TotalEvents,SuccessfulEvents,FailedEvents,SuccessRate");
                await writer.WriteLineAsync($"{summaryReport.StartDate:yyyy-MM-dd},{summaryReport.EndDate:yyyy-MM-dd},{summaryReport.TotalEvents},{summaryReport.SuccessfulEvents},{summaryReport.FailedEvents},{summaryReport.SuccessRate:F2}");
            }
            
            _logger.LogInformation("Report exported to CSV: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting report to CSV: {FilePath}", filePath);
            return false;
        }
    }

    public async Task<bool> ExportReportToJsonAsync<T>(T report, string filePath) where T : class
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(report, options);
            await File.WriteAllTextAsync(filePath, json);
            
            _logger.LogInformation("Report exported to JSON: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting report to JSON: {FilePath}", filePath);
            return false;
        }
    }
}

