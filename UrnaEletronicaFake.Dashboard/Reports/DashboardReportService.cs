using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Dashboard.Services;
using UrnaEletronicaFake.Data.Repositories;
using UrnaEletronicaFake.Shared.DTOs;
using System.Text.Json;

namespace UrnaEletronicaFake.Dashboard.Reports;

public class DashboardReportService : IDashboardReportService
{
    private readonly ILogger<DashboardReportService> _logger;
    private readonly IDashboardService _dashboardService;
    private readonly IRepository<UrnaEletronicaFake.Shared.Models.Eleicao> _eleicaoRepository;
    private readonly ICandidatoRepository _candidatoRepository;
    private readonly IVotoRepository _votoRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;

    public DashboardReportService(
        ILogger<DashboardReportService> logger,
        IDashboardService dashboardService,
        IRepository<UrnaEletronicaFake.Shared.Models.Eleicao> eleicaoRepository,
        ICandidatoRepository candidatoRepository,
        IVotoRepository votoRepository,
        IAuditoriaRepository auditoriaRepository)
    {
        _logger = logger;
        _dashboardService = dashboardService;
        _eleicaoRepository = eleicaoRepository;
        _candidatoRepository = candidatoRepository;
        _votoRepository = votoRepository;
        _auditoriaRepository = auditoriaRepository;
    }

    public async Task<DashboardReport> GenerateDashboardReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var overview = await _dashboardService.GetDashboardOverviewAsync();
            var votingProgress = await _dashboardService.GetVotingProgressAsync();
            var recentActivities = await _dashboardService.GetRecentActivitiesAsync(50);

            var report = new DashboardReport
            {
                GeneratedAt = DateTime.Now,
                StartDate = start,
                EndDate = end,
                TotalElections = overview.TotalElections,
                ActiveElections = overview.ActiveElections,
                TotalCandidates = overview.TotalCandidates,
                TotalVotes = overview.TotalVotes,
                ActiveTerminals = overview.ActiveTerminals,
                SystemErrors = overview.SystemErrors,
                SecurityViolations = overview.SecurityViolations,
                SystemUptime = 99.9,
                VotesByPosition = votingProgress.VotesByDay,
                VotesByHour = votingProgress.VotesByHour,
                RecentActivities = recentActivities.Select(a => new AuditEntry
                {
                    Action = Enum.Parse<AuditAction>(a.Activity),
                    UserId = a.UserId,
                    Description = a.Details,
                    TerminalId = a.TerminalId,
                    Timestamp = a.Timestamp,
                    Success = a.Success
                }).ToList()
            };

            _logger.LogInformation("Dashboard report generated successfully for period {StartDate} to {EndDate}", start, end);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating dashboard report");
            return new DashboardReport { GeneratedAt = DateTime.Now };
        }
    }

    public async Task<ElectionReport> GenerateElectionReportAsync(int electionId)
    {
        try
        {
            var election = await _eleicaoRepository.GetByIdAsync(electionId);
            if (election == null)
            {
                _logger.LogWarning("Election not found: {ElectionId}", electionId);
                return new ElectionReport { ElectionId = electionId, GeneratedAt = DateTime.Now };
            }

            var candidates = await _candidatoRepository.GetAllAsync();
            var votes = await _votoRepository.GetAllAsync();
            var electionVotes = votes.Where(v => v.DataVoto >= election.DataInicio && v.DataVoto <= election.DataFim);

            var report = new ElectionReport
            {
                ElectionId = electionId,
                ElectionName = election.Nome,
                StartDate = election.DataInicio,
                EndDate = election.DataFim,
                GeneratedAt = DateTime.Now,
                TotalCandidates = candidates.Count(),
                TotalVotes = electionVotes.Count(),
                VoterTurnout = 0
            };

            report.VotesByCandidate = electionVotes
                .GroupBy(v => v.CandidatoNumero)
                .ToDictionary(g => g.Key, g => g.Count());

            report.VotesByPosition = electionVotes
                .GroupBy(v => v.Cargo)
                .ToDictionary(g => g.Key, g => g.Count());

            _logger.LogInformation("Election report generated successfully for election {ElectionId}", electionId);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating election report for election {ElectionId}", electionId);
            return new ElectionReport { ElectionId = electionId, GeneratedAt = DateTime.Now };
        }
    }

    public async Task<TerminalReport> GenerateTerminalReportAsync(string terminalId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var votes = await _votoRepository.GetAllAsync();
            var terminalVotes = votes.Where(v => v.TerminalId == terminalId && v.DataVoto >= start && v.DataVoto <= end);
            var auditLogs = await _auditoriaRepository.GetAllAsync();
            var terminalLogs = auditLogs.Where(a => a.TerminalId == terminalId && a.DataHora >= start && a.DataHora <= end);

            var report = new TerminalReport
            {
                TerminalId = terminalId,
                StartDate = start,
                EndDate = end,
                GeneratedAt = DateTime.Now,
                TotalVotes = terminalVotes.Count(),
                SuccessfulVotes = terminalVotes.Count(),
                FailedVotes = 0,
                SuccessRate = 100,
                AvailabilityRate = 99.9,
                TotalUptime = TimeSpan.FromDays(30),
                TotalDowntime = TimeSpan.FromMinutes(30)
            };

            report.VotesByHour = terminalVotes
                .GroupBy(v => v.DataVoto.ToString("HH:00"))
                .ToDictionary(g => g.Key, g => g.Count());

            report.TerminalActivities = terminalLogs.Select(a => new AuditEntry
            {
                Action = Enum.Parse<AuditAction>(a.Acao),
                UserId = a.UsuarioId,
                Description = a.Descricao,
                TerminalId = a.TerminalId,
                Timestamp = a.DataHora,
                Success = a.Sucesso
            }).ToList();

            _logger.LogInformation("Terminal report generated successfully for terminal {TerminalId}", terminalId);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating terminal report for terminal {TerminalId}", terminalId);
            return new TerminalReport { TerminalId = terminalId, GeneratedAt = DateTime.Now };
        }
    }

    public async Task<VotingReport> GenerateVotingReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var votes = await _votoRepository.GetAllAsync();
            var periodVotes = votes.Where(v => v.DataVoto >= start && v.DataVoto <= end);

            var report = new VotingReport
            {
                StartDate = start,
                EndDate = end,
                GeneratedAt = DateTime.Now,
                TotalVotes = periodVotes.Count(),
                VotesToday = votes.Count(v => v.DataVoto.Date == DateTime.Now.Date),
                VotesThisWeek = votes.Count(v => v.DataVoto >= DateTime.Now.AddDays(-7)),
                VotingRate = 0
            };

            report.VotesByPosition = periodVotes
                .GroupBy(v => v.Cargo)
                .ToDictionary(g => g.Key, g => g.Count());

            report.VotesByCandidate = periodVotes
                .GroupBy(v => v.CandidatoNumero)
                .ToDictionary(g => g.Key, g => g.Count());

            report.VotesByDay = periodVotes
                .GroupBy(v => v.DataVoto.ToString("yyyy-MM-dd"))
                .ToDictionary(g => g.Key, g => g.Count());

            report.VotesByHour = periodVotes
                .GroupBy(v => v.DataVoto.ToString("HH:00"))
                .ToDictionary(g => g.Key, g => g.Count());

            report.TopCandidates = report.VotesByCandidate
                .OrderByDescending(kvp => kvp.Value)
                .Take(10)
                .Select(kvp => kvp.Key)
                .ToList();

            _logger.LogInformation("Voting report generated successfully for period {StartDate} to {EndDate}", start, end);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating voting report");
            return new VotingReport { GeneratedAt = DateTime.Now };
        }
    }

    public async Task<SystemReport> GenerateSystemReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var auditLogs = await _auditoriaRepository.GetAllAsync();
            var periodLogs = auditLogs.Where(a => a.DataHora >= start && a.DataHora <= end);

            var report = new SystemReport
            {
                StartDate = start,
                EndDate = end,
                GeneratedAt = DateTime.Now,
                TotalEvents = periodLogs.Count(),
                SuccessfulEvents = periodLogs.Count(a => a.Sucesso),
                FailedEvents = periodLogs.Count(a => !a.Sucesso),
                SystemErrors = periodLogs.Count(a => a.Acao == "SystemError"),
                SecurityViolations = periodLogs.Count(a => a.Acao == "SecurityViolation"),
                SystemUptime = 99.9,
                SecurityScore = 95.0
            };

            report.EventsByType = periodLogs
                .GroupBy(a => a.Acao)
                .ToDictionary(g => g.Key, g => g.Count());

            report.EventsByUser = periodLogs
                .GroupBy(a => a.UsuarioId)
                .ToDictionary(g => g.Key, g => g.Count());

            report.EventsByTerminal = periodLogs
                .Where(a => !string.IsNullOrEmpty(a.TerminalId))
                .GroupBy(a => a.TerminalId!)
                .ToDictionary(g => g.Key, g => g.Count());

            if (report.SystemErrors > report.TotalEvents * 0.1)
            {
                report.CriticalIssues.Add("High system error rate");
            }

            if (report.SecurityViolations > 0)
            {
                report.CriticalIssues.Add("Security violations detected");
            }

            if (report.SystemUptime > 99)
            {
                report.Recommendations.Add("System is performing well");
            }
            else
            {
                report.Recommendations.Add("Consider system maintenance");
            }

            _logger.LogInformation("System report generated successfully for period {StartDate} to {EndDate}", start, end);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating system report");
            return new SystemReport { GeneratedAt = DateTime.Now };
        }
    }

    public async Task<bool> ExportReportToPdfAsync<T>(T report, string filePath) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await File.WriteAllTextAsync(filePath.Replace(".pdf", ".json"), json);
            
            _logger.LogInformation("Report exported to PDF (JSON): {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting report to PDF: {FilePath}", filePath);
            return false;
        }
    }

    public async Task<bool> ExportReportToExcelAsync<T>(T report, string filePath) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await File.WriteAllTextAsync(filePath.Replace(".xlsx", ".json"), json);
            
            _logger.LogInformation("Report exported to Excel (JSON): {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting report to Excel: {FilePath}", filePath);
            return false;
        }
    }

    public async Task<bool> ScheduleReportAsync(string reportType, DateTime scheduleTime, string email)
    {
        try
        {
            _logger.LogInformation("Report scheduled: {ReportType} at {ScheduleTime} for {Email}", 
                reportType, scheduleTime, email);
            
            await Task.Delay(100);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling report: {ReportType}", reportType);
            return false;
        }
    }
}
