using System.Windows.Input;
using UrnaEletronicaFake.WPF.Services;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.WPF.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly WindowManager _windowManager;
    private bool _eleicaoAtiva = false;
    private DateTime _ultimaAtualizacao = DateTime.Now;

    public bool EleicaoAtiva
    {
        get => _eleicaoAtiva;
        set 
        { 
            SetProperty(ref _eleicaoAtiva, value);
            OnPropertyChanged(nameof(StatusEleicaoTexto));
        }
    }

    public string StatusEleicaoTexto => EleicaoAtiva ? "Eleição Ativa" : "Aguardando início";

    public DateTime UltimaAtualizacao
    {
        get => _ultimaAtualizacao;
        set => SetProperty(ref _ultimaAtualizacao, value);
    }

    public ICommand IniciarEleicaoCommand { get; }
    public ICommand FinalizarEleicaoCommand { get; }
    public ICommand PausarEleicaoCommand { get; }
    public ICommand OpenVotacaoCommand { get; }
    public ICommand OpenMesaCommand { get; }
    public ICommand OpenDashboardCommand { get; }

    public MainWindowViewModel(
        WindowManager windowManager,
        ILogger<MainWindowViewModel> logger) : base(logger)
    {
        _windowManager = windowManager;
        
        // Comandos
        IniciarEleicaoCommand = new RelayCommand(IniciarEleicao);
        FinalizarEleicaoCommand = new RelayCommand(FinalizarEleicao);
        PausarEleicaoCommand = new RelayCommand(PausarEleicao);
        OpenVotacaoCommand = new RelayCommand(OpenVotacao);
        OpenMesaCommand = new RelayCommand(OpenMesa);
        OpenDashboardCommand = new RelayCommand(OpenDashboard);
        
        // Simular atualização periódica
        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += (s, e) => UltimaAtualizacao = DateTime.Now;
        timer.Start();
    }

    private void IniciarEleicao()
    {
        try
        {
            EleicaoAtiva = true;
            LogInformation("Eleição iniciada");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao iniciar eleição");
        }
    }

    private void FinalizarEleicao()
    {
        try
        {
            EleicaoAtiva = false;
            LogInformation("Eleição finalizada");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao finalizar eleição");
        }
    }

    private void PausarEleicao()
    {
        try
        {
            EleicaoAtiva = false;
            LogInformation("Eleição pausada");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao pausar eleição");
        }
    }

    private void OpenVotacao()
    {
        try
        {
            _windowManager.OpenVotacaoWindow();
            LogInformation("Janela de votação aberta");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao abrir janela de votação");
        }
    }

    private void OpenMesa()
    {
        try
        {
            _windowManager.OpenMesaWindow();
            LogInformation("Janela de mesa aberta");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao abrir janela de mesa");
        }
    }

    private void OpenDashboard()
    {
        try
        {
            _windowManager.OpenDashboardWindow();
            LogInformation("Janela de dashboard aberta");
        }
        catch (Exception ex)
        {
            LogError(ex, "Erro ao abrir janela de dashboard");
        }
    }
}

// Implementação simples do RelayCommand para WPF
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute();
    }
}
