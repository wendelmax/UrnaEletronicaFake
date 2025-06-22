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
    private int _indiceCargoAtual = 0;
    private List<CargoEleitoral> _cargos = new();
    private CargoEleitoral? CargoAtual => (_cargos.Count > _indiceCargoAtual) ? _cargos[_indiceCargoAtual] : null;
    private int QuantidadeDigitosCargoAtual => CargoAtual?.QuantidadeDigitos ?? 2;

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

    public string TextoBotaoUrna => UrnaDesanexada ? "Anexar Urna" : "Desanexar Urna";

    private bool _urnaDesanexada = false;
    public bool UrnaDesanexada
    {
        get => _urnaDesanexada;
        set
        {
            if (SetProperty(ref _urnaDesanexada, value))
            {
                OnPropertyChanged(nameof(TextoBotaoUrna));
            }
        }
    }

    public IRelayCommand AlternarUrnaCommand { get; }
    public event Action? OnDesanexarUrna;
    public event Action? OnAnexarUrna;

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
        ConfirmarCommand = new RelayCommand(ConfirmarVoto, () => !IsTerminalLocked && IsTelaCandidatoVisible);
        AlternarUrnaCommand = new RelayCommand(AlternarUrna);
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
            _cargos = _eleicaoAtiva.CargosEleitorais.OrderBy(c => c.Ordem).ToList();
            _indiceCargoAtual = 0;
            if (_cargos.Count == 0)
            {
                Cargo = "SEM CARGOS DEFINIDOS";
                Instrucoes = "Contate o mesário.";
                return;
            }
            ExibirCargoAtual();
        }
        else
        {
            Cargo = "NENHUMA ELEIÇÃO ATIVA";
            Instrucoes = "Contate o mesário.";
        }
    }

    private void ExibirCargoAtual()
    {
        var cargo = CargoAtual;
        if (cargo == null)
        {
            Cargo = "FIM";
            IsTelaFimVisible = true;
            Instrucoes = "FIM DE VOTAÇÃO. Aguarde o próximo eleitor.";
            _votacaoStateService.LockTerminal();
            return;
        }
        Cargo = cargo.Nome.ToUpper();
        ResetarTela(true);
    }

    private void DigitarNumero(string? numero)
    {
        if (string.IsNullOrEmpty(numero) || _numeroDigitado.Length >= QuantidadeDigitosCargoAtual || IsVotoBranco || IsTelaFimVisible) return;

        _numeroDigitado += numero;
        AtualizarDigitos();

        if (_numeroDigitado.Length == QuantidadeDigitosCargoAtual)
        {
            BuscarCandidato();
        }
    }

    private void AtualizarDigitos()
    {
        var qtd = QuantidadeDigitosCargoAtual;
        var digitos = _numeroDigitado.PadRight(qtd, ' ').ToCharArray();
        Digito1 = digitos.Length > 0 && digitos[0] != ' ' ? digitos[0].ToString() : null;
        Digito2 = digitos.Length > 1 && digitos[1] != ' ' ? digitos[1].ToString() : null;
        Digito3 = digitos.Length > 2 && digitos[2] != ' ' ? digitos[2].ToString() : null;
        Digito4 = digitos.Length > 3 && digitos[3] != ' ' ? digitos[3].ToString() : null;
        Digito5 = digitos.Length > 4 && digitos[4] != ' ' ? digitos[4].ToString() : null;
    }

    private void BuscarCandidato()
    {
        IsTelaCandidatoVisible = true;
        var cargo = CargoAtual;
        var candidato = _eleicaoAtiva?.Candidatos.FirstOrDefault(c => c.CargoEleitoralId == cargo?.Id && c.Numero == _numeroDigitado);

        if (candidato != null)
        {
            NomeCandidato = candidato.Nome;
            PartidoCandidato = candidato.Partido;
            FotoCandidato = string.IsNullOrWhiteSpace(candidato.Foto) ? "/Assets/candidate_placeholder.png" : candidato.Foto;
            Instrucoes = "Aperte a tecla:\nVERDE para CONFIRMAR\nLARANJA para CORRIGIR";
            IsVotoNulo = false;
        }
        else
        {
            NomeCandidato = "VOTO NULO";
            PartidoCandidato = "";
            FotoCandidato = null;
            Instrucoes = "Aperte a tecla:\nVERDE para CONFIRMAR\nLARANJA para CORRIGIR";
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
        if (_eleicaoAtiva == null || CargoAtual == null) return;
        
        string? eleitorId = _votacaoStateService.EleitorAutenticadoId;
        if (string.IsNullOrWhiteSpace(eleitorId))
        {
            // Medida de segurança: não deveria acontecer no fluxo normal
            IsTelaFimVisible = true;
            Instrucoes = "ERRO: Eleitor não identificado.";
            _votacaoStateService.LockTerminal();
            return;
        }

        try
        {
            if (IsVotoBranco)
            {
                // Voto em branco
                await _votoService.RegistrarVotoAsync(eleitorId, _eleicaoAtiva.Id, CargoAtual.Id, null, votoBranco: true);
            }
            else if (IsVotoNulo)
            {
                // Voto nulo (número inválido)
                await _votoService.RegistrarVotoAsync(eleitorId, _eleicaoAtiva.Id, CargoAtual.Id, null, votoNulo: true);
            }
            else
            {
                // Voto em candidato válido
                var candidato = _eleicaoAtiva.Candidatos.FirstOrDefault(c => c.CargoEleitoralId == CargoAtual.Id && c.Numero == _numeroDigitado);
                if (candidato != null)
                {
                    await _votoService.RegistrarVotoAsync(eleitorId, _eleicaoAtiva.Id, CargoAtual.Id, candidato.Id);
                }
                else
                {
                    // Fallback: voto nulo por número inexistente
                    await _votoService.RegistrarVotoAsync(eleitorId, _eleicaoAtiva.Id, CargoAtual.Id, null, votoNulo: true);
                }
            }

            // Avança para o próximo cargo
            _indiceCargoAtual++;
            if (_indiceCargoAtual < _cargos.Count)
            {
                ExibirCargoAtual();
            }
            else
            {
                IsTelaFimVisible = true;
                Instrucoes = "FIM DE VOTAÇÃO. Aguarde o próximo eleitor.";
                await Task.Delay(2000);
                _votacaoStateService.LockTerminal();
                ResetarTela(false);
                IsTelaFimVisible = false;
                Instrucoes = "Aguardando liberação do terminal...";
            }
        }
        catch (Exception)
        {
            Instrucoes = "Erro ao registrar voto. Contate o mesário.";
            await Task.Delay(3000);
        }
        finally
        {
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

    private void AlternarUrna()
    {
        if (UrnaDesanexada)
        {
            UrnaDesanexada = false;
            OnAnexarUrna?.Invoke();
        }
        else
        {
            UrnaDesanexada = true;
            OnDesanexarUrna?.Invoke();
        }
    }
} 