using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Audit.Reports;

public interface IAuditReportService
{
    Task<AuditSummaryReport> GenerateSummaryReportAsync(DateTime startDate, DateTime endDate);
    Task<UserActivityReport> GenerateUserActivityReportAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<TerminalActivityReport> GenerateTerminalActivityReportAsync(string terminalId, DateTime? startDate = null, DateTime? endDate = null);
    Task<SecurityViolationReport> GenerateSecurityViolationReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<SystemHealthReport> GenerateSystemHealthReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> ExportReportToCsvAsync<T>(T report, string filePath) where T : class;
    Task<bool> ExportReportToJsonAsync<T>(T report, string filePath) where T : class;
}

public class AuditSummaryReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalEvents { get; set; }
    public int SuccessfulEvents { get; set; }
    public int FailedEvents { get; set; }
    public Dictionary<string, int> EventsByAction { get; set; } = new();
    public Dictionary<string, int> EventsByUser { get; set; } = new();
    public Dictionary<string, int> EventsByTerminal { get; set; } = new();
    public double SuccessRate { get; set; }
    public TimeSpan AverageEventDuration { get; set; }
}

public class UserActivityReport
{
    public string UserId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalActivities { get; set; }
    public int SuccessfulActivities { get; set; }
    public int FailedActivities { get; set; }
    public Dictionary<string, int> ActivitiesByAction { get; set; } = new();
    public List<AuditEntry> RecentActivities { get; set; } = new();
    public TimeSpan TotalSessionTime { get; set; }
    public int SessionCount { get; set; }
}

public class TerminalActivityReport
{
    public string TerminalId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalEvents { get; set; }
    public int SuccessfulEvents { get; set; }
    public int FailedEvents { get; set; }
    public Dictionary<string, int> EventsByAction { get; set; } = new();
    public List<AuditEntry> RecentEvents { get; set; } = new();
    public TimeSpan TotalUptime { get; set; }
    public TimeSpan TotalDowntime { get; set; }
    public double AvailabilityRate { get; set; }
}

public class SecurityViolationReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalViolations { get; set; }
    public Dictionary<string, int> ViolationsByType { get; set; } = new();
    public Dictionary<string, int> ViolationsByUser { get; set; } = new();
    public Dictionary<string, int> ViolationsByTerminal { get; set; } = new();
    public List<AuditEntry> RecentViolations { get; set; } = new();
    public List<string> SuspiciousUsers { get; set; } = new();
    public List<string> SuspiciousTerminals { get; set; } = new();
}

public class SystemHealthReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalEvents { get; set; }
    public int SystemErrors { get; set; }
    public int SecurityViolations { get; set; }
    public int FailedLogins { get; set; }
    public int SuccessfulLogins { get; set; }
    public double SystemUptime { get; set; }
    public double SecurityScore { get; set; }
    public List<string> CriticalIssues { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

