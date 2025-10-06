using System.Windows.Input;
using UrnaEletronicaFake.Dashboard.Services;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.WPF.ViewModels;

public class DashboardWindowViewModel : ViewModelBase
{
    private readonly IDashboardService _dashboardService;
    private DashboardOverview _overview = new();
    private ElectionStatistics _electionStats = new();
    private TerminalStatusSummary _terminalStatus = new();
    private VotingProgress _votingProgress = new();
    private SystemHealthStatus _systemHealth = new();
    private bool _isLoading = false;
    private DateTime _lastUpdate = DateTime.Now;

    public DashboardOverview Overview
    {
        get => _overview;
        set => SetProperty(ref _overview, value);
    }

    public ElectionStatistics ElectionStats
    {
        get => _electionStats;
        set => SetProperty(ref _electionStats, value);
    }

    public TerminalStatusSummary TerminalStatus
    {
        get => _terminalStatus;
        set => SetProperty(ref _terminalStatus, value);
    }

    public VotingProgress VotingProgress
    {
        get => _votingProgress;
        set => SetProperty(ref _votingProgress, value);
    }

    public SystemHealthStatus SystemHealth
    {
        get => _systemHealth;
        set => SetProperty(ref _systemHealth, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public DateTime LastUpdate
    {
        get => _lastUpdate;
        set => SetProperty(ref _lastUpdate, value);
    }

    public ICommand RefreshDataCommand { get; }

    public DashboardWindowViewModel(
        IDashboardService dashboardService,
        ILogger<DashboardWindowViewModel> logger) : base(logger)
    {
        _dashboardService = dashboardService;
        RefreshDataCommand = new RelayCommand(async () => await RefreshDataAsync());
        
        // Carregar dados iniciais
        _ = Task.Run(RefreshDataAsync);
    }

    private async Task RefreshDataAsync()
    {
        try
        {
            IsLoading = true;
            LogInformation("Atualizando dados do dashboard...");

            // Carregar dados em paralelo
            var tasks = new Task[]
            {
                _dashboardService.GetDashboardOverviewAsync().ContinueWith(t => Overview = t.Result),
                _dashboardService.GetElectionStatisticsAsync().ContinueWith(t => ElectionStats = t.Result),
                _dashboardService.GetTerminalStatusSummaryAsync().ContinueWith(t => TerminalStatus = t.Result),
                _dashboardService.GetVotingProgressAsync().ContinueWith(t => VotingProgress = t.Result),
                _dashboardService.GetSystemHealthStatusAsync().ContinueWith(t => SystemHealth = t.Result)
            };

            await Task.WhenAll(tasks);

            LastUpdate = DateTime.Now;
            LogInformation("Dados do dashboard atualizados com sucesso");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao atualizar dados do dashboard");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
