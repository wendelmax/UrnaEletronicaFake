using System;
using System.Collections.ObjectModel;
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

    public AdminViewModel(IEleicaoService eleicaoService, IAuditoriaService auditoriaService)
    {
        _eleicaoService = eleicaoService;
        _auditoriaService = auditoriaService;
        
        _eleicoes = new ObservableCollection<Eleicao>();
        
        // Comandos
        CarregarEleicoesCommand = new RelayCommand(async () => await CarregarEleicoes());
        CriarEleicaoCommand = new RelayCommand(async () => await CriarEleicao());
        EditarEleicaoCommand = new RelayCommand(async () => await EditarEleicao());
        DeletarEleicaoCommand = new RelayCommand(async () => await DeletarEleicao());
        AtivarEleicaoCommand = new RelayCommand(async () => await AtivarEleicao());
        DesativarEleicaoCommand = new RelayCommand(async () => await DesativarEleicao());
        
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

    public ICommand CarregarEleicoesCommand { get; }
    public ICommand CriarEleicaoCommand { get; }
    public ICommand EditarEleicaoCommand { get; }
    public ICommand DeletarEleicaoCommand { get; }
    public ICommand AtivarEleicaoCommand { get; }
    public ICommand DesativarEleicaoCommand { get; }

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

    private async Task CriarEleicao()
    {
        try
        {
            var novaEleicao = new Eleicao
            {
                Titulo = "Nova Eleição",
                Descricao = "Descrição da nova eleição",
                DataInicio = DateTime.Now.AddDays(1),
                DataFim = DateTime.Now.AddDays(2)
            };

            await _eleicaoService.CriarEleicaoAsync(novaEleicao);
            await CarregarEleicoes();
            StatusMessage = "Eleição criada com sucesso!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao criar eleição: {ex.Message}";
        }
    }

    private async Task EditarEleicao()
    {
        if (EleicaoSelecionada == null)
        {
            StatusMessage = "Selecione uma eleição para editar";
            return;
        }

        try
        {
            // Aqui você implementaria a lógica para abrir uma janela de edição
            StatusMessage = $"Editando eleição: {EleicaoSelecionada.Titulo}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao editar eleição: {ex.Message}";
        }
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
            var sucesso = await _eleicaoService.DeletarEleicaoAsync(EleicaoSelecionada.Id);
            if (sucesso)
            {
                await CarregarEleicoes();
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
    }
} 