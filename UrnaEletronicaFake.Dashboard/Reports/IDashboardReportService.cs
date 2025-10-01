using UrnaEletronicaFake.Shared.DTOs;

namespace UrnaEletronicaFake.Dashboard.Reports;

public interface IDashboardReportService
{
    Task<DashboardReport> GenerateDashboardReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ElectionReport> GenerateElectionReportAsync(int electionId);
    Task<TerminalReport> GenerateTerminalReportAsync(string terminalId, DateTime? startDate = null, DateTime? endDate = null);
    Task<VotingReport> GenerateVotingReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<SystemReport> GenerateSystemReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> ExportReportToPdfAsync<T>(T report, string filePath) where T : class;
    Task<bool> ExportReportToExcelAsync<T>(T report, string filePath) where T : class;
    Task<bool> ScheduleReportAsync(string reportType, DateTime scheduleTime, string email);
}

public class DashboardReport
{
    public DateTime GeneratedAt { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalElections { get; set; }
    public int ActiveElections { get; set; }
    public int TotalCandidates { get; set; }
    public int TotalVotes { get; set; }
    public int ActiveTerminals { get; set; }
    public int SystemErrors { get; set; }
    public int SecurityViolations { get; set; }
    public double SystemUptime { get; set; }
    public Dictionary<string, int> VotesByPosition { get; set; } = new();
    public Dictionary<string, int> VotesByHour { get; set; } = new();
    public List<AuditEntry> RecentActivities { get; set; } = new();
}

public class ElectionReport
{
    public int ElectionId { get; set; }
    public string ElectionName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int TotalCandidates { get; set; }
    public int TotalVotes { get; set; }
    public double VoterTurnout { get; set; }
    public Dictionary<string, int> VotesByCandidate { get; set; } = new();
    public Dictionary<string, int> VotesByPosition { get; set; } = new();
    public List<VotingTrend> VotingTrends { get; set; } = new();
    public List<AuditEntry> ElectionActivities { get; set; } = new();
}

public class TerminalReport
{
    public string TerminalId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int TotalVotes { get; set; }
    public int SuccessfulVotes { get; set; }
    public int FailedVotes { get; set; }
    public double SuccessRate { get; set; }
    public double AvailabilityRate { get; set; }
    public TimeSpan TotalUptime { get; set; }
    public TimeSpan TotalDowntime { get; set; }
    public Dictionary<string, int> VotesByHour { get; set; } = new();
    public List<AuditEntry> TerminalActivities { get; set; } = new();
    public List<string> Issues { get; set; } = new();
}

public class VotingReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int TotalVotes { get; set; }
    public int VotesToday { get; set; }
    public int VotesThisWeek { get; set; }
    public double VotingRate { get; set; }
    public Dictionary<string, int> VotesByPosition { get; set; } = new();
    public Dictionary<string, int> VotesByCandidate { get; set; } = new();
    public Dictionary<string, int> VotesByDay { get; set; } = new();
    public Dictionary<string, int> VotesByHour { get; set; } = new();
    public List<VotingTrend> Trends { get; set; } = new();
    public List<string> TopCandidates { get; set; } = new();
}

public class SystemReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int TotalEvents { get; set; }
    public int SuccessfulEvents { get; set; }
    public int FailedEvents { get; set; }
    public int SystemErrors { get; set; }
    public int SecurityViolations { get; set; }
    public double SystemUptime { get; set; }
    public double SecurityScore { get; set; }
    public Dictionary<string, int> EventsByType { get; set; } = new();
    public Dictionary<string, int> EventsByUser { get; set; } = new();
    public Dictionary<string, int> EventsByTerminal { get; set; } = new();
    public List<string> CriticalIssues { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class VotingTrend
{
    public DateTime Timestamp { get; set; }
    public int VoteCount { get; set; }
    public string Period { get; set; } = string.Empty;
    public double Percentage { get; set; }
}


