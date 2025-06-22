using System;
using Avalonia;
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
        InitializeDatabase();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>()
            };
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
        services.AddScoped<IEleicaoService, EleicaoService>();
        services.AddScoped<IVotoService, VotoService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();

        // Registrar ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<AdminViewModel>();
        services.AddTransient<VotacaoViewModel>();
        services.AddTransient<ResultadosViewModel>();
        services.AddTransient<AuditoriaViewModel>();
        services.AddTransient<MesaViewModel>();

        // Registrar Views
        services.AddTransient<AdminView>();
        services.AddTransient<VotacaoView>();
        services.AddTransient<ResultadosView>();
        services.AddTransient<AuditoriaView>();
        services.AddTransient<MesaView>();
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
}