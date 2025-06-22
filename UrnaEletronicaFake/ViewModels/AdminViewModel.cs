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
    private readonly ITerminalLogService _terminalLogService;
    
    [ObservableProperty]
    private ObservableCollection<Eleicao> _eleicoes = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEleicaoSelecionada))]
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
    private CargoEleitoral? _cargoCandidatoSelecionado;

    [ObservableProperty]
    private bool _modoEdicaoCandidato;
    
    [ObservableProperty]
    private bool _mostrarFormularioCandidato;

    // Propriedades para gerenciamento de cargos
    [ObservableProperty]
    private bool _mostrarPainelCargos;

    [ObservableProperty]
    private ObservableCollection<CargoEleitoral> _cargos = new();

    [ObservableProperty]
    private CargoEleitoral? _cargoSelecionado;

    [ObservableProperty]
    private string _nomeCargo = "";

    [ObservableProperty]
    private string _quantidadeDigitosCargo = "";

    [ObservableProperty]
    private string _ordemCargo = "";

    [ObservableProperty]
    private bool _modoEdicaoCargo;
    
    [ObservableProperty]
    private bool _mostrarFormularioCargo;

    public string FormularioTitulo => ModoEdicao ? "Editar Eleição" : "Criar Nova Eleição";
    public string BotaoSalvarTexto => ModoEdicao ? "Atualizar" : "Salvar";
    public string FormularioCandidatoTitulo => ModoEdicaoCandidato ? "Editar Candidato" : "Adicionar Candidato";
    public string BotaoSalvarCandidatoTexto => ModoEdicaoCandidato ? "Atualizar" : "Salvar";
    public string FormularioCargoTitulo => ModoEdicaoCargo ? "Editar Cargo" : "Adicionar Cargo";
    public string BotaoSalvarCargoTexto => ModoEdicaoCargo ? "Atualizar" : "Salvar";
    public bool IsEleicaoSelecionada => EleicaoSelecionada != null;

    public AdminViewModel(IEleicaoService eleicaoService, IAuditoriaService auditoriaService, ITerminalLogService terminalLogService)
    {
        _eleicaoService = eleicaoService;
        _auditoriaService = auditoriaService;
        _terminalLogService = terminalLogService;
        
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
        GerenciarCargosCommand = new RelayCommand(async () => await GerenciarCargos());
        
        // Comandos para candidatos
        AdicionarCandidatoCommand = new RelayCommand(AbrirFormularioCandidato);
        SalvarCandidatoCommand = new RelayCommand(async () => await SalvarCandidato());
        CancelarCandidatoCommand = new RelayCommand(CancelarFormularioCandidato);
        EditarCandidatoCommand = new RelayCommand(async () => await EditarCandidato());
        RemoverCandidatoCommand = new RelayCommand(async () => await RemoverCandidato());
        VoltarParaEleicoesCommand = new RelayCommand(VoltarParaEleicoes);
        
        // Comandos para gerenciamento de cargos
        AdicionarCargoCommand = new RelayCommand(AbrirFormularioCargo);
        SalvarCargoCommand = new RelayCommand(async () => await SalvarCargo());
        CancelarCargoCommand = new RelayCommand(CancelarFormularioCargo);
        EditarCargoCommand = new RelayCommand(async () => await EditarCargo());
        RemoverCargoCommand = new RelayCommand(async () => await RemoverCargo());
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
    public ICommand GerenciarCargosCommand { get; }
    
    // Comandos para candidatos
    public ICommand AdicionarCandidatoCommand { get; }
    public ICommand SalvarCandidatoCommand { get; }
    public ICommand CancelarCandidatoCommand { get; }
    public ICommand EditarCandidatoCommand { get; }
    public ICommand RemoverCandidatoCommand { get; }
    public ICommand VoltarParaEleicoesCommand { get; }

    // Comandos para gerenciamento de cargos
    public ICommand AdicionarCargoCommand { get; }
    public ICommand SalvarCargoCommand { get; }
    public ICommand CancelarCargoCommand { get; }
    public ICommand EditarCargoCommand { get; }
    public ICommand RemoverCargoCommand { get; }

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
                _terminalLogService.Registrar($"Eleição atualizada: {TituloEleicao}");
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
                _terminalLogService.Registrar($"Nova eleição criada: {TituloEleicao}");
                StatusMessage = "Eleição criada com sucesso!";
            }

            await CarregarEleicoes();
            CancelarFormulario();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao salvar eleição: {ex.Message}";
            _terminalLogService.Registrar($"Erro ao salvar eleição: {ex.Message}");
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
                _terminalLogService.Registrar($"Eleição deletada: {EleicaoSelecionada.Titulo}");
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
            _terminalLogService.Registrar($"Erro ao deletar eleição: {ex.Message}");
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
                _terminalLogService.Registrar($"Eleição ativada: {EleicaoSelecionada.Titulo}");
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
            _terminalLogService.Registrar($"Erro ao ativar eleição: {ex.Message}");
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
                _terminalLogService.Registrar($"Eleição desativada: {EleicaoSelecionada.Titulo}");
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
            _terminalLogService.Registrar($"Erro ao desativar eleição: {ex.Message}");
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
            StatusMessage = "Selecione uma eleição para gerenciar os candidatos.";
            return;
        }

        await CarregarCandidatos();
        await CarregarCargos(); // Garante que a lista de cargos esteja disponível

        MostrarFormulario = false;
        MostrarPainelCargos = false;
        MostrarPainelCandidatos = true;
        StatusMessage = $"Gerenciando candidatos para: {EleicaoSelecionada.Titulo}";
    }

    private void AbrirFormularioCandidato()
    {
        ModoEdicaoCandidato = false;
        CandidatoSelecionado = null;
        NomeCandidato = "";
        PartidoCandidato = "";
        NumeroCandidato = "";
        CargoCandidatoSelecionado = null;
        MostrarFormularioCandidato = true;
    }

    private async Task SalvarCandidato()
    {
        if (EleicaoSelecionada == null)
        {
            StatusMessage = "Nenhuma eleição selecionada.";
            return;
        }

        if (CargoCandidatoSelecionado == null)
        {
            StatusMessage = "Erro: É necessário selecionar um cargo para o candidato.";
            return;
        }

        try
        {
            var candidato = new Candidato
            {
                Nome = NomeCandidato,
                Partido = PartidoCandidato,
                Numero = NumeroCandidato,
                EleicaoId = EleicaoSelecionada.Id,
                CargoEleitoralId = CargoCandidatoSelecionado.Id
            };

            if (ModoEdicaoCandidato)
            {
                candidato.Id = CandidatoSelecionado!.Id;
                await _eleicaoService.AtualizarCandidatoAsync(EleicaoSelecionada.Id, candidato);
            }
            else
            {
                await _eleicaoService.AdicionarCandidatoAsync(EleicaoSelecionada.Id, candidato);
            }

            await CarregarCandidatos();
            CancelarFormularioCandidato();
            StatusMessage = "Candidato salvo com sucesso!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao salvar candidato: {ex.Message}";
            // Log detalhado da exceção interna
            _terminalLogService.Registrar($"ERRO FATAL ao salvar candidato: {ex.Message}");
            if (ex.InnerException != null)
            {
                _terminalLogService.Registrar($"Detalhes do erro: {ex.InnerException.Message}");
                StatusMessage += $" Detalhes: {ex.InnerException.Message}";
            }
        }
    }

    private void CancelarFormularioCandidato()
    {
        MostrarFormularioCandidato = false;
        NomeCandidato = "";
        PartidoCandidato = "";
        NumeroCandidato = "";
        CargoCandidatoSelecionado = null;
        CandidatoSelecionado = null;
    }

    private async Task EditarCandidato()
    {
        if (CandidatoSelecionado == null) return;

        ModoEdicaoCandidato = true;
        NomeCandidato = CandidatoSelecionado.Nome;
        PartidoCandidato = CandidatoSelecionado.Partido;
        NumeroCandidato = CandidatoSelecionado.Numero;
        CargoCandidatoSelecionado = Cargos.FirstOrDefault(c => c.Id == CandidatoSelecionado.CargoEleitoralId);
        MostrarFormularioCandidato = true;
        await Task.CompletedTask;
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
        MostrarPainelCargos = false;
        MostrarFormulario = false;
        StatusMessage = "Painel principal";
    }

    private void AbrirFormularioCargo()
    {
        ModoEdicaoCargo = false;
        CargoSelecionado = null;
        NomeCargo = "";
        QuantidadeDigitosCargo = "2";
        OrdemCargo = "0";
        MostrarFormularioCargo = true;
    }

    private async Task SalvarCargo()
    {
        if (string.IsNullOrWhiteSpace(NomeCargo))
        {
            StatusMessage = "Digite o nome do cargo";
            return;
        }

        if (string.IsNullOrWhiteSpace(QuantidadeDigitosCargo))
        {
            StatusMessage = "Digite a quantidade de dígitos do cargo";
            return;
        }

        if (string.IsNullOrWhiteSpace(OrdemCargo))
        {
            StatusMessage = "Digite a ordem do cargo";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Salvando cargo...";

            if (ModoEdicaoCargo && CargoSelecionado != null)
            {
                CargoSelecionado.Nome = NomeCargo;
                CargoSelecionado.QuantidadeDigitos = int.Parse(QuantidadeDigitosCargo);
                CargoSelecionado.Ordem = int.Parse(OrdemCargo);

                await _eleicaoService.AtualizarCargoAsync(EleicaoSelecionada.Id, CargoSelecionado);
                StatusMessage = "Cargo atualizado com sucesso!";
            }
            else
            {
                var novoCargo = new CargoEleitoral
                {
                    Nome = NomeCargo,
                    QuantidadeDigitos = int.Parse(QuantidadeDigitosCargo),
                    Ordem = int.Parse(OrdemCargo)
                };

                await _eleicaoService.AdicionarCargoAsync(EleicaoSelecionada.Id, novoCargo);
                StatusMessage = "Cargo criado com sucesso!";
            }

            await CarregarCargos();
            CancelarFormularioCargo();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao salvar cargo: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CancelarFormularioCargo()
    {
        MostrarFormularioCargo = false;
        NomeCargo = "";
        QuantidadeDigitosCargo = "";
        OrdemCargo = "";
        CargoSelecionado = null;
    }

    private async Task EditarCargo()
    {
        if (CargoSelecionado == null) return;

        ModoEdicaoCargo = true;
        NomeCargo = CargoSelecionado.Nome;
        QuantidadeDigitosCargo = CargoSelecionado.QuantidadeDigitos.ToString();
        OrdemCargo = CargoSelecionado.Ordem.ToString();
        MostrarFormularioCargo = true;
        await Task.CompletedTask;
    }

    private async Task RemoverCargo()
    {
        if (CargoSelecionado == null)
        {
            StatusMessage = "Selecione um cargo para remover";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Removendo cargo...";
            
            var sucesso = await _eleicaoService.RemoverCargoAsync(EleicaoSelecionada.Id, CargoSelecionado.Id);
            if (sucesso)
            {
                await CarregarCargos();
                CargoSelecionado = null;
                StatusMessage = "Cargo removido com sucesso!";
            }
            else
            {
                StatusMessage = "Erro ao remover cargo";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao remover cargo: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CarregarCargos()
    {
        if (EleicaoSelecionada == null) return;

        try
        {
            var cargos = await _eleicaoService.ObterCargosAsync(EleicaoSelecionada.Id);
            
            Cargos.Clear();
            foreach (var cargo in cargos)
            {
                Cargos.Add(cargo);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao carregar cargos: {ex.Message}";
        }
    }

    private async Task GerenciarCargos()
    {
        if (EleicaoSelecionada == null)
        {
            StatusMessage = "Selecione uma eleição para gerenciar cargos";
            return;
        }

        try
        {
            MostrarPainelCargos = true;
            await CarregarCargos();
            StatusMessage = $"Gerenciando cargos da eleição: {EleicaoSelecionada.Titulo}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erro ao abrir gerenciamento de cargos: {ex.Message}";
        }
    }
} 