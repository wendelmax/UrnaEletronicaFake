using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Data.Repositories;
using UrnaEletronicaFake.Core.Interfaces;
using UrnaEletronicaFake.Audit.Services;
using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Dashboard.Services;

public class DashboardService : IDashboardService
{
    private readonly ILogger<DashboardService> _logger;
    private readonly IRepository<UrnaEletronicaFake.Shared.Models.Eleicao> _eleicaoRepository;
    private readonly ICandidatoRepository _candidatoRepository;
    private readonly IVotoRepository _votoRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly ITerminalStateService _terminalStateService;
    private readonly UrnaEletronicaFake.Audit.Services.IAuditService _auditService;

    public DashboardService(
        ILogger<DashboardService> logger,
        IRepository<UrnaEletronicaFake.Shared.Models.Eleicao> eleicaoRepository,
        ICandidatoRepository candidatoRepository,
        IVotoRepository votoRepository,
        IAuditoriaRepository auditoriaRepository,
        ITerminalStateService terminalStateService,
        UrnaEletronicaFake.Audit.Services.IAuditService auditService)
    {
        _logger = logger;
        _eleicaoRepository = eleicaoRepository;
        _candidatoRepository = candidatoRepository;
        _votoRepository = votoRepository;
        _auditoriaRepository = auditoriaRepository;
        _terminalStateService = terminalStateService;
        _auditService = auditService;
    }

    public async Task<DashboardOverview> GetDashboardOverviewAsync()
    {
        try
        {
            var elections = await _eleicaoRepository.GetAllAsync();
            var candidates = await _candidatoRepository.GetAllAsync();
            var votes = await _votoRepository.GetAllAsync();
            var auditLogs = await _auditoriaRepository.GetAllAsync();

            var overview = new DashboardOverview
            {
                TotalElections = elections.Count(),
                ActiveElections = elections.Count(e => e.Ativa),
                TotalCandidates = candidates.Count(),
                TotalVotes = votes.Count(),
                ActiveTerminals = 1,
                TotalTerminals = 1,
                SystemErrors = auditLogs.Count(a => a.Acao == "SystemError"),
                SecurityViolations = auditLogs.Count(a => a.Acao == "SecurityViolation"),
                LastUpdate = DateTime.Now
            };

            _logger.LogDebug("Dashboard overview generated successfully");
            return overview;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating dashboard overview");
            return new DashboardOverview { LastUpdate = DateTime.Now };
        }
    }

    public async Task<ElectionStatistics> GetElectionStatisticsAsync()
    {
        try
        {
            var elections = await _eleicaoRepository.GetAllAsync();
            var votes = await _votoRepository.GetAllAsync();

            var statistics = new ElectionStatistics
            {
                TotalElections = elections.Count(),
                CompletedElections = elections.Count(e => e.DataFim < DateTime.Now && !e.Ativa),
                InProgressElections = elections.Count(e => e.Ativa),
                ScheduledElections = elections.Count(e => e.DataInicio > DateTime.Now),
                AverageVoterTurnout = 0
            };

            statistics.VotesByElection = elections.ToDictionary(
                e => e.Nome,
                e => votes.Count(v => v.DataVoto >= e.DataInicio && v.DataVoto <= e.DataFim)
            );

            statistics.VotesByPosition = votes
                .GroupBy(v => v.Cargo)
                .ToDictionary(g => g.Key, g => g.Count());

            _logger.LogDebug("Election statistics generated successfully");
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating election statistics");
            return new ElectionStatistics();
        }
    }

    public async Task<TerminalStatusSummary> GetTerminalStatusSummaryAsync()
    {
        try
        {
            var terminalStatus = await _terminalStateService.GetTerminalStatusAsync();
            var votes = await _votoRepository.GetAllAsync();

            var summary = new TerminalStatusSummary
            {
                TotalTerminals = 1,
                AvailableTerminals = terminalStatus.IsAvailable ? 1 : 0,
                InUseTerminals = !terminalStatus.IsAvailable ? 1 : 0,
                MaintenanceTerminals = 0,
                ErrorTerminals = 0,
                AvailabilityRate = terminalStatus.IsAvailable ? 100 : 0
            };

            summary.TerminalDetails.Add(new TerminalInfo
            {
                TerminalId = "TERMINAL-001",
                State = terminalStatus.State,
                CurrentEleitorId = terminalStatus.CurrentEleitorId,
                LastActivity = terminalStatus.LastActivity,
                IsOnline = true,
                TotalVotes = votes.Count()
            });

            _logger.LogDebug("Terminal status summary generated successfully");
            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating terminal status summary");
            return new TerminalStatusSummary();
        }
    }

    public async Task<VotingProgress> GetVotingProgressAsync()
    {
        try
        {
            var votes = await _votoRepository.GetAllAsync();
            var today = DateTime.Now.Date;
            var thisHour = DateTime.Now.AddHours(-1);

            var progress = new VotingProgress
            {
                TotalVotes = votes.Count(),
                VotesToday = votes.Count(v => v.DataVoto.Date == today),
                VotesThisHour = votes.Count(v => v.DataVoto >= thisHour),
                VotingRate = 0
            };

            progress.VotesByHour = votes
                .Where(v => v.DataVoto.Date == today)
                .GroupBy(v => v.DataVoto.ToString("HH:00"))
                .ToDictionary(g => g.Key, g => g.Count());

            progress.VotesByDay = votes
                .GroupBy(v => v.DataVoto.ToString("yyyy-MM-dd"))
                .ToDictionary(g => g.Key, g => g.Count());

            _logger.LogDebug("Voting progress generated successfully");
            return progress;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating voting progress");
            return new VotingProgress();
        }
    }

    public async Task<SystemHealthStatus> GetSystemHealthStatusAsync()
    {
        try
        {
            var auditLogs = await _auditoriaRepository.GetAllAsync();
            var recentLogs = auditLogs.Where(a => a.DataHora >= DateTime.Now.AddHours(-1));

            var health = new SystemHealthStatus
            {
                CpuUsage = 0,
                MemoryUsage = 0,
                DiskUsage = 0,
                ActiveConnections = 1,
                ErrorCount = recentLogs.Count(a => !a.Sucesso),
                WarningCount = recentLogs.Count(a => a.Acao == "SecurityViolation"),
                OverallStatus = "Healthy"
            };

            if (health.ErrorCount > 10)
            {
                health.OverallStatus = "Critical";
                health.Issues.Add("High error rate detected");
            }
            else if (health.ErrorCount > 5)
            {
                health.OverallStatus = "Warning";
                health.Issues.Add("Moderate error rate detected");
            }

            _logger.LogDebug("System health status generated successfully");
            return health;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating system health status");
            return new SystemHealthStatus { OverallStatus = "Error" };
        }
    }

    public async Task<IEnumerable<RecentActivity>> GetRecentActivitiesAsync(int count = 10)
    {
        try
        {
            var auditLogs = await _auditoriaRepository.GetAllAsync();
            var recentLogs = auditLogs
                .OrderByDescending(a => a.DataHora)
                .Take(count);

            var activities = recentLogs.Select(a => new RecentActivity
            {
                Timestamp = a.DataHora,
                Activity = a.Acao,
                UserId = a.UsuarioId,
                TerminalId = a.TerminalId ?? string.Empty,
                Success = a.Sucesso,
                Details = a.Descricao ?? string.Empty
            });

            _logger.LogDebug("Recent activities retrieved successfully");
            return activities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent activities");
            return new List<RecentActivity>();
        }
    }

    public async Task<IEnumerable<AuditEntry>> GetRecentAuditLogsAsync(int count = 20)
    {
        try
        {
            var auditLogs = await _auditService.GetAuditLogsAsync();
            return auditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent audit logs");
            return new List<AuditEntry>();
        }
    }

    public async Task<Dictionary<string, object>> GetRealTimeMetricsAsync()
    {
        try
        {
            var overview = await GetDashboardOverviewAsync();
            var terminalStatus = await GetTerminalStatusSummaryAsync();
            var votingProgress = await GetVotingProgressAsync();

            return new Dictionary<string, object>
            {
                ["TotalVotes"] = overview.TotalVotes,
                ["ActiveTerminals"] = overview.ActiveTerminals,
                ["SystemErrors"] = overview.SystemErrors,
                ["VotingRate"] = votingProgress.VotingRate,
                ["TerminalAvailability"] = terminalStatus.AvailabilityRate,
                ["LastUpdate"] = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting real-time metrics");
            return new Dictionary<string, object>();
        }
    }

    public async Task<bool> RefreshDashboardDataAsync()
    {
        try
        {
            _logger.LogInformation("Refreshing dashboard data...");
            
            await GetDashboardOverviewAsync();
            await GetElectionStatisticsAsync();
            await GetTerminalStatusSummaryAsync();
            await GetVotingProgressAsync();
            await GetSystemHealthStatusAsync();
            
            _logger.LogInformation("Dashboard data refreshed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing dashboard data");
            return false;
        }
    }

    public async Task<Dictionary<string, object>> GetCustomReportAsync(string reportType, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            return reportType.ToLower() switch
            {
                "election" => await GetElectionReportAsync(start, end),
                "voting" => await GetVotingReportAsync(start, end),
                "terminal" => await GetTerminalReportAsync(start, end),
                "audit" => await GetAuditReportAsync(start, end),
                _ => new Dictionary<string, object> { ["Error"] = "Invalid report type" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating custom report: {ReportType}", reportType);
            return new Dictionary<string, object> { ["Error"] = ex.Message };
        }
    }

    private async Task<Dictionary<string, object>> GetElectionReportAsync(DateTime startDate, DateTime endDate)
    {
        var elections = await _eleicaoRepository.GetAllAsync();
        var votes = await _votoRepository.GetAllAsync();

        return new Dictionary<string, object>
        {
            ["Period"] = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
            ["TotalElections"] = elections.Count(),
            ["ActiveElections"] = elections.Count(e => e.Ativa),
            ["TotalVotes"] = votes.Count(v => v.DataVoto >= startDate && v.DataVoto <= endDate),
            ["GeneratedAt"] = DateTime.Now
        };
    }

    private async Task<Dictionary<string, object>> GetVotingReportAsync(DateTime startDate, DateTime endDate)
    {
        var votes = await _votoRepository.GetAllAsync();
        var periodVotes = votes.Where(v => v.DataVoto >= startDate && v.DataVoto <= endDate);

        return new Dictionary<string, object>
        {
            ["Period"] = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
            ["TotalVotes"] = periodVotes.Count(),
            ["VotesByPosition"] = periodVotes.GroupBy(v => v.Cargo).ToDictionary(g => g.Key, g => g.Count()),
            ["GeneratedAt"] = DateTime.Now
        };
    }

    private async Task<Dictionary<string, object>> GetTerminalReportAsync(DateTime startDate, DateTime endDate)
    {
        var terminalStatus = await _terminalStateService.GetTerminalStatusAsync();
        var votes = await _votoRepository.GetAllAsync();

        return new Dictionary<string, object>
        {
            ["Period"] = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
            ["TerminalId"] = "TERMINAL-001",
            ["Status"] = terminalStatus.State.ToString(),
            ["IsAvailable"] = terminalStatus.IsAvailable,
            ["TotalVotes"] = votes.Count(v => v.DataVoto >= startDate && v.DataVoto <= endDate),
            ["GeneratedAt"] = DateTime.Now
        };
    }

    private async Task<Dictionary<string, object>> GetAuditReportAsync(DateTime startDate, DateTime endDate)
    {
        var auditLogs = await _auditoriaRepository.GetAllAsync();
        var periodLogs = auditLogs.Where(a => a.DataHora >= startDate && a.DataHora <= endDate);

        return new Dictionary<string, object>
        {
            ["Period"] = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
            ["TotalEvents"] = periodLogs.Count(),
            ["SuccessfulEvents"] = periodLogs.Count(a => a.Sucesso),
            ["FailedEvents"] = periodLogs.Count(a => !a.Sucesso),
            ["EventsByAction"] = periodLogs.GroupBy(a => a.Acao).ToDictionary(g => g.Key, g => g.Count()),
            ["GeneratedAt"] = DateTime.Now
        };
    }
}
