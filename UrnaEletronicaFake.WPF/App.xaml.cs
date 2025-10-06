using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Core.Extensions;
using UrnaEletronicaFake.Data.Extensions;
using UrnaEletronicaFake.Administration.Extensions;
using UrnaEletronicaFake.Dashboard.Extensions;
using UrnaEletronicaFake.Voting.Extensions;
using UrnaEletronicaFake.Mesa.Extensions;
using UrnaEletronicaFake.Audit.Extensions;
using UrnaEletronicaFake.WPF.Extensions;
using UrnaEletronicaFake.WPF.ViewModels;
using System.IO;

namespace UrnaEletronicaFake.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        // Configurar o host com DI seguindo a arquitetura em camadas
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Configurar banco de dados
                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "urna_eletronica.db");
                var connectionString = $"Data Source={dbPath}";
                
                // Registrar módulos seguindo a arquitetura
                services.AddDataServices(connectionString);
                services.AddCoreServices();
                services.AddAdministrationServices();
                services.AddDashboardServices();
                services.AddVotingServices();
                services.AddMesaServices();
                services.AddAuditServices();
                services.AddWPFServices();
                
                // Registrar janelas
                services.AddTransient<MainWindow>();
                services.AddTransient<Views.VotacaoWindow>();
                services.AddTransient<Views.MesaWindow>();
                services.AddTransient<Views.DashboardWindow>();
                services.AddTransient<Views.AdminWindow>();
            })
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            })
            .Build();

        // Inicializar banco de dados
        using (var scope = _host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<UrnaEletronicaFake.Data.DbContext.UrnaDbContext>();
            await context.Database.EnsureCreatedAsync();
        }

        // Inicializar a janela principal
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        var mainWindowViewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
        mainWindow.DataContext = mainWindowViewModel;
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        base.OnExit(e);
    }
}

