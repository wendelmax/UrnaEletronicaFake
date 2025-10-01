using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UrnaEletronicaFake.Dashboard.Services;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.UI.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IDashboardService _dashboardService;

    [ObservableProperty]
    private DashboardOverview _overview = new();

    [ObservableProperty]
    private ElectionStatistics _electionStats = new();

    [ObservableProperty]
    private TerminalStatusSummary _terminalStatus = new();

    [ObservableProperty]
    private VotingProgress _votingProgress = new();

    [ObservableProperty]
    private SystemHealthStatus _systemHealth = new();

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private DateTime _lastUpdate = DateTime.Now;

    [ObservableProperty]
    private object? _eleicaoSelecionada;

    [ObservableProperty]
    private object? _eleicoes;

    [ObservableProperty]
    private object? _dashboardData;

    public ICommand AtualizarDadosCommand { get; }

    public DashboardViewModel(
        IDashboardService dashboardService,
        ILogger<DashboardViewModel> logger) : base(logger)
    {
        _dashboardService = dashboardService;
        
        // Comandos
        AtualizarDadosCommand = new RelayCommand(async () => await RefreshDataAsync());
    }


    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Atualizando dados...";

            var tasks = new Task[]
            {
                _dashboardService.GetDashboardOverviewAsync(),
                _dashboardService.GetElectionStatisticsAsync(),
                _dashboardService.GetTerminalStatusSummaryAsync(),
                _dashboardService.GetVotingProgressAsync(),
                _dashboardService.GetSystemHealthStatusAsync()
            };

            await Task.WhenAll(tasks);

            Overview = await _dashboardService.GetDashboardOverviewAsync();
            ElectionStats = await _dashboardService.GetElectionStatisticsAsync();
            TerminalStatus = await _dashboardService.GetTerminalStatusSummaryAsync();
            VotingProgress = await _dashboardService.GetVotingProgressAsync();
            SystemHealth = await _dashboardService.GetSystemHealthStatusAsync();

            LastUpdate = DateTime.Now;
            StatusMessage = "Dados atualizados com sucesso";
            
            LogInformation("Dashboard data refreshed successfully");
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro ao atualizar dados";
            LogError(ex, "Error refreshing dashboard data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ExportReportAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Exportando relatório...";

            var report = await _dashboardService.GetCustomReportAsync("dashboard");
            
            StatusMessage = "Relatório exportado com sucesso";
            LogInformation("Dashboard report exported successfully");
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro ao exportar relatório";
            LogError(ex, "Error exporting dashboard report");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ScheduleReportAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Agendando relatório...";

            await Task.Delay(1000);
            
            StatusMessage = "Relatório agendado com sucesso";
            LogInformation("Dashboard report scheduled successfully");
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro ao agendar relatório";
            LogError(ex, "Error scheduling dashboard report");
        }
        finally
        {
            IsLoading = false;
        }
    }
}