using System;
using Avalonia.Controls;
using UrnaEletronicaFake.Views;
using UrnaEletronicaFake.ViewModels;

namespace UrnaEletronicaFake.Services;

public interface IWindowManagerService
{
    void AbrirDashboardWindow(DashboardViewModel dashboardViewModel);
    void AbrirMesaWindow(MesaViewModel mesaViewModel);
    void AbrirVotacaoWindow(VotacaoViewModel votacaoViewModel);
    void FecharDashboardWindow();
    void FecharMesaWindow();
    void FecharVotacaoWindow();
    void FecharTodasAsJanelas();
    void TrazerDashboardWindowParaFrente();
    void TrazerMesaWindowParaFrente();
    void TrazerVotacaoWindowParaFrente();
    bool DashboardWindowEstaAberta { get; }
    bool MesaWindowEstaAberta { get; }
    bool VotacaoWindowEstaAberta { get; }
    event Action? OnDashboardWindowFechada;
    event Action? OnMesaWindowFechada;
    event Action? OnVotacaoWindowFechada;
}

public class WindowManagerService : IWindowManagerService
{
    private DashboardWindow? _dashboardWindow;
    private MesaWindow? _mesaWindow;
    private VotacaoWindow? _votacaoWindow;
    
    public bool DashboardWindowEstaAberta => _dashboardWindow != null;
    public bool MesaWindowEstaAberta => _mesaWindow != null;
    public bool VotacaoWindowEstaAberta => _votacaoWindow != null;
    
    public event Action? OnDashboardWindowFechada;
    public event Action? OnMesaWindowFechada;
    public event Action? OnVotacaoWindowFechada;
    
    public void AbrirDashboardWindow(DashboardViewModel dashboardViewModel)
    {
        if (_dashboardWindow != null) return;
        
        _dashboardWindow = new DashboardWindow(dashboardViewModel);
        _dashboardWindow.Closed += (s, e) =>
        {
            _dashboardWindow = null;
            OnDashboardWindowFechada?.Invoke();
        };
        
        _dashboardWindow.Show();
        _dashboardWindow.Activate();
    }
    
    public void AbrirMesaWindow(MesaViewModel mesaViewModel)
    {
        if (_mesaWindow != null) return;
        
        _mesaWindow = new MesaWindow(mesaViewModel);
        _mesaWindow.Closed += (s, e) =>
        {
            _mesaWindow = null;
            OnMesaWindowFechada?.Invoke();
        };
        
        _mesaWindow.Show();
        _mesaWindow.Activate();
    }
    
    public void AbrirVotacaoWindow(VotacaoViewModel votacaoViewModel)
    {
        if (_votacaoWindow != null) return;
        
        _votacaoWindow = new VotacaoWindow(votacaoViewModel);
        _votacaoWindow.Closed += (s, e) =>
        {
            _votacaoWindow = null;
            OnVotacaoWindowFechada?.Invoke();
        };
        
        _votacaoWindow.Show();
        _votacaoWindow.Activate();
    }
    
    public void FecharMesaWindow()
    {
        _mesaWindow?.Close();
    }
    
    public void FecharVotacaoWindow()
    {
        _votacaoWindow?.Close();
    }
    
    public void FecharDashboardWindow()
    {
        _dashboardWindow?.Close();
    }
    
    public void FecharTodasAsJanelas()
    {
        FecharDashboardWindow();
        FecharMesaWindow();
        FecharVotacaoWindow();
    }
    
    public void TrazerDashboardWindowParaFrente()
    {
        if (_dashboardWindow != null)
        {
            _dashboardWindow.Activate();
            _dashboardWindow.BringIntoView();
        }
    }
    
    public void TrazerMesaWindowParaFrente()
    {
        if (_mesaWindow != null)
        {
            _mesaWindow.Activate();
            _mesaWindow.BringIntoView();
        }
    }
    
    public void TrazerVotacaoWindowParaFrente()
    {
        if (_votacaoWindow != null)
        {
            _votacaoWindow.Activate();
            _votacaoWindow.BringIntoView();
        }
    }
}
