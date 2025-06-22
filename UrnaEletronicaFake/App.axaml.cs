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
        services.AddScoped<IEleicaoService, EleicaoService>();
        services.AddScoped<IVotoService, VotoService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();

        // Registrar ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<AdminViewModel>();
        services.AddTransient<VotacaoViewModel>();
        services.AddTransient<ResultadosViewModel>();
        services.AddTransient<AuditoriaViewModel>();

        // Registrar Views
        services.AddTransient<AdminView>();
        services.AddTransient<VotacaoView>();
        services.AddTransient<ResultadosView>();
        services.AddTransient<AuditoriaView>();
    }

    private void InitializeDatabase()
    {
        using var scope = _serviceProvider!.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UrnaDbContext>();
        
        // Criar banco de dados se não existir
        context.Database.EnsureCreated();
        
        // Inserir dados de exemplo se o banco estiver vazio
        if (!context.Eleicoes.Any())
        {
            SeedDatabase(context);
        }
    }

    private void SeedDatabase(UrnaDbContext context)
    {
        // Criar eleição de exemplo
        var eleicao = new Models.Eleicao
        {
            Titulo = "Eleição para Presidente da República",
            Descricao = "Eleição presidencial de 2024",
            DataInicio = DateTime.Now.AddDays(-1),
            DataFim = DateTime.Now.AddDays(30),
            Ativa = true,
            DataCriacao = DateTime.Now
        };

        context.Eleicoes.Add(eleicao);
        context.SaveChanges();

        // Criar candidatos de exemplo
        var candidatos = new[]
        {
            new Models.Candidato
            {
                Nome = "João Silva",
                Partido = "Partido A",
                Numero = "10",
                Biografia = "Candidato do Partido A",
                EleicaoId = eleicao.Id
            },
            new Models.Candidato
            {
                Nome = "Maria Santos",
                Partido = "Partido B",
                Numero = "20",
                Biografia = "Candidata do Partido B",
                EleicaoId = eleicao.Id
            },
            new Models.Candidato
            {
                Nome = "Pedro Costa",
                Partido = "Partido C",
                Numero = "30",
                Biografia = "Candidato do Partido C",
                EleicaoId = eleicao.Id
            }
        };

        context.Candidatos.AddRange(candidatos);
        context.SaveChanges();
    }
}