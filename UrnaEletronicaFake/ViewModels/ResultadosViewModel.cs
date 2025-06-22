using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using UrnaEletronicaFake.Models;
using UrnaEletronicaFake.Services;
using System.Collections.Generic;

namespace UrnaEletronicaFake.ViewModels;

public class ResultadosViewModel : ViewModelBase
{
    private readonly IEleicaoService _eleicaoService;
    private readonly IVotoService _votoService;
    
    private ObservableCollection<Eleicao> _eleicoes;
    private Eleicao? _eleicaoSelecionada;
    private ResultadoEleicaoDto? _resultadoEleicao;
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

    public ResultadoEleicaoDto? ResultadoEleicao
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

            var resultado = await _votoService.ObterResultadoEleicaoAsync(EleicaoSelecionada.Id);
            // Conversão do objeto anônimo para DTO
            if (resultado != null)
            {
                var eleicaoProp = resultado.GetType().GetProperty("Eleicao")?.GetValue(resultado);
                var estatProp = resultado.GetType().GetProperty("Estatisticas")?.GetValue(resultado);
                var candProp = resultado.GetType().GetProperty("Candidatos")?.GetValue(resultado);

                var eleicaoDto = new EleicaoDto
                {
                    Id = (int)eleicaoProp?.GetType().GetProperty("Id")?.GetValue(eleicaoProp)!,
                    Titulo = (string)eleicaoProp?.GetType().GetProperty("Titulo")?.GetValue(eleicaoProp)!,
                    Descricao = (string)eleicaoProp?.GetType().GetProperty("Descricao")?.GetValue(eleicaoProp)!,
                    DataInicio = (DateTime)eleicaoProp?.GetType().GetProperty("DataInicio")?.GetValue(eleicaoProp)!,
                    DataFim = (DateTime)eleicaoProp?.GetType().GetProperty("DataFim")?.GetValue(eleicaoProp)!,
                    Ativa = (bool)eleicaoProp?.GetType().GetProperty("Ativa")?.GetValue(eleicaoProp)!,
                };
                var estatDto = new EstatisticasDto
                {
                    TotalVotos = (int)estatProp?.GetType().GetProperty("TotalVotos")?.GetValue(estatProp)!,
                    VotosValidos = (int)estatProp?.GetType().GetProperty("VotosValidos")?.GetValue(estatProp)!,
                    VotosNulos = (int)estatProp?.GetType().GetProperty("VotosNulos")?.GetValue(estatProp)!,
                    VotosBrancos = (int)estatProp?.GetType().GetProperty("VotosBrancos")?.GetValue(estatProp)!,
                    PercentualValidos = (double)estatProp?.GetType().GetProperty("PercentualValidos")?.GetValue(estatProp)!,
                    PercentualNulos = (double)estatProp?.GetType().GetProperty("PercentualNulos")?.GetValue(estatProp)!,
                    PercentualBrancos = (double)estatProp?.GetType().GetProperty("PercentualBrancos")?.GetValue(estatProp)!,
                };
                var candidatosDto = new List<CandidatoResultadoDto>();
                if (candProp is IEnumerable<object> candList)
                {
                    foreach (var c in candList)
                    {
                        candidatosDto.Add(new CandidatoResultadoDto
                        {
                            Id = (int)c.GetType().GetProperty("Id")?.GetValue(c)!,
                            Nome = (string)c.GetType().GetProperty("Nome")?.GetValue(c)!,
                            Partido = (string)c.GetType().GetProperty("Partido")?.GetValue(c)!,
                            Numero = (string)c.GetType().GetProperty("Numero")?.GetValue(c)!,
                            Votos = (int)c.GetType().GetProperty("Votos")?.GetValue(c)!,
                            Percentual = (double)c.GetType().GetProperty("Percentual")?.GetValue(c)!,
                        });
                    }
                }
                ResultadoEleicao = new ResultadoEleicaoDto
                {
                    Eleicao = eleicaoDto,
                    Estatisticas = estatDto,
                    Candidatos = candidatosDto
                };
            }
            else
            {
                ResultadoEleicao = null;
            }

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

public class ResultadoEleicaoDto
{
    public EleicaoDto Eleicao { get; set; } = new();
    public EstatisticasDto Estatisticas { get; set; } = new();
    public List<CandidatoResultadoDto> Candidatos { get; set; } = new();

    public int TotalVotos => Estatisticas.TotalVotos;
    public int VotosValidos => Estatisticas.VotosValidos;
    public int VotosNulos => Estatisticas.VotosNulos;
    public int VotosBrancos => Estatisticas.VotosBrancos;
    public double PercentualValidos => Estatisticas.PercentualValidos;
    public double PercentualNulos => Estatisticas.PercentualNulos;
    public double PercentualBrancos => Estatisticas.PercentualBrancos;
}

public class EleicaoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public bool Ativa { get; set; }
}

public class EstatisticasDto
{
    public int TotalVotos { get; set; }
    public int VotosValidos { get; set; }
    public int VotosNulos { get; set; }
    public int VotosBrancos { get; set; }
    public double PercentualValidos { get; set; }
    public double PercentualNulos { get; set; }
    public double PercentualBrancos { get; set; }
}

public class CandidatoResultadoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Partido { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public int Votos { get; set; }
    public double Percentual { get; set; }
} 