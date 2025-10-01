using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Core.Extensions;
using UrnaEletronicaFake.Data.Extensions;
using UrnaEletronicaFake.Voting.Extensions;
using UrnaEletronicaFake.Mesa.Extensions;
using UrnaEletronicaFake.Audit.Extensions;
using UrnaEletronicaFake.Administration.Extensions;
using UrnaEletronicaFake.Dashboard.Extensions;
using UrnaEletronicaFake.Web.Services;
using UrnaEletronicaFake.Web.ViewModels;
using UrnaEletronicaFake.Web.Views;

namespace UrnaEletronicaFake.Web;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        // Configurar serviços
        ConfigureServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var mainViewModel = _serviceProvider?.GetService<MainWindowViewModel>();
            singleViewPlatform.MainView = new MainWindow
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configurar logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        // Configurar banco de dados (usando SQLite em memória para web)
        var connectionString = "Data Source=:memory:";
        services.AddDataServices(connectionString);

        // Configurar módulos
        services.AddCoreServices();
        services.AddVotingServices();
        services.AddMesaServices();
        services.AddAuditServices();
        services.AddAdministrationServices();
        services.AddDashboardServices();

        // Configurar serviços web específicos
        services.AddSingleton<IWebNavigationService, WebNavigationService>();
        services.AddSingleton<IWebDialogService, WebDialogService>();

        // Configurar ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<AdminViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<MesaViewModel>();
        services.AddTransient<VotacaoViewModel>();
        services.AddTransient<AuditoriaViewModel>();
        services.AddTransient<ResultadosViewModel>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(sender, e);
    }
}
