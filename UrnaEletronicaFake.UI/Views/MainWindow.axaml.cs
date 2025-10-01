using Avalonia.Controls;
using UrnaEletronicaFake.UI.ViewModels;
using UrnaEletronicaFake.UI.Services;
using UrnaEletronicaFake.UI.Components;
using System;
using Avalonia;

namespace UrnaEletronicaFake.UI.Views;

public partial class MainWindow : Window
{
    private IWindowManagerService? _windowManagerService;
    private INotificationService? _notificationService;

    public MainWindow()
    {
        InitializeComponent();
        this.PropertyChanged += MainWindow_PropertyChanged;
        if (DataContext != null)
        {
            ConectarDataContext();
        }
    }
    
    public void ConfigurarServicos(IWindowManagerService windowManagerService, INotificationService notificationService)
    {
        _windowManagerService = windowManagerService;
        _notificationService = notificationService;
        
        if (NotificationOverlay != null)
        {
            NotificationOverlay.Initialize(notificationService);
        }
    }
    
    public void ConfigurarWindowManager(IWindowManagerService windowManagerService)
    {
        _windowManagerService = windowManagerService;
    }

    private void MainWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == DataContextProperty && DataContext != null)
        {
            ConectarDataContext();
        }
    }

    private void ConectarDataContext()
    {
        if (DataContext is MainWindowViewModel vm)
        {
            // conectar bindings específicos, se necessário
        }
    }

    private void OnDashboardWindowFechada()
    {
        if (DataContext is MainWindowViewModel mainVm)
        {
            mainVm.AtualizarStatusDashboardWindow();
        }
    }
    
    private void OnMesaWindowFechada()
    {
        if (DataContext is MainWindowViewModel mainVm)
        {
            mainVm.AtualizarStatusMesaWindow();
        }
    }
    
    private void OnVotacaoWindowFechada()
    {
        if (DataContext is MainWindowViewModel mainVm)
        {
            mainVm.AtualizarStatusVotacaoWindow();
        }
    }
}