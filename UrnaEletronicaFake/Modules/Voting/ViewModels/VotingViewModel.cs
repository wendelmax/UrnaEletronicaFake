using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using UrnaEletronicaFake.Models;
using UrnaEletronicaFake.Services;
using UrnaEletronicaFake.Modules.Core.Events;

namespace UrnaEletronicaFake.Modules.Voting.ViewModels;

public partial class VotingViewModel : ObservableObject,
    INotificationHandler<TerminalUnlockedEvent>,
    INotificationHandler<TerminalLockedEvent>
{
    private readonly IEleicaoService _eleicaoService;
    private readonly IVotoService _votoService;
    private readonly ITerminalLogService _terminalLogService;

    private Eleicao? _eleicaoAtiva;
    private string _numeroDigitado = "";
    private int _indiceCargoAtual = 0;
    private List<CargoEleitoral> _cargos = new();
    private string? _eleitorId;

    private CargoEleitoral? CargoAtual => (_cargos.Count > _indiceCargoAtual) ? _cargos[_indiceCargoAtual] : null;
    private int QuantidadeDigitosCargoAtual => CargoAtual?.QuantidadeDigitos ?? 2;

    [ObservableProperty] private bool _isTerminalLocked = true;
    [ObservableProperty] private bool _isTelaFimVisible;
    [ObservableProperty] private bool _isTelaCandidatoVisible;
    [ObservableProperty] private bool _isVotoBranco;
    [ObservableProperty] private bool _isVotoNulo;

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

    public VotingViewModel(
        IEleicaoService eleicaoService,
        IVotoService votoService,
        ITerminalLogService terminalLogService)
    {
        _eleicaoService = eleicaoService;
        _votoService = votoService;
        _terminalLogService = terminalLogService;

        DigitarNumeroCommand = new RelayCommand<string>(DigitarNumero);
        CorrigirCommand = new RelayCommand(Corrigir);
        VotarBrancoCommand = new RelayCommand(VotarBranco);
        ConfirmarCommand = new RelayCommand(ConfirmarVoto, CanConfirmarVoto);
    }

    public IRelayCommand<string> DigitarNumeroCommand { get; }
    public IRelayCommand CorrigirCommand { get; }
    public IRelayCommand VotarBrancoCommand { get; }
    public IRelayCommand ConfirmarCommand { get; }

    public async Task Handle(TerminalUnlockedEvent notification, CancellationToken cancellationToken)
    {
        _eleitorId = notification.EleitorId;
        IsTerminalLocked = false;
        await IniciarVotacao();
        
        await _terminalLogService.LogAsync($"Terminal desbloqueado para eleitor: {notification.EleitorId}");
    }

    public async Task Handle(TerminalLockedEvent notification, CancellationToken cancellationToken)
    {
        IsTerminalLocked = true;
        _eleitorId = null;
        ResetarTela(false);
        IsTelaFimVisible = false;
        Instrucoes = "Aguardando liberação do terminal...";
        
        await _terminalLogService.LogAsync("Terminal bloqueado");
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
            return;
        }
        
        Cargo = cargo.Nome.ToUpper();
        ResetarTela(true);
    }

    private void DigitarNumero(string? numero)
    {
        if (string.IsNullOrEmpty(numero) || 
            _numeroDigitado.Length >= QuantidadeDigitosCargoAtual || 
            IsTelaFimVisible || 
            IsTerminalLocked) 
            return;

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
        var candidato = _eleicaoAtiva?.Candidatos
            .FirstOrDefault(c => c.CargoEleitoralId == cargo?.Id && c.Numero == _numeroDigitado);

        if (candidato != null)
        {
            ProcessarVotoValido(candidato);
        }
        else
        {
            ProcessarVotoNulo();
        }
        
        ConfirmarCommand.NotifyCanExecuteChanged();
    }

    private void ProcessarVotoValido(Candidato candidato)
    {
        NomeCandidato = candidato.Nome;
        PartidoCandidato = candidato.Partido;
        FotoCandidato = string.IsNullOrWhiteSpace(candidato.Foto) 
            ? "/Assets/candidate_placeholder.png" 
            : candidato.Foto;
        Instrucoes = "Aperte a tecla:\nVERDE para CONFIRMAR\nLARANJA para CORRIGIR";
        IsVotoNulo = false;
    }

    private void ProcessarVotoNulo()
    {
        NomeCandidato = "VOTO NULO";
        PartidoCandidato = "";
        FotoCandidato = null;
        Instrucoes = "Aperte a tecla:\nVERDE para CONFIRMAR\nLARANJA para CORRIGIR";
        IsVotoNulo = true;
    }

    private void VotarBranco()
    {
        if (_numeroDigitado.Length > 0 || 
            IsTelaFimVisible || 
            IsTerminalLocked) 
            return;

        ProcessarVotoBranco();
    }

    private void ProcessarVotoBranco()
    {
        ResetarTela(false);
        IsVotoBranco = true;
        IsTelaCandidatoVisible = true;
        NomeCandidato = "VOTO EM BRANCO";
        PartidoCandidato = "";
        FotoCandidato = null;
        Instrucoes = "Aperte a tecla:\nVERDE para CONFIRMAR\nLARANJA para CORRIGIR";
        ConfirmarCommand.NotifyCanExecuteChanged();
    }

    private void Corrigir()
    {
        if (IsTerminalLocked) return;
        ResetarTela(true);
    }

    private bool CanConfirmarVoto()
    {
        return !IsTerminalLocked && IsTelaCandidatoVisible;
    }

    private async void ConfirmarVoto()
    {
        if (_eleicaoAtiva == null || 
            CargoAtual == null || 
            string.IsNullOrWhiteSpace(_eleitorId)) 
        {
            await TratarErroConfirmacao("ERRO: Eleitor não identificado.");
            return;
        }

        try
        {
            await ProcessarConfirmacaoVoto();
            
            _indiceCargoAtual++;
            if (_indiceCargoAtual < _cargos.Count)
            {
                ExibirCargoAtual();
            }
            else
            {
                await FinalizarVotacao();
            }
        }
        catch (Exception ex)
        {
            await TratarErroConfirmacao("Erro ao registrar voto. Contate o mesário.");
            await _terminalLogService.LogAsync($"Erro ao registrar voto: {ex.Message}");
        }
        finally
        {
            ConfirmarCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task ProcessarConfirmacaoVoto()
    {
        if (IsVotoBranco)
        {
            await _votoService.RegistrarVotoAsync(_eleitorId!, _eleicaoAtiva!.Id, CargoAtual!.Id, null, votoBranco: true);
        }
        else if (IsVotoNulo)
        {
            await _votoService.RegistrarVotoAsync(_eleitorId!, _eleicaoAtiva!.Id, CargoAtual!.Id, null, votoNulo: true);
        }
        else
        {
            var candidato = _eleicaoAtiva!.Candidatos
                .FirstOrDefault(c => c.CargoEleitoralId == CargoAtual!.Id && c.Numero == _numeroDigitado);
            
            if (candidato != null)
            {
                await _votoService.RegistrarVotoAsync(_eleitorId!, _eleicaoAtiva.Id, CargoAtual!.Id, candidato.Id);
            }
            else
            {
                await _votoService.RegistrarVotoAsync(_eleitorId!, _eleicaoAtiva.Id, CargoAtual!.Id, null, votoNulo: true);
            }
        }
    }

    private async Task FinalizarVotacao()
    {
        IsTelaFimVisible = true;
        Instrucoes = "FIM DE VOTAÇÃO. Aguarde o próximo eleitor.";
        await Task.Delay(2000);
        
        ResetarTela(false);
        IsTelaFimVisible = false;
        Instrucoes = "Aguardando liberação do terminal...";
    }

    private async Task TratarErroConfirmacao(string mensagem)
    {
        IsTelaFimVisible = true;
        Instrucoes = mensagem;
        await Task.Delay(3000);
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