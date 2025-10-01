using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UrnaEletronicaFake.Web.Services;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.Web.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    protected readonly ILogger Logger;

    protected ViewModelBase(ILogger logger)
    {
        Logger = logger;
    }

    protected void LogInformation(string message)
    {
        Logger.LogInformation(message);
    }

    protected void LogError(Exception ex, string message)
    {
        Logger.LogError(ex, message);
    }
}

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IWebNavigationService _navigationService;

    public MainWindowViewModel(
        IWebNavigationService navigationService,
        ILogger<MainWindowViewModel> logger) : base(logger)
    {
        _navigationService = navigationService;
    }

    [RelayCommand]
    private void AbrirAdmin()
    {
        try
        {
            _navigationService.NavigateToAdmin();
            LogInformation("Navegando para painel administrativo");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao navegar para administração");
        }
    }

    [RelayCommand]
    private void AbrirDashboard()
    {
        try
        {
            _navigationService.NavigateToDashboard();
            LogInformation("Navegando para dashboard");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao navegar para dashboard");
        }
    }

    [RelayCommand]
    private void AbrirAuditoria()
    {
        try
        {
            _navigationService.NavigateToAuditoria();
            LogInformation("Navegando para auditoria");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao navegar para auditoria");
        }
    }

    [RelayCommand]
    private void AbrirResultados()
    {
        try
        {
            _navigationService.NavigateToResultados();
            LogInformation("Navegando para resultados");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao navegar para resultados");
        }
    }
}
