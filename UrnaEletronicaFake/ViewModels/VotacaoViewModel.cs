using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UrnaEletronicaFake.Models;
using UrnaEletronicaFake.Services;

namespace UrnaEletronicaFake.ViewModels;

public class VotacaoViewModel : ViewModelBase
{
    private readonly IEleicaoService _eleicaoService;
    private readonly IVotoService _votoService;
    
    private Eleicao? _eleicaoAtiva;
    private ObservableCollection<Candidato> _candidatos;
    private Candidato? _candidatoSelecionado;
    private string _cpfEleitor = "";
    private string _numeroDigitado = "";
    private string _statusMessage = "";
    private bool _isLoading;
    private bool _votoConfirmado;

    public VotacaoViewModel(IEleicaoService eleicaoService, IVotoService votoService)
    {
        _eleicaoService = eleicaoService;
        _votoService = votoService;
        
        _candidatos = new ObservableCollection<Candidato>();
        
        // Comandos
        CarregarEleicaoAtivaCommand = new RelayCommand(async () => await CarregarEleicaoAtiva());
        DigitarNumeroCommand = new RelayCommand<string>(DigitarNumero);
        ConfirmarVotoCommand = new RelayCommand(async () => await ConfirmarVoto());
        VotarNuloCommand = new RelayCommand(async () => await VotarNulo());
        VotarBrancoCommand = new RelayCommand(async () => await VotarBranco());
        CorrigirCommand = new RelayCommand(Corrigir);
        
        // Carregar eleição ativa
        _ = CarregarEleicaoAtiva();
    }

    public Eleicao? EleicaoAtiva
    {
        get => _eleicaoAtiva;
        set => SetProperty(ref _eleicaoAtiva, value);
    }

    public ObservableCollection<Candidato> Candidatos
    {
        get => _candidatos;
        set => SetProperty(ref _candidatos, value);
    }

    public Candidato? CandidatoSelecionado
    {
        get => _candidatoSelecionado;
        set => SetProperty(ref _candidatoSelecionado, value);
    }

    public string CpfEleitor
    {
        get => _cpfEleitor;
        set => SetProperty(ref _cpfEleitor, value);
    }

    public string NumeroDigitado
    {
        get => _numeroDigitado;
        set => SetProperty(ref _numeroDigitado, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool VotoConfirmado
    {
        get => _votoConfirmado;
        set => SetProperty(ref _votoConfirmado, value);
    }

    public ICommand CarregarEleicaoAtivaCommand { get; }
    public ICommand DigitarNumeroCommand { get; }
    public ICommand ConfirmarVotoCommand { get; }
    public ICommand VotarNuloCommand { get; }
    public ICommand VotarBrancoCommand { get; }
    public ICommand CorrigirCommand { get; }

    private async Task CarregarEleicaoAtiva()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Carregando eleição ativa...";
            
            EleicaoAtiva = await _eleicaoService.ObterEleicaoAtivaAsync();
            
            if (EleicaoAtiva != null)
            {
                Candidatos.Clear();
                foreach (var candidato in EleicaoAtiva.Candidatos)
                {
                    Candidatos.Add(candidato);
                }
                
                StatusMessage = $"Eleição ativa: {EleicaoAtiva.Titulo}";
            }
            else
            {
                StatusMessage = "Nenhuma eleição ativa no momento";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao carregar eleição: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void DigitarNumero(string numero)
    {
        if (NumeroDigitado.Length < 5)
        {
            NumeroDigitado += numero;
            BuscarCandidato();
        }
    }

    private void BuscarCandidato()
    {
        CandidatoSelecionado = Candidatos.FirstOrDefault(c => c.Numero == NumeroDigitado);
        
        if (CandidatoSelecionado != null)
        {
            StatusMessage = $"Candidato: {CandidatoSelecionado.Nome} - {CandidatoSelecionado.Partido}";
        }
        else if (!string.IsNullOrEmpty(NumeroDigitado))
        {
            StatusMessage = "Candidato não encontrado";
        }
    }

    private async Task ConfirmarVoto()
    {
        if (string.IsNullOrEmpty(CpfEleitor))
        {
            StatusMessage = "Digite o CPF do eleitor";
            return;
        }

        if (CandidatoSelecionado == null)
        {
            StatusMessage = "Selecione um candidato válido";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Registrando voto...";
            
            await _votoService.RegistrarVotoAsync(CpfEleitor, EleicaoAtiva!.Id, CandidatoSelecionado.Id);
            
            VotoConfirmado = true;
            StatusMessage = "Voto registrado com sucesso!";
            
            // Limpar dados após confirmação
            await Task.Delay(3000);
            LimparDados();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao registrar voto: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task VotarNulo()
    {
        if (string.IsNullOrEmpty(CpfEleitor))
        {
            StatusMessage = "Digite o CPF do eleitor";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Registrando voto nulo...";
            
            await _votoService.RegistrarVotoAsync(CpfEleitor, EleicaoAtiva!.Id, null, votoNulo: true);
            
            VotoConfirmado = true;
            StatusMessage = "Voto nulo registrado com sucesso!";
            
            await Task.Delay(3000);
            LimparDados();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao registrar voto nulo: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task VotarBranco()
    {
        if (string.IsNullOrEmpty(CpfEleitor))
        {
            StatusMessage = "Digite o CPF do eleitor";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Registrando voto branco...";
            
            await _votoService.RegistrarVotoAsync(CpfEleitor, EleicaoAtiva!.Id, null, votoBranco: true);
            
            VotoConfirmado = true;
            StatusMessage = "Voto branco registrado com sucesso!";
            
            await Task.Delay(3000);
            LimparDados();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao registrar voto branco: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Corrigir()
    {
        NumeroDigitado = "";
        CandidatoSelecionado = null;
        StatusMessage = "Digite o número do candidato";
    }

    private void LimparDados()
    {
        CpfEleitor = "";
        NumeroDigitado = "";
        CandidatoSelecionado = null;
        VotoConfirmado = false;
        StatusMessage = "Digite o CPF do eleitor";
    }
} 