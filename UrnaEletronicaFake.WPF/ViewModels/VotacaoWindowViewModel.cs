using System.Windows.Input;
using UrnaEletronicaFake.Voting.Services;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.WPF.ViewModels;

public class VotacaoWindowViewModel : ViewModelBase
{
    private readonly IVotingService _votingService;
    private string _currentVote = "";
    private bool _votacaoLiberada = false;
    private string _eleitorAtual = "";

    public string CurrentVote
    {
        get => _currentVote;
        set => SetProperty(ref _currentVote, value);
    }

    public bool VotacaoLiberada
    {
        get => _votacaoLiberada;
        set => SetProperty(ref _votacaoLiberada, value);
    }

    public string EleitorAtual
    {
        get => _eleitorAtual;
        set => SetProperty(ref _eleitorAtual, value);
    }

    public ICommand DigitButtonCommand { get; }
    public ICommand DeleteButtonCommand { get; }
    public ICommand ClearButtonCommand { get; }
    public ICommand ConfirmVoteCommand { get; }
    public ICommand BlankVoteCommand { get; }

    public VotacaoWindowViewModel(
        IVotingService votingService,
        ILogger<VotacaoWindowViewModel> logger) : base(logger)
    {
        _votingService = votingService;
        
        DigitButtonCommand = new RelayCommand<string>(AddDigit);
        DeleteButtonCommand = new RelayCommand(DeleteDigit);
        ClearButtonCommand = new RelayCommand(ClearVote);
        ConfirmVoteCommand = new RelayCommand(ConfirmVote);
        BlankVoteCommand = new RelayCommand(BlankVote);
    }

    public void LiberarVotacao(string nomeEleitor)
    {
        EleitorAtual = nomeEleitor;
        VotacaoLiberada = true;
        LogInformation($"Votação liberada para: {nomeEleitor}");
    }

    public void BloquearVotacao()
    {
        VotacaoLiberada = false;
        EleitorAtual = "";
        CurrentVote = "";
        LogInformation("Votação bloqueada");
    }

    private void AddDigit(string digit)
    {
        if (!VotacaoLiberada) return;
        
        if (CurrentVote.Length < 2)
        {
            CurrentVote += digit;
        }
    }

    private void DeleteDigit()
    {
        if (!VotacaoLiberada) return;
        
        if (CurrentVote.Length > 0)
        {
            CurrentVote = CurrentVote[..^1];
        }
    }

    private void ClearVote()
    {
        if (!VotacaoLiberada) return;
        
        CurrentVote = "";
    }

    private void ConfirmVote()
    {
        if (!VotacaoLiberada) return;
        
        if (!string.IsNullOrEmpty(CurrentVote))
        {
            LogInformation($"Voto confirmado para o candidato número {CurrentVote}!");
            CurrentVote = "";
            BloquearVotacao();
        }
    }

    private void BlankVote()
    {
        if (!VotacaoLiberada) return;
        
        LogInformation("Voto em branco confirmado!");
        BloquearVotacao();
    }
}

// Implementação do RelayCommand com parâmetro para WPF
public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool>? _canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
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
        if (parameter is T typedParameter)
        {
            return _canExecute?.Invoke(typedParameter) ?? true;
        }
        return false;
    }

    public void Execute(object? parameter)
    {
        if (parameter is T typedParameter)
        {
            _execute(typedParameter);
        }
    }
}
