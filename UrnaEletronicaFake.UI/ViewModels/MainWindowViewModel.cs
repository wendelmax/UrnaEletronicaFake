using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using UrnaEletronicaFake.UI.Services;
using UrnaEletronicaFake.UI.Views;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IWindowManagerService _windowManager;
    private object? _currentView = null;

    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public bool MesaWindowEstaAberta => _windowManager.IsWindowOpen<MesaWindow>();
    public bool DashboardWindowEstaAberta => _windowManager.IsWindowOpen<DashboardWindow>();
    public bool VotacaoWindowEstaAberta => _windowManager.IsWindowOpen<VotacaoWindow>();

    private bool _isLoading = false;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _eleicaoAtiva = false;
    public bool EleicaoAtiva
    {
        get => _eleicaoAtiva;
        set => SetProperty(ref _eleicaoAtiva, value);
    }

    private DateTime _ultimaAtualizacao = DateTime.Now;
    public DateTime UltimaAtualizacao
    {
        get => _ultimaAtualizacao;
        set => SetProperty(ref _ultimaAtualizacao, value);
    }

    public MainWindowViewModel(
        IWindowManagerService windowManager,
        ILogger<MainWindowViewModel> logger) : base(logger)
    {
        _windowManager = windowManager;
        
        // Simular atualização periódica
        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += (s, e) => UltimaAtualizacao = DateTime.Now;
        timer.Start();
    }

    [RelayCommand]
    private void IniciarEleicao()
    {
        try
        {
            EleicaoAtiva = true;
            LogInformation("Eleição iniciada");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao iniciar eleição");
        }
    }

    [RelayCommand]
    private void PausarEleicao()
    {
        try
        {
            EleicaoAtiva = false;
            LogInformation("Eleição pausada");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao pausar eleição");
        }
    }

    [RelayCommand]
    private void FinalizarEleicao()
    {
        try
        {
            EleicaoAtiva = false;
            LogInformation("Eleição finalizada");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao finalizar eleição");
        }
    }

    [RelayCommand]
    private void AbrirVotacaoWindow()
    {
        try
        {
            _windowManager.ShowVotingWindow();
            LogInformation("Voting window requested");
        }
        catch (Exception ex)
        {
            LogError(ex, "Error showing voting window");
        }
    }

    [RelayCommand]
    private void AbrirMesaWindow()
    {
        try
        {
            _windowManager.ShowMesaWindow();
            LogInformation("Mesa window requested");
        }
        catch (Exception ex)
        {
            LogError(ex, "Error showing mesa window");
        }
    }

    [RelayCommand]
    private void AbrirDashboardWindow()
    {
        try
        {
            _windowManager.ShowDashboardWindow();
            LogInformation("Dashboard window requested");
        }
        catch (Exception ex)
        {
            LogError(ex, "Error showing dashboard window");
        }
    }

    [RelayCommand]
    private void AbrirAdminWindow()
    {
        try
        {
            _windowManager.ShowAdminWindow();
            LogInformation("Admin window requested");
        }
        catch (Exception ex)
        {
            LogError(ex, "Error showing admin window");
        }
    }

    [RelayCommand]
    private void ShowAdmin()
    {
        try
        {
            _windowManager.ShowAdminWindow();
            LogInformation("Admin window requested");
        }
        catch (Exception ex)
        {
            LogError(ex, "Error showing admin window");
        }
    }

    [RelayCommand]
    private void ShowAudit()
    {
        try
        {
            _windowManager.ShowAuditWindow();
            LogInformation("Audit window requested");
        }
        catch (Exception ex)
        {
            LogError(ex, "Error showing audit window");
        }
    }

    [RelayCommand]
    private void ShowResultados()
    {
        try
        {
            _windowManager.ShowResultsWindow();
            LogInformation("Results window requested");
        }
        catch (Exception ex)
        {
            LogError(ex, "Error showing results window");
        }
    }

    [RelayCommand]
    private void VoltarAoControle()
    {
        try
        {
            CurrentView = null;
            LogInformation("Returned to control panel");
        }
        catch (Exception ex)
        {
            LogError(ex, "Error returning to control panel");
        }
    }

    [RelayCommand]
    private void TrazerMesaWindow()
    {
        try
        {
            var window = _windowManager.GetWindow<MesaWindow>();
            if (window != null)
            {
                window.Show();
                window.Activate();
                LogInformation("Mesa window brought to front");
            }
            else
            {
                _windowManager.ShowMesaWindow();
                LogInformation("Mesa window opened");
            }
        }
        catch (Exception ex)
        {
            LogError(ex, "Error bringing mesa window to front");
        }
    }

    [RelayCommand]
    private void TrazerDashboardWindow()
    {
        try
        {
            var window = _windowManager.GetWindow<DashboardWindow>();
            if (window != null)
            {
                window.Show();
                window.Activate();
                LogInformation("Dashboard window brought to front");
            }
            else
            {
                _windowManager.ShowDashboardWindow();
                LogInformation("Dashboard window opened");
            }
        }
        catch (Exception ex)
        {
            LogError(ex, "Error bringing dashboard window to front");
        }
    }

    [RelayCommand]
    private void TrazerVotacaoWindow()
    {
        try
        {
            var window = _windowManager.GetWindow<VotacaoWindow>();
            if (window != null)
            {
                window.Show();
                window.Activate();
                LogInformation("Voting window brought to front");
            }
            else
            {
                _windowManager.ShowVotingWindow();
                LogInformation("Voting window opened");
            }
        }
        catch (Exception ex)
        {
            LogError(ex, "Error bringing voting window to front");
        }
    }

    [RelayCommand]
    private void FecharTodasJanelas()
    {
        try
        {
            _windowManager.CloseAllWindows();
            LogInformation("All windows closed");
        }
        catch (Exception ex)
        {
            LogError(ex, "Error closing all windows");
        }
    }

    public void AtualizarStatusDashboardWindow()
    {
        LogInformation("Dashboard window status updated");
    }

    public void AtualizarStatusMesaWindow()
    {
        LogInformation("Mesa window status updated");
    }

    public void AtualizarStatusVotacaoWindow()
    {
        LogInformation("Votacao window status updated");
    }
}