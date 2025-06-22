using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UrnaEletronicaFake.Models;
using UrnaEletronicaFake.Services;

namespace UrnaEletronicaFake.ViewModels;

public class AdminViewModel : ViewModelBase
{
    private readonly IEleicaoService _eleicaoService;
    private readonly IAuditoriaService _auditoriaService;
    
    private ObservableCollection<Eleicao> _eleicoes;
    private Eleicao? _eleicaoSelecionada;
    private bool _isLoading;
    private string _statusMessage = "";
    
    // Propriedades do formulário
    private bool _mostrarFormulario;
    private bool _modoEdicao;
    private string _tituloEleicao = "";
    private string _descricaoEleicao = "";
    private DateTime _dataInicio = DateTime.Now.AddDays(1);
    private DateTime _dataFim = DateTime.Now.AddDays(2);

    public AdminViewModel(IEleicaoService eleicaoService, IAuditoriaService auditoriaService)
    {
        _eleicaoService = eleicaoService;
        _auditoriaService = auditoriaService;
        
        _eleicoes = new ObservableCollection<Eleicao>();
        
        // Comandos
        CarregarEleicoesCommand = new RelayCommand(async () => await CarregarEleicoes());
        MostrarFormularioCommand = new RelayCommand(AbrirFormulario);
        SalvarEleicaoCommand = new RelayCommand(async () => await SalvarEleicao());
        CancelarFormularioCommand = new RelayCommand(CancelarFormulario);
        EditarEleicaoCommand = new RelayCommand(async () => await EditarEleicao());
        DeletarEleicaoCommand = new RelayCommand(async () => await DeletarEleicao());
        AtivarEleicaoCommand = new RelayCommand(async () => await AtivarEleicao());
        DesativarEleicaoCommand = new RelayCommand(async () => await DesativarEleicao());
        GerenciarCandidatosCommand = new RelayCommand(async () => await GerenciarCandidatos());
        
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
        set => SetProperty(ref _eleicaoSelecionada, value);
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

    // Propriedades do formulário
    public bool MostrarFormulario
    {
        get => _mostrarFormulario;
        set => SetProperty(ref _mostrarFormulario, value);
    }

    public bool ModoEdicao
    {
        get => _modoEdicao;
        set => SetProperty(ref _modoEdicao, value);
    }

    public string TituloEleicao
    {
        get => _tituloEleicao;
        set => SetProperty(ref _tituloEleicao, value);
    }

    public string DescricaoEleicao
    {
        get => _descricaoEleicao;
        set => SetProperty(ref _descricaoEleicao, value);
    }

    public DateTime DataInicio
    {
        get => _dataInicio;
        set => SetProperty(ref _dataInicio, value);
    }

    public DateTime DataFim
    {
        get => _dataFim;
        set => SetProperty(ref _dataFim, value);
    }

    public int EleicoesAtivas => Eleicoes.Count(e => e.Ativa);

    public ICommand CarregarEleicoesCommand { get; }
    public ICommand MostrarFormularioCommand { get; }
    public ICommand SalvarEleicaoCommand { get; }
    public ICommand CancelarFormularioCommand { get; }
    public ICommand EditarEleicaoCommand { get; }
    public ICommand DeletarEleicaoCommand { get; }
    public ICommand AtivarEleicaoCommand { get; }
    public ICommand DesativarEleicaoCommand { get; }
    public ICommand GerenciarCandidatosCommand { get; }

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

    private void AbrirFormulario()
    {
        ModoEdicao = false;
        TituloEleicao = "";
        DescricaoEleicao = "";
        DataInicio = DateTime.Now.AddDays(1);
        DataFim = DateTime.Now.AddDays(2);
        MostrarFormulario = true;
        StatusMessage = "Criando nova eleição...";
    }

    private async Task SalvarEleicao()
    {
        if (string.IsNullOrWhiteSpace(TituloEleicao))
        {
            StatusMessage = "Digite o título da eleição";
            return;
        }

        if (DataInicio >= DataFim)
        {
            StatusMessage = "A data de início deve ser anterior à data de fim";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Salvando eleição...";

            if (ModoEdicao && EleicaoSelecionada != null)
            {
                EleicaoSelecionada.Titulo = TituloEleicao;
                EleicaoSelecionada.Descricao = DescricaoEleicao;
                EleicaoSelecionada.DataInicio = DataInicio;
                EleicaoSelecionada.DataFim = DataFim;

                await _eleicaoService.AtualizarEleicaoAsync(EleicaoSelecionada);
                StatusMessage = "Eleição atualizada com sucesso!";
            }
            else
            {
                var novaEleicao = new Eleicao
                {
                    Titulo = TituloEleicao,
                    Descricao = DescricaoEleicao,
                    DataInicio = DataInicio,
                    DataFim = DataFim
                };

                await _eleicaoService.CriarEleicaoAsync(novaEleicao);
                StatusMessage = "Eleição criada com sucesso!";
            }

            await CarregarEleicoes();
            CancelarFormulario();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao salvar eleição: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CancelarFormulario()
    {
        MostrarFormulario = false;
        ModoEdicao = false;
        TituloEleicao = "";
        DescricaoEleicao = "";
        StatusMessage = "Formulário cancelado";
    }

    private async Task EditarEleicao()
    {
        if (EleicaoSelecionada == null)
        {
            StatusMessage = "Selecione uma eleição para editar";
            return;
        }

        ModoEdicao = true;
        TituloEleicao = EleicaoSelecionada.Titulo;
        DescricaoEleicao = EleicaoSelecionada.Descricao;
        DataInicio = EleicaoSelecionada.DataInicio;
        DataFim = EleicaoSelecionada.DataFim;
        MostrarFormulario = true;
        StatusMessage = $"Editando eleição: {EleicaoSelecionada.Titulo}";
    }

    private async Task DeletarEleicao()
    {
        if (EleicaoSelecionada == null)
        {
            StatusMessage = "Selecione uma eleição para deletar";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Deletando eleição...";
            
            var sucesso = await _eleicaoService.DeletarEleicaoAsync(EleicaoSelecionada.Id);
            if (sucesso)
            {
                await CarregarEleicoes();
                EleicaoSelecionada = null;
                StatusMessage = "Eleição deletada com sucesso!";
            }
            else
            {
                StatusMessage = "Erro ao deletar eleição";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao deletar eleição: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task AtivarEleicao()
    {
        if (EleicaoSelecionada == null)
        {
            StatusMessage = "Selecione uma eleição para ativar";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Ativando eleição...";
            
            var sucesso = await _eleicaoService.AtivarEleicaoAsync(EleicaoSelecionada.Id);
            if (sucesso)
            {
                await CarregarEleicoes();
                StatusMessage = "Eleição ativada com sucesso!";
            }
            else
            {
                StatusMessage = "Erro ao ativar eleição";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao ativar eleição: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DesativarEleicao()
    {
        if (EleicaoSelecionada == null)
        {
            StatusMessage = "Selecione uma eleição para desativar";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Desativando eleição...";
            
            var sucesso = await _eleicaoService.DesativarEleicaoAsync(EleicaoSelecionada.Id);
            if (sucesso)
            {
                await CarregarEleicoes();
                StatusMessage = "Eleição desativada com sucesso!";
            }
            else
            {
                StatusMessage = "Erro ao desativar eleição";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao desativar eleição: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task GerenciarCandidatos()
    {
        if (EleicaoSelecionada == null)
        {
            StatusMessage = "Selecione uma eleição para gerenciar candidatos";
            return;
        }

        StatusMessage = $"Gerenciando candidatos da eleição: {EleicaoSelecionada.Titulo}";
        // Aqui você implementaria a navegação para a tela de gerenciamento de candidatos
    }
} 