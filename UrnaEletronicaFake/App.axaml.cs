using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using UrnaEletronicaFake.Data;
using UrnaEletronicaFake.Services;
using UrnaEletronicaFake.ViewModels;
using UrnaEletronicaFake.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UrnaEletronicaFake;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Configurar injeção de dependências
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Inicializar banco de dados
        try
        {
            InitializeDatabase();
            Console.WriteLine("[INFO] Banco de dados inicializado com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Falha na inicialização do banco de dados: {ex.Message}");
            throw;
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            
            try
            {
                Console.WriteLine("[INFO] Criando MainWindow...");
                var mainWindow = new MainWindow
                {
                    DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>()
                };
                
                // Configurar WindowManagerService na MainWindow
                Console.WriteLine("[INFO] Configurando WindowManagerService...");
                var windowManagerService = _serviceProvider.GetRequiredService<IWindowManagerService>();
                mainWindow.ConfigurarWindowManager(windowManagerService);
                
                desktop.MainWindow = mainWindow;
                Console.WriteLine("[INFO] MainWindow configurada como janela principal");
                
                // Abrir janelas separadas automaticamente
                Console.WriteLine("[INFO] Abrindo janelas separadas...");
                AbrirJanelasSeparadas(windowManagerService);
                Console.WriteLine("[INFO] Aplicativo iniciado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha na criação da MainWindow: {ex.Message}");
                Console.WriteLine($"[ERRO] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Configurar Entity Framework
        services.AddDbContext<UrnaDbContext>(options =>
            options.UseSqlite("Data Source=urna_eletronica.db"));

        // Registrar serviços
        services.AddSingleton<IVotacaoStateService, VotacaoStateService>();
        services.AddSingleton<ITerminalLogService, TerminalLogService>();
        services.AddSingleton<IWindowManagerService, WindowManagerService>();
        services.AddScoped<IEleicaoService, EleicaoService>();
        services.AddScoped<IVotoService, VotoService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();

        // Registrar ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<AdminViewModel>();
        services.AddTransient<VotacaoViewModel>();
        services.AddTransient<ResultadosViewModel>();
        services.AddTransient<AuditoriaViewModel>();
        services.AddTransient<MesaViewModel>();

        // Registrar Views
        services.AddTransient<DashboardView>();
        services.AddTransient<DashboardWindow>();
        services.AddTransient<AdminView>();
        services.AddTransient<VotacaoView>();
        services.AddTransient<VotacaoWindow>();
        services.AddTransient<ResultadosView>();
        services.AddTransient<AuditoriaView>();
        services.AddTransient<MesaView>();
        services.AddTransient<MesaWindow>();
    }

    private void InitializeDatabase()
    {
        try
        {
            Console.WriteLine("[INFO] Iniciando inicialização do banco de dados...");
            using var scope = _serviceProvider!.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UrnaDbContext>();
            
            Console.WriteLine("[INFO] Deletando banco existente (se houver)...");
            context.Database.EnsureDeleted();
            
            Console.WriteLine("[INFO] Criando novo banco de dados...");
            context.Database.EnsureCreated();
            
            Console.WriteLine("[INFO] Banco de dados criado com sucesso!");
            
            // Verificar se os dados foram criados
            var eleicoes = context.Eleicoes.ToList();
            var cargos = context.CargosEleitorais.ToList();
            var candidatos = context.Candidatos.ToList();
            
            Console.WriteLine($"[INFO] Dados criados: {eleicoes.Count} eleições, {cargos.Count} cargos, {candidatos.Count} candidatos");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Erro na inicialização do banco de dados: {ex.Message}");
            Console.WriteLine($"[ERRO] Stack trace: {ex.StackTrace}");
        }
    }
    
    private void AbrirJanelasSeparadas(IWindowManagerService windowManagerService)
    {
        try
        {
            // Aguardar um pouco para a MainWindow carregar completamente
            Task.Delay(1000).ContinueWith(_ =>
            {
                // Abrir Dashboard
                var dashboardViewModel = _serviceProvider.GetRequiredService<DashboardViewModel>();
                windowManagerService.AbrirDashboardWindow(dashboardViewModel);
                
                // Aguardar um pouco entre as aberturas
                Task.Delay(500).ContinueWith(__ =>
                {
                    // Abrir Mesa
                    var mesaViewModel = _serviceProvider.GetRequiredService<MesaViewModel>();
                    windowManagerService.AbrirMesaWindow(mesaViewModel);
                    
                    // Aguardar um pouco entre as aberturas
                    Task.Delay(500).ContinueWith(___ =>
                    {
                        // Abrir Urna
                        var votacaoViewModel = _serviceProvider.GetRequiredService<VotacaoViewModel>();
                        windowManagerService.AbrirVotacaoWindow(votacaoViewModel);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Erro ao abrir janelas separadas: {ex.Message}");
        }
    }
}