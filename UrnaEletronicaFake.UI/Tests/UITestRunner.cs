using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.UI.Services;
using UrnaEletronicaFake.UI.ViewModels;

namespace UrnaEletronicaFake.UI.Tests;

public class UITestRunner
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UITestRunner> _logger;

    public UITestRunner(IServiceProvider serviceProvider, ILogger<UITestRunner> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task RunAllTestsAsync()
    {
        _logger.LogInformation("Iniciando testes da UI...");

        try
        {
            await TestMainWindowViewModel();
            await TestVotacaoViewModel();
            await TestMesaViewModel();
            await TestDashboardViewModel();
            await TestAdminViewModel();

            _logger.LogInformation("Todos os testes da UI foram executados com sucesso!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a execução dos testes da UI");
            throw;
        }
    }

    private Task TestMainWindowViewModel()
    {
        _logger.LogInformation("Testando MainWindowViewModel...");

        var viewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();

        // Teste de inicialização
        if (viewModel == null)
            throw new Exception("MainWindowViewModel não foi criado corretamente");

        // Teste de comandos
        if (viewModel.IniciarEleicaoCommand == null)
            throw new Exception("Comando IniciarEleicaoCommand não foi criado");

        if (viewModel.PausarEleicaoCommand == null)
            throw new Exception("Comando PausarEleicaoCommand não foi criado");

        if (viewModel.FinalizarEleicaoCommand == null)
            throw new Exception("Comando FinalizarEleicaoCommand não foi criado");

        if (viewModel.AbrirVotacaoWindowCommand == null)
            throw new Exception("Comando AbrirVotacaoWindowCommand não foi criado");

        if (viewModel.AbrirMesaWindowCommand == null)
            throw new Exception("Comando AbrirMesaWindowCommand não foi criado");

        if (viewModel.AbrirDashboardWindowCommand == null)
            throw new Exception("Comando AbrirDashboardWindowCommand não foi criado");

        if (viewModel.AbrirAdminWindowCommand == null)
            throw new Exception("Comando AbrirAdminWindowCommand não foi criado");

        // Teste de propriedades
        // EleicaoAtiva é bool, não bool?, então não precisa verificar null
        // A propriedade será false por padrão se não foi inicializada

        if (viewModel.UltimaAtualizacao == default)
            throw new Exception("Propriedade UltimaAtualizacao não foi inicializada");

        _logger.LogInformation("MainWindowViewModel testado com sucesso!");
        return Task.CompletedTask;
    }

    private Task TestVotacaoViewModel()
    {
        _logger.LogInformation("Testando VotacaoViewModel...");

        var viewModel = _serviceProvider.GetRequiredService<VotacaoViewModel>();

        // Teste de inicialização
        if (viewModel == null)
            throw new Exception("VotacaoViewModel não foi criado corretamente");

        // Teste de comandos
        if (viewModel.ConfirmarCommand == null)
            throw new Exception("Comando ConfirmarCommand não foi criado");

        if (viewModel.DigitarNumeroCommand == null)
            throw new Exception("Comando DigitarNumeroCommand não foi criado");

        if (viewModel.VotarBrancoCommand == null)
            throw new Exception("Comando VotarBrancoCommand não foi criado");

        if (viewModel.CorrigirCommand == null)
            throw new Exception("Comando CorrigirCommand não foi criado");

        // Teste de propriedades
        if (viewModel.Cargo == null)
            throw new Exception("Propriedade Cargo não foi inicializada");

        if (viewModel.Instrucoes == null)
            throw new Exception("Propriedade Instrucoes não foi inicializada");

        _logger.LogInformation("VotacaoViewModel testado com sucesso!");
        return Task.CompletedTask;
    }

    private Task TestMesaViewModel()
    {
        _logger.LogInformation("Testando MesaViewModel...");

        var viewModel = _serviceProvider.GetRequiredService<MesaViewModel>();

        // Teste de inicialização
        if (viewModel == null)
            throw new Exception("MesaViewModel não foi criado corretamente");

        // Teste de comandos
        if (viewModel.LiberarUrnaCommand == null)
            throw new Exception("Comando LiberarUrnaCommand não foi criado");

        if (viewModel.BloquearUrnaCommand == null)
            throw new Exception("Comando BloquearUrnaCommand não foi criado");

        if (viewModel.LimparLogCommand == null)
            throw new Exception("Comando LimparLogCommand não foi criado");

        // Teste de propriedades
        if (viewModel.StatusMessage == null)
            throw new Exception("Propriedade StatusMessage não foi inicializada");

        if (viewModel.TerminalId == null)
            throw new Exception("Propriedade TerminalId não foi inicializada");

        _logger.LogInformation("MesaViewModel testado com sucesso!");
        return Task.CompletedTask;
    }

    private Task TestDashboardViewModel()
    {
        _logger.LogInformation("Testando DashboardViewModel...");

        var viewModel = _serviceProvider.GetRequiredService<DashboardViewModel>();

        // Teste de inicialização
        if (viewModel == null)
            throw new Exception("DashboardViewModel não foi criado corretamente");

        // Teste de comandos
        if (viewModel.AtualizarDadosCommand == null)
            throw new Exception("Comando AtualizarDadosCommand não foi criado");

        // Teste de propriedades
        if (viewModel.StatusMessage == null)
            throw new Exception("Propriedade StatusMessage não foi inicializada");

        if (viewModel.LastUpdate == default)
            throw new Exception("Propriedade LastUpdate não foi inicializada");

        _logger.LogInformation("DashboardViewModel testado com sucesso!");
        return Task.CompletedTask;
    }

    private Task TestAdminViewModel()
    {
        _logger.LogInformation("Testando AdminViewModel...");

        var viewModel = _serviceProvider.GetRequiredService<AdminViewModel>();

        // Teste de inicialização
        if (viewModel == null)
            throw new Exception("AdminViewModel não foi criado corretamente");

        // Teste de comandos
        if (viewModel.MostrarFormularioCommand == null)
            throw new Exception("Comando MostrarFormularioCommand não foi criado");

        if (viewModel.CancelarFormularioCommand == null)
            throw new Exception("Comando CancelarFormularioCommand não foi criado");

        if (viewModel.SalvarEleicaoCommand == null)
            throw new Exception("Comando SalvarEleicaoCommand não foi criado");

        // Teste de propriedades
        if (viewModel.StatusMessage == null)
            throw new Exception("Propriedade StatusMessage não foi inicializada");

        if (viewModel.Eleicoes == null)
            throw new Exception("Propriedade Eleicoes não foi inicializada");

        _logger.LogInformation("AdminViewModel testado com sucesso!");
        return Task.CompletedTask;
    }
}
