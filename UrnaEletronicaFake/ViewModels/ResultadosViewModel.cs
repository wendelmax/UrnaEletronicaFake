using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using UrnaEletronicaFake.Models;
using UrnaEletronicaFake.Services;

namespace UrnaEletronicaFake.ViewModels;

public class ResultadosViewModel : ViewModelBase
{
    private readonly IEleicaoService _eleicaoService;
    private readonly IVotoService _votoService;
    
    private ObservableCollection<Eleicao> _eleicoes;
    private Eleicao? _eleicaoSelecionada;
    private object? _resultadoEleicao;
    private bool _isLoading;
    private string _statusMessage = "";

    public ResultadosViewModel(IEleicaoService eleicaoService, IVotoService votoService)
    {
        _eleicaoService = eleicaoService;
        _votoService = votoService;
        
        _eleicoes = new ObservableCollection<Eleicao>();
        
        // Comandos
        CarregarEleicoesCommand = new RelayCommand(async () => await CarregarEleicoes());
        CarregarResultadosCommand = new RelayCommand(async () => await CarregarResultados());
        
        // Carregar dados iniciais
        _ = CarregarEleicoes();
    }

    public ObservableCollection<Eleicao> Eleicoes
    {
        get => _eleicoes;
        set => SetProperty(ref _eleicoes, value);
    }

    public Eleicao? EleicaoSelecionada
    {
        get => _eleicaoSelecionada;
        set
        {
            SetProperty(ref _eleicaoSelecionada, value);
            if (value != null)
            {
                _ = CarregarResultados();
            }
        }
    }

    public object? ResultadoEleicao
    {
        get => _resultadoEleicao;
        set => SetProperty(ref _resultadoEleicao, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand CarregarEleicoesCommand { get; }
    public ICommand CarregarResultadosCommand { get; }

    private async Task CarregarEleicoes()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Carregando eleições...";
            
            var eleicoes = await _eleicaoService.ObterTodasEleicoesAsync();
            
            Eleicoes.Clear();
            foreach (var eleicao in eleicoes)
            {
                Eleicoes.Add(eleicao);
            }
            
            StatusMessage = $"Carregadas {Eleicoes.Count} eleições";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao carregar eleições: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CarregarResultados()
    {
        if (EleicaoSelecionada == null)
            return;

        try
        {
            IsLoading = true;
            StatusMessage = "Carregando resultados...";
            
            ResultadoEleicao = await _votoService.ObterResultadoEleicaoAsync(EleicaoSelecionada.Id);
            
            StatusMessage = $"Resultados carregados para: {EleicaoSelecionada.Titulo}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao carregar resultados: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
} 