using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UrnaEletronicaFake.Models;
using UrnaEletronicaFake.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace UrnaEletronicaFake.ViewModels;

public partial class AdminViewModel : ViewModelBase
{
    private readonly IEleicaoService _eleicaoService;
    private readonly IAuditoriaService _auditoriaService;
    
    [ObservableProperty]
    private ObservableCollection<Eleicao> _eleicoes = new();

    [ObservableProperty]
    private Eleicao? _eleicaoSelecionada;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "";
    
    // Propriedades do formulário
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FormularioTitulo))]
    [NotifyPropertyChangedFor(nameof(BotaoSalvarTexto))]
    private bool _mostrarFormulario;

    [ObservableProperty]
    private bool _modoEdicao;

    [ObservableProperty]
    private string _tituloEleicao = "";

    [ObservableProperty]
    private string _descricaoEleicao = "";

    [ObservableProperty]
    private DateTimeOffset _dataInicio = DateTimeOffset.Now.AddDays(1);

    [ObservableProperty]
    private DateTimeOffset _dataFim = DateTimeOffset.Now.AddDays(2);
    
    // Propriedades para gerenciamento de candidatos
    [ObservableProperty]
    private bool _mostrarPainelCandidatos;

    [ObservableProperty]
    private ObservableCollection<Candidato> _candidatos = new();

    [ObservableProperty]
    private Candidato? _candidatoSelecionado;

    [ObservableProperty]
    private string _nomeCandidato = "";

    [ObservableProperty]
    private string _partidoCandidato = "";

    [ObservableProperty]
    private string _numeroCandidato = "";

    [ObservableProperty]
    private bool _modoEdicaoCandidato;
    
    public string FormularioTitulo => ModoEdicao ? "Editar Eleição" : "Criar Nova Eleição";
    public string BotaoSalvarTexto => ModoEdicao ? "Atualizar" : "Salvar";
    public bool IsEleicaoSelecionada => EleicaoSelecionada != null;

    public AdminViewModel(IEleicaoService eleicaoService, IAuditoriaService auditoriaService)
    {
        _eleicaoService = eleicaoService;
        _auditoriaService = auditoriaService;
        
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
        
        // Comandos para candidatos
        AdicionarCandidatoCommand = new RelayCommand(AbrirFormularioCandidato);
        SalvarCandidatoCommand = new RelayCommand(async () => await SalvarCandidato());
        CancelarCandidatoCommand = new RelayCommand(CancelarFormularioCandidato);
        EditarCandidatoCommand = new RelayCommand(async () => await EditarCandidato());
        RemoverCandidatoCommand = new RelayCommand(async () => await RemoverCandidato());
        VoltarParaEleicoesCommand = new RelayCommand(VoltarParaEleicoes);
        
        // Carregar dados iniciais
        _ = CarregarEleicoes();
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
    
    // Comandos para candidatos
    public ICommand AdicionarCandidatoCommand { get; }
    public ICommand SalvarCandidatoCommand { get; }
    public ICommand CancelarCandidatoCommand { get; }
    public ICommand EditarCandidatoCommand { get; }
    public ICommand RemoverCandidatoCommand { get; }
    public ICommand VoltarParaEleicoesCommand { get; }

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
        DataInicio = DateTimeOffset.Now.AddDays(1);
        DataFim = DateTimeOffset.Now.AddDays(2);
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
                EleicaoSelecionada.DataInicio = DataInicio.DateTime;
                EleicaoSelecionada.DataFim = DataFim.DateTime;

                await _eleicaoService.AtualizarEleicaoAsync(EleicaoSelecionada);
                StatusMessage = "Eleição atualizada com sucesso!";
            }
            else
            {
                var novaEleicao = new Eleicao
                {
                    Titulo = TituloEleicao,
                    Descricao = DescricaoEleicao,
                    DataInicio = DataInicio.DateTime,
                    DataFim = DataFim.DateTime
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

        try
        {
            IsLoading = true;
            StatusMessage = $"Carregando candidatos da eleição: {EleicaoSelecionada.Titulo}";
            
            var candidatos = await _eleicaoService.ObterCandidatosAsync(EleicaoSelecionada.Id);
            
            Candidatos.Clear();
            foreach (var candidato in candidatos)
            {
                Candidatos.Add(candidato);
            }
            
            MostrarPainelCandidatos = true;
            StatusMessage = $"Gerenciando candidatos da eleição: {EleicaoSelecionada.Titulo} ({Candidatos.Count} candidatos)";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao carregar candidatos: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AbrirFormularioCandidato()
    {
        ModoEdicaoCandidato = false;
        NomeCandidato = "";
        PartidoCandidato = "";
        NumeroCandidato = "";
        StatusMessage = "Preencha os dados do candidato...";
    }

    private async Task SalvarCandidato()
    {
        if (string.IsNullOrWhiteSpace(NomeCandidato))
        {
            StatusMessage = "Digite o nome do candidato";
            return;
        }

        if (string.IsNullOrWhiteSpace(PartidoCandidato))
        {
            StatusMessage = "Digite o partido do candidato";
            return;
        }

        if (string.IsNullOrWhiteSpace(NumeroCandidato))
        {
            StatusMessage = "Digite o número do candidato";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Salvando candidato...";

            if (ModoEdicaoCandidato && CandidatoSelecionado != null)
            {
                CandidatoSelecionado.Nome = NomeCandidato;
                CandidatoSelecionado.Partido = PartidoCandidato;
                CandidatoSelecionado.Numero = NumeroCandidato;

                await _eleicaoService.AtualizarCandidatoAsync(EleicaoSelecionada.Id, CandidatoSelecionado);
                StatusMessage = "Candidato atualizado com sucesso!";
            }
            else
            {
                var novoCandidato = new Candidato
                {
                    Nome = NomeCandidato,
                    Partido = PartidoCandidato,
                    Numero = NumeroCandidato
                };

                await _eleicaoService.AdicionarCandidatoAsync(EleicaoSelecionada.Id, novoCandidato);
                StatusMessage = "Candidato criado com sucesso!";
            }

            await CarregarCandidatos();
            CancelarFormularioCandidato();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao salvar candidato: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CancelarFormularioCandidato()
    {
        ModoEdicaoCandidato = false;
        NomeCandidato = "";
        PartidoCandidato = "";
        NumeroCandidato = "";
        StatusMessage = "Formulário cancelado";
    }

    private async Task EditarCandidato()
    {
        if (CandidatoSelecionado == null)
        {
            StatusMessage = "Selecione um candidato para editar";
            return;
        }

        ModoEdicaoCandidato = true;
        NomeCandidato = CandidatoSelecionado.Nome;
        PartidoCandidato = CandidatoSelecionado.Partido;
        NumeroCandidato = CandidatoSelecionado.Numero;
        StatusMessage = $"Editando candidato: {CandidatoSelecionado.Nome}";
    }

    private async Task RemoverCandidato()
    {
        if (CandidatoSelecionado == null)
        {
            StatusMessage = "Selecione um candidato para remover";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Removendo candidato...";
            
            var sucesso = await _eleicaoService.RemoverCandidatoAsync(EleicaoSelecionada.Id, CandidatoSelecionado.Id);
            if (sucesso)
            {
                await CarregarCandidatos();
                CandidatoSelecionado = null;
                StatusMessage = "Candidato removido com sucesso!";
            }
            else
            {
                StatusMessage = "Erro ao remover candidato";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao remover candidato: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CarregarCandidatos()
    {
        if (EleicaoSelecionada == null) return;

        try
        {
            var candidatos = await _eleicaoService.ObterCandidatosAsync(EleicaoSelecionada.Id);
            
            Candidatos.Clear();
            foreach (var candidato in candidatos)
            {
                Candidatos.Add(candidato);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao carregar candidatos: {ex.Message}";
        }
    }

    private void VoltarParaEleicoes()
    {
        MostrarPainelCandidatos = false;
        Candidatos.Clear();
        CandidatoSelecionado = null;
        StatusMessage = "Voltando para eleições...";
    }
} 