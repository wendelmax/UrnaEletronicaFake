using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.UI.Extensions;
using UrnaEletronicaFake.UI.ViewModels;
using UrnaEletronicaFake.UI.Services;
using UrnaEletronicaFake.UI.Views;

namespace UrnaEletronicaFake.UI;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        // Configurar DI
        ConfigureServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            var mainWindowViewModel = _serviceProvider?.GetRequiredService<MainWindowViewModel>();
            var windowManagerService = _serviceProvider?.GetRequiredService<IWindowManagerService>();
            var notificationService = _serviceProvider?.GetRequiredService<INotificationService>();
            
            mainWindow.DataContext = mainWindowViewModel;
            
            if (windowManagerService != null && notificationService != null)
            {
                mainWindow.ConfigurarServicos(windowManagerService, notificationService);
            }
            
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // Configurar todos os serviços
        services.AddUrnaEletronicaFakeServices("Data Source=urna_eletronica.db");
        
        _serviceProvider = services.BuildServiceProvider();
        
        // Configurar event handlers
        // Com MediatR, os event handlers são registrados automaticamente via DI
        // Não precisamos mais de Subscribe manual
    }
}
