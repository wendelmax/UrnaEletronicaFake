using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Dashboard.Services;

public interface IDashboardService
{
    Task<DashboardOverview> GetDashboardOverviewAsync();
    Task<ElectionStatistics> GetElectionStatisticsAsync();
    Task<TerminalStatusSummary> GetTerminalStatusSummaryAsync();
    Task<VotingProgress> GetVotingProgressAsync();
    Task<SystemHealthStatus> GetSystemHealthStatusAsync();
    Task<IEnumerable<RecentActivity>> GetRecentActivitiesAsync(int count = 10);
    Task<IEnumerable<AuditEntry>> GetRecentAuditLogsAsync(int count = 20);
    Task<Dictionary<string, object>> GetRealTimeMetricsAsync();
    Task<bool> RefreshDashboardDataAsync();
    Task<Dictionary<string, object>> GetCustomReportAsync(string reportType, DateTime? startDate = null, DateTime? endDate = null);
}

public class DashboardOverview
{
    public int TotalElections { get; set; }
    public int ActiveElections { get; set; }
    public int TotalCandidates { get; set; }
    public int TotalVotes { get; set; }
    public int ActiveTerminals { get; set; }
    public int TotalTerminals { get; set; }
    public int SystemErrors { get; set; }
    public int SecurityViolations { get; set; }
    public DateTime LastUpdate { get; set; }
}

public class ElectionStatistics
{
    public int TotalElections { get; set; }
    public int CompletedElections { get; set; }
    public int InProgressElections { get; set; }
    public int ScheduledElections { get; set; }
    public double AverageVoterTurnout { get; set; }
    public Dictionary<string, int> VotesByElection { get; set; } = new();
    public Dictionary<string, int> VotesByPosition { get; set; } = new();
}

public class TerminalStatusSummary
{
    public int TotalTerminals { get; set; }
    public int AvailableTerminals { get; set; }
    public int InUseTerminals { get; set; }
    public int MaintenanceTerminals { get; set; }
    public int ErrorTerminals { get; set; }
    public double AvailabilityRate { get; set; }
    public List<TerminalInfo> TerminalDetails { get; set; } = new();
}

public class TerminalInfo
{
    public string TerminalId { get; set; } = string.Empty;
    public TerminalState State { get; set; }
    public string? CurrentEleitorId { get; set; }
    public DateTime? LastActivity { get; set; }
    public bool IsOnline { get; set; }
    public int TotalVotes { get; set; }
}

public class VotingProgress
{
    public int TotalVotes { get; set; }
    public int VotesToday { get; set; }
    public int VotesThisHour { get; set; }
    public double VotingRate { get; set; }
    public Dictionary<string, int> VotesByHour { get; set; } = new();
    public Dictionary<string, int> VotesByDay { get; set; } = new();
    public List<VotingTrend> Trends { get; set; } = new();
}

public class VotingTrend
{
    public DateTime Timestamp { get; set; }
    public int VoteCount { get; set; }
    public string Period { get; set; } = string.Empty;
}

public class SystemHealthStatus
{
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public string OverallStatus { get; set; } = string.Empty;
    public List<string> Issues { get; set; } = new();
}

public class RecentActivity
{
    public DateTime Timestamp { get; set; }
    public string Activity { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string TerminalId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Details { get; set; } = string.Empty;
}


