using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UrnaEletronicaFake.Models;
using UrnaEletronicaFake.Services;

namespace UrnaEletronicaFake.ViewModels;

public partial class VotacaoViewModel : ViewModelBase
{
    private readonly IEleicaoService _eleicaoService;
    private readonly IVotoService _votoService;
    private readonly IVotacaoStateService _votacaoStateService;

    private Eleicao? _eleicaoAtiva;
    private string _numeroDigitado = "";
    private const int NumeroDeDigitos = 5; // Pode ser ajustado conforme o cargo

    // --- Propriedades de Estado ---
    [ObservableProperty] private bool _isTerminalLocked = true;
    [ObservableProperty] private bool _isTelaFimVisible;
    [ObservableProperty] private bool _isTelaCandidatoVisible;
    [ObservableProperty] private bool _isVotoBranco;
    [ObservableProperty] private bool _isVotoNulo;

    // --- Propriedades de Tela ---
    [ObservableProperty] private string _cargo = "CARGO";
    [ObservableProperty] private string? _digito1;
    [ObservableProperty] private string? _digito2;
    [ObservableProperty] private string? _digito3;
    [ObservableProperty] private string? _digito4;
    [ObservableProperty] private string? _digito5;
    [ObservableProperty] private string _nomeCandidato = "";
    [ObservableProperty] private string _partidoCandidato = "";
    [ObservableProperty] private string? _fotoCandidato;
    [ObservableProperty] private string _instrucoes = "Aguardando liberação do terminal...";

    public VotacaoViewModel(IEleicaoService eleicaoService, IVotoService votoService, IVotacaoStateService votacaoStateService)
    {
        _eleicaoService = eleicaoService;
        _votoService = votoService;
        _votacaoStateService = votacaoStateService;

        _votacaoStateService.OnTerminalStateChanged += OnTerminalStateChanged;
        IsTerminalLocked = _votacaoStateService.IsTerminalLocked;

        DigitarNumeroCommand = new RelayCommand<string>(DigitarNumero);
        CorrigirCommand = new RelayCommand(Corrigir);
        VotarBrancoCommand = new RelayCommand(VotarBranco);
        ConfirmarCommand = new RelayCommand(ConfirmarVoto, () => !IsTerminalLocked && (IsTelaCandidatoVisible || IsVotoBranco));
    }

    public ICommand DigitarNumeroCommand { get; }
    public ICommand CorrigirCommand { get; }
    public ICommand VotarBrancoCommand { get; }
    public IRelayCommand ConfirmarCommand { get; }

    private async void OnTerminalStateChanged()
    {
        IsTerminalLocked = _votacaoStateService.IsTerminalLocked;
        if (!IsTerminalLocked)
        {
            await IniciarVotacao();
        }
    }

    private async Task IniciarVotacao()
    {
        _eleicaoAtiva = await _eleicaoService.ObterEleicaoAtivaAsync();
        if (_eleicaoAtiva != null)
        {
            Cargo = _eleicaoAtiva.Titulo.ToUpper();
            ResetarTela(true);
        }
        else
        {
            Cargo = "NENHUMA ELEIÇÃO ATIVA";
            Instrucoes = "Contate o mesário.";
        }
    }

    private void DigitarNumero(string? numero)
    {
        if (string.IsNullOrEmpty(numero) || _numeroDigitado.Length >= NumeroDeDigitos || IsVotoBranco || IsTelaFimVisible) return;

        _numeroDigitado += numero;
        AtualizarDigitos();

        if (_numeroDigitado.Length == NumeroDeDigitos)
        {
            BuscarCandidato();
        }
    }

    private void AtualizarDigitos()
    {
        var digitos = _numeroDigitado.PadRight(NumeroDeDigitos, ' ').ToCharArray();
        Digito1 = digitos[0] == ' ' ? null : digitos[0].ToString();
        Digito2 = digitos[1] == ' ' ? null : digitos[1].ToString();
        Digito3 = digitos[2] == ' ' ? null : digitos[2].ToString();
        Digito4 = digitos[3] == ' ' ? null : digitos[3].ToString();
        Digito5 = digitos[4] == ' ' ? null : digitos[4].ToString();
    }

    private void BuscarCandidato()
    {
        IsTelaCandidatoVisible = true;
        var candidato = _eleicaoAtiva?.Candidatos.FirstOrDefault(c => c.Numero == _numeroDigitado);

        if (candidato != null)
        {
            NomeCandidato = candidato.Nome;
            PartidoCandidato = candidato.Partido;
            FotoCandidato = "/Assets/candidate_placeholder.png";
            Instrucoes = "Aperte a tecla:\nVERDE para CONFIRMAR\nLARANJA para CORRIGIR";
            IsVotoNulo = false;
        }
        else
        {
            NomeCandidato = "NÚMERO ERRADO";
            PartidoCandidato = "";
            FotoCandidato = null;
            Instrucoes = "Seu voto será NULO.\nAperte a tecla:\nVERDE para CONFIRMAR\nLARANJA para CORRIGIR";
            IsVotoNulo = true;
        }
        ConfirmarCommand.NotifyCanExecuteChanged();
    }

    private void VotarBranco()
    {
        if (_numeroDigitado.Length > 0 || IsTelaFimVisible) return;

        ResetarTela(false);
        IsVotoBranco = true;
        IsTelaCandidatoVisible = true;
        NomeCandidato = "VOTO EM BRANCO";
        Instrucoes = "Aperte a tecla:\nVERDE para CONFIRMAR\nLARANJA para CORRIGIR";
        ConfirmarCommand.NotifyCanExecuteChanged();
    }

    private void Corrigir()
    {
        ResetarTela(true);
    }

    private async void ConfirmarVoto()
    {
        if (_eleicaoAtiva == null) return;
        
        var candidatoId = _eleicaoAtiva.Candidatos.FirstOrDefault(c => c.Numero == _numeroDigitado)?.Id;

        try
        {
            // O CPF é um placeholder, pois a identificação é feita pelo mesário
            var cpfPlaceholder = $"urn_{DateTime.Now:yyyyMMddHHmmssfff}";

            if (IsVotoBranco)
            {
                await _votoService.RegistrarVotoAsync(cpfPlaceholder, _eleicaoAtiva.Id, null, votoBranco: true);
            }
            else if (IsVotoNulo)
            {
                 await _votoService.RegistrarVotoAsync(cpfPlaceholder, _eleicaoAtiva.Id, null, votoNulo: true);
            }
            else if (candidatoId.HasValue)
            {
                await _votoService.RegistrarVotoAsync(cpfPlaceholder, _eleicaoAtiva.Id, candidatoId.Value);
            }
            else
            {
                return; // Não confirma se não há estado de voto válido
            }

            IsTelaCandidatoVisible = false;
            IsTelaFimVisible = true;
            await Task.Delay(2000);
        }
        catch (Exception)
        {
            Instrucoes = "Erro ao registrar voto. Contate o mesário.";
            await Task.Delay(3000);
        }
        finally
        {
            ResetarTela(false);
            IsTelaFimVisible = false;
            _votacaoStateService.LockTerminal();
            Instrucoes = "Aguardando liberação do terminal...";
            ConfirmarCommand.NotifyCanExecuteChanged();
        }
    }

    private void ResetarTela(bool manterCargo)
    {
        _numeroDigitado = "";
        IsVotoBranco = false;
        IsVotoNulo = false;
        IsTelaCandidatoVisible = false;
        NomeCandidato = "";
        PartidoCandidato = "";
        FotoCandidato = null;
        if (manterCargo)
        {
             Instrucoes = "Digite o número do seu candidato.";
        }
        ResetarDigitos();
        ConfirmarCommand.NotifyCanExecuteChanged();
    }

    private void ResetarDigitos()
    {
        Digito1 = null;
        Digito2 = null;
        Digito3 = null;
        Digito4 = null;
        Digito5 = null;
    }
} 