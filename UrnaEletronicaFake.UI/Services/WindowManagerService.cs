using Microsoft.Extensions.Logging;
using Avalonia.Controls;
using System.Collections.Generic;
using UrnaEletronicaFake.UI.Views;
using UrnaEletronicaFake.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace UrnaEletronicaFake.UI.Services;

public class WindowManagerService : IWindowManagerService
{
    private readonly ILogger<WindowManagerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, Window> _openWindows = new();
    private Window? _mainWindow;

    public WindowManagerService(ILogger<WindowManagerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public void ShowMainWindow()
    {
        try
        {
            var window = GetWindow<MainWindow>();
            if (window == null)
            {
                window = new MainWindow();
                _openWindows[typeof(MainWindow)] = window;
                _logger.LogInformation("Main window created and shown");
            }
            else
            {
                window.Show();
                window.Activate();
                _logger.LogDebug("Main window shown and activated");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing main window");
        }
    }

    public void ShowVotingWindow()
    {
        try
        {
            var window = GetWindow<VotacaoWindow>();
            if (window == null)
            {
                window = new VotacaoWindow();
                var viewModel = _serviceProvider.GetRequiredService<VotacaoViewModel>();
                window.DataContext = viewModel;
                _openWindows[typeof(VotacaoWindow)] = window;
                _logger.LogInformation("Voting window created and shown");
            }
            else
            {
                window.Show();
                window.Activate();
                _logger.LogDebug("Voting window shown and activated");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing voting window");
        }
    }

    public void ShowMesaWindow()
    {
        try
        {
            var window = GetWindow<MesaWindow>();
            if (window == null)
            {
                window = new MesaWindow();
                var viewModel = _serviceProvider.GetRequiredService<MesaViewModel>();
                window.DataContext = viewModel;
                _openWindows[typeof(MesaWindow)] = window;
                _logger.LogInformation("Mesa window created and shown");
            }
            else
            {
                window.Show();
                window.Activate();
                _logger.LogDebug("Mesa window shown and activated");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing mesa window");
        }
    }

    public void ShowDashboardWindow()
    {
        try
        {
            var window = GetWindow<DashboardWindow>();
            if (window == null)
            {
                window = new DashboardWindow();
                var viewModel = _serviceProvider.GetRequiredService<DashboardViewModel>();
                window.DataContext = viewModel;
                _openWindows[typeof(DashboardWindow)] = window;
                _logger.LogInformation("Dashboard window created and shown");
            }
            else
            {
                window.Show();
                window.Activate();
                _logger.LogDebug("Dashboard window shown and activated");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing dashboard window");
        }
    }

    public void ShowAdminWindow()
    {
        try
        {
            var window = GetWindow<AdminWindow>();
            if (window == null)
            {
                window = new AdminWindow();
                var viewModel = _serviceProvider.GetRequiredService<AdminViewModel>();
                window.DataContext = viewModel;
                _openWindows[typeof(AdminWindow)] = window;
                _logger.LogInformation("Admin window created and shown");
            }
            else
            {
                window.Show();
                window.Activate();
                _logger.LogDebug("Admin window shown and activated");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing admin window");
        }
    }

    public void ShowAuditWindow()
    {
        try
        {
            var window = GetWindow<AuditWindow>();
            if (window == null)
            {
                window = new AuditWindow();
                var viewModel = _serviceProvider.GetRequiredService<AuditoriaViewModel>();
                window.DataContext = viewModel;
                _openWindows[typeof(AuditWindow)] = window;
                _logger.LogInformation("Audit window created and shown");
            }
            else
            {
                window.Show();
                window.Activate();
                _logger.LogDebug("Audit window shown and activated");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing audit window");
        }
    }

    public void ShowResultsWindow()
    {
        try
        {
            var window = GetWindow<ResultadosWindow>();
            if (window == null)
            {
                window = new ResultadosWindow();
                var viewModel = _serviceProvider.GetRequiredService<ResultadosViewModel>();
                window.DataContext = viewModel;
                _openWindows[typeof(ResultadosWindow)] = window;
                _logger.LogInformation("Results window created and shown");
            }
            else
            {
                window.Show();
                window.Activate();
                _logger.LogDebug("Results window shown and activated");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing results window");
        }
    }

    public void CloseAllWindows()
    {
        try
        {
            var windowsToClose = _openWindows.Values.ToList();
            foreach (var window in windowsToClose)
            {
                window.Close();
            }
            _openWindows.Clear();
            _logger.LogInformation("All windows closed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing all windows");
        }
    }

    public void MinimizeAllWindows()
    {
        try
        {
            foreach (var window in _openWindows.Values)
            {
                if (window.WindowState != WindowState.Minimized)
                {
                    window.WindowState = WindowState.Minimized;
                }
            }
            _logger.LogDebug("All windows minimized");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error minimizing all windows");
        }
    }

    public void RestoreAllWindows()
    {
        try
        {
            foreach (var window in _openWindows.Values)
            {
                if (window.WindowState == WindowState.Minimized)
                {
                    window.WindowState = WindowState.Normal;
                }
                window.Activate();
            }
            _logger.LogDebug("All windows restored");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring all windows");
        }
    }

    public bool IsWindowOpen<T>() where T : Window
    {
        return _openWindows.ContainsKey(typeof(T));
    }

    public T? GetWindow<T>() where T : Window
    {
        _openWindows.TryGetValue(typeof(T), out var window);
        return window as T;
    }

    private Window? GetWindow(Type type)
    {
        _openWindows.TryGetValue(type, out var window);
        return window;
    }

    public void SetMainWindow(Window mainWindow)
    {
        _mainWindow = mainWindow;
        _logger.LogInformation("Main window set");
    }

    public Window? GetMainWindow()
    {
        return _mainWindow;
    }

    public void OnDashboardWindowFechada()
    {
        _logger.LogInformation("Dashboard window closed");
    }

    public void OnMesaWindowFechada()
    {
        _logger.LogInformation("Mesa window closed");
    }

    public void OnVotacaoWindowFechada()
    {
        _logger.LogInformation("Votacao window closed");
    }
}