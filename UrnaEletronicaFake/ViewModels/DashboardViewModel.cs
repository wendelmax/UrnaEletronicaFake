using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UrnaEletronicaFake.Models;
using UrnaEletronicaFake.Services;
using System.Collections.Generic;
using System.Linq;

namespace UrnaEletronicaFake.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IEleicaoService _eleicaoService;
    private readonly IVotoService _votoService;
    private readonly IVotacaoStateService _votacaoStateService;
    
    private ObservableCollection<Eleicao> _eleicoes;
    private Eleicao? _eleicaoSelecionada;
    private DashboardDataDto? _dashboardData;
    private bool _isLoading;
    private string _statusMessage = "";
    private bool _autoRefresh = true;
    private int _totalVotosRealizados = 0;
    private int _urnaLiberada = 0;
    private int _urnaBloqueada = 1;

    public DashboardViewModel(IEleicaoService eleicaoService, IVotoService votoService, IVotacaoStateService votacaoStateService)
    {
        _eleicaoService = eleicaoService;
        _votoService = votoService;
        _votacaoStateService = votacaoStateService;
        
        _eleicoes = new ObservableCollection<Eleicao>();
        
        // Comandos
        CarregarEleicoesCommand = new RelayCommand(async () => await CarregarEleicoes());
        CarregarDashboardCommand = new RelayCommand(async () => await CarregarDashboard());
        AtualizarDadosCommand = new RelayCommand(async () => await AtualizarDados());
        
        // Configurar eventos
        _votacaoStateService.OnTerminalStateChanged += OnTerminalStateChanged;
        
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
                _ = CarregarDashboard();
            }
        }
    }

    public DashboardDataDto? DashboardData
    {
        get => _dashboardData;
        set => SetProperty(ref _dashboardData, value);
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

    public bool AutoRefresh
    {
        get => _autoRefresh;
        set => SetProperty(ref _autoRefresh, value);
    }

    public int TotalVotosRealizados
    {
        get => _totalVotosRealizados;
        set => SetProperty(ref _totalVotosRealizados, value);
    }

    public int UrnaLiberada
    {
        get => _urnaLiberada;
        set => SetProperty(ref _urnaLiberada, value);
    }

    public int UrnaBloqueada
    {
        get => _urnaBloqueada;
        set => SetProperty(ref _urnaBloqueada, value);
    }

    public ICommand CarregarEleicoesCommand { get; }
    public ICommand CarregarDashboardCommand { get; }
    public ICommand AtualizarDadosCommand { get; }

    private async Task CarregarEleicoes()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Carregando eleições...";
            
            var eleicoes = await _eleicaoService.ObterTodasEleicoesAsync();
            
            Eleicoes.Clear();
            foreach (var eleicao in eleicoes.OrderByDescending(e => e.DataInicio))
            {
                Eleicoes.Add(eleicao);
            }
            
            // Selecionar automaticamente a primeira eleição ativa
            var eleicaoAtiva = eleicoes.FirstOrDefault(e => e.Ativa);
            if (eleicaoAtiva != null)
            {
                EleicaoSelecionada = eleicaoAtiva;
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

    private async Task CarregarDashboard()
    {
        if (EleicaoSelecionada == null)
            return;

        try
        {
            IsLoading = true;
            StatusMessage = "Carregando dados do dashboard...";

            var resultado = await _votoService.ObterResultadoEleicaoAsync(EleicaoSelecionada.Id);
            
            if (resultado != null)
            {
                var eleicaoProp = resultado.GetType().GetProperty("Eleicao")?.GetValue(resultado);
                var estatProp = resultado.GetType().GetProperty("Estatisticas")?.GetValue(resultado);
                var candProp = resultado.GetType().GetProperty("Candidatos")?.GetValue(resultado);

                var estatisticas = new EstatisticasDto
                {
                    TotalVotos = (int)estatProp?.GetType().GetProperty("TotalVotos")?.GetValue(estatProp)!,
                    VotosValidos = (int)estatProp?.GetType().GetProperty("VotosValidos")?.GetValue(estatProp)!,
                    VotosNulos = (int)estatProp?.GetType().GetProperty("VotosNulos")?.GetValue(estatProp)!,
                    VotosBrancos = (int)estatProp?.GetType().GetProperty("VotosBrancos")?.GetValue(estatProp)!,
                    PercentualValidos = (double)estatProp?.GetType().GetProperty("PercentualValidos")?.GetValue(estatProp)!,
                    PercentualNulos = (double)estatProp?.GetType().GetProperty("PercentualNulos")?.GetValue(estatProp)!,
                    PercentualBrancos = (double)estatProp?.GetType().GetProperty("PercentualBrancos")?.GetValue(estatProp)!,
                };

                var candidatos = new List<CandidatoResultadoDto>();
                if (candProp is IEnumerable<object> candList)
                {
                    foreach (var c in candList)
                    {
                        candidatos.Add(new CandidatoResultadoDto
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

                DashboardData = new DashboardDataDto
                {
                    Eleicao = EleicaoSelecionada,
                    Estatisticas = estatisticas,
                    Candidatos = candidatos,
                    StatusUrna = _votacaoStateService.IsTerminalLocked ? "Bloqueada" : "Liberada",
                    EleitorAtual = _votacaoStateService.EleitorAutenticadoId ?? "Nenhum",
                    UltimaAtualizacao = DateTime.Now
                };

                TotalVotosRealizados = estatisticas.TotalVotos;
                UrnaLiberada = _votacaoStateService.IsTerminalLocked ? 0 : 1;
                UrnaBloqueada = _votacaoStateService.IsTerminalLocked ? 1 : 0;
            }

            StatusMessage = $"Dashboard atualizado - {EleicaoSelecionada.Titulo}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao carregar dashboard: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task AtualizarDados()
    {
        if (AutoRefresh && EleicaoSelecionada != null)
        {
            await CarregarDashboard();
        }
    }

    private async void OnTerminalStateChanged()
    {
        if (EleicaoSelecionada != null)
        {
            await AtualizarDados();
        }
    }
}

public class DashboardDataDto
{
    public Eleicao Eleicao { get; set; } = new();
    public EstatisticasDto Estatisticas { get; set; } = new();
    public List<CandidatoResultadoDto> Candidatos { get; set; } = new();
    public string StatusUrna { get; set; } = "";
    public string EleitorAtual { get; set; } = "";
    public DateTime UltimaAtualizacao { get; set; }

    public string StatusUrnaFormatado => StatusUrna == "Bloqueada" ? "🔒 Bloqueada" : "🔓 Liberada";
    public string UltimaAtualizacaoFormatada => UltimaAtualizacao.ToString("HH:mm:ss");
    public string EleicaoAtiva => Eleicao.Ativa ? "🟢 Ativa" : "🔴 Inativa";
}
