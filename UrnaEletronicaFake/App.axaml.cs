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
            
            Console.WriteLine("[INFO] Criando banco de dados se não existir...");
            // Criar banco de dados se não existir
            context.Database.EnsureCreated();
            Console.WriteLine("[INFO] Banco de dados criado/verificado com sucesso.");
            
            // Inserir dados de exemplo se o banco estiver vazio
            if (!context.Eleicoes.Any())
            {
                Console.WriteLine("[INFO] Banco vazio, inserindo dados de exemplo...");
                SeedDatabase(context);
                Console.WriteLine("[INFO] Dados de exemplo inseridos com sucesso.");
            }
            else
            {
                Console.WriteLine($"[INFO] Banco já contém {context.Eleicoes.Count()} eleições.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Erro na inicialização do banco de dados: {ex.Message}");
            Console.WriteLine($"[ERRO] Stack trace: {ex.StackTrace}");
        }
    }

    private void SeedDatabase(UrnaDbContext context)
    {
        try
        {
            Console.WriteLine("[INFO] Criando eleição de exemplo...");
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
            Console.WriteLine($"[INFO] Eleição criada com ID: {eleicao.Id}");

            Console.WriteLine("[INFO] Criando cargos eleitorais...");
            // Criar cargos eleitorais
            var cargoPresidente = new Models.CargoEleitoral
            {
                Nome = "Presidente",
                QuantidadeDigitos = 2,
                Ordem = 1,
                Ativo = true,
                EleicaoId = eleicao.Id
            };

            context.CargosEleitorais.Add(cargoPresidente);
            context.SaveChanges();
            Console.WriteLine($"[INFO] Cargo criado com ID: {cargoPresidente.Id}");

            Console.WriteLine("[INFO] Criando candidatos de exemplo...");
            // Criar candidatos de exemplo
            var candidatos = new[]
            {
                new Models.Candidato
                {
                    Nome = "João Silva",
                    Partido = "Partido A",
                    Numero = "10",
                    Biografia = "Candidato do Partido A",
                    EleicaoId = eleicao.Id,
                    CargoEleitoralId = cargoPresidente.Id
                },
                new Models.Candidato
                {
                    Nome = "Maria Santos",
                    Partido = "Partido B",
                    Numero = "20",
                    Biografia = "Candidata do Partido B",
                    EleicaoId = eleicao.Id,
                    CargoEleitoralId = cargoPresidente.Id
                },
                new Models.Candidato
                {
                    Nome = "Pedro Costa",
                    Partido = "Partido C",
                    Numero = "30",
                    Biografia = "Candidato do Partido C",
                    EleicaoId = eleicao.Id,
                    CargoEleitoralId = cargoPresidente.Id
                }
            };

            context.Candidatos.AddRange(candidatos);
            context.SaveChanges();
            Console.WriteLine($"[INFO] {candidatos.Length} candidatos criados com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Erro ao inserir dados de exemplo: {ex.Message}");
            Console.WriteLine($"[ERRO] Stack trace: {ex.StackTrace}");
        }
    }
}