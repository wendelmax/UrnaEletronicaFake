using Microsoft.Extensions.Logging;
using Avalonia.Controls;
using System.Collections.Generic;
using UrnaEletronicaFake.UI.Views;

namespace UrnaEletronicaFake.UI.Services;

public class WindowManagerService : IWindowManagerService
{
    private readonly ILogger<WindowManagerService> _logger;
    private readonly Dictionary<Type, Window> _openWindows = new();
    private Window? _mainWindow;

    public WindowManagerService(ILogger<WindowManagerService> logger)
    {
        _logger = logger;
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
        _logger.LogWarning("AuditWindow view not implemented yet.");
    }

    public void ShowResultsWindow()
    {
        _logger.LogWarning("ResultsWindow view not implemented yet.");
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