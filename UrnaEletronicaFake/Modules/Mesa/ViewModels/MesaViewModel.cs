using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UrnaEletronicaFake.Modules.Core.Services;
using UrnaEletronicaFake.Modules.Core.Events;
using UrnaEletronicaFake.Modules.Mesa.Services;
using UrnaEletronicaFake.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UrnaEletronicaFake.Modules.Mesa.ViewModels;

public partial class MesaViewModel : ObservableObject, INotificationHandler<TerminalUnlockedEvent>, INotificationHandler<TerminalLockedEvent>, INotificationHandler<VoteCompletedEvent>
{
    private readonly IEventBus _eventBus;
    private readonly IVotacaoStateService _votacaoStateService;
    private readonly ITerminalLogService _terminalLogService;
    private readonly IMesaSecurityService _mesaSecurityService;

    [ObservableProperty]
    private string _statusMessage = "Sistema iniciado. Aguardando identificação do eleitor.";
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LiberarUrnaCommand))]
    [NotifyCanExecuteChangedFor(nameof(BloquearUrnaCommand))]
    private string _identificacaoEleitor = "";
    
    [ObservableProperty]
    private bool _isProcessing = false;
    
    [ObservableProperty]
    private bool _isUrnaLiberada = false;
    
    [ObservableProperty]
    private DateTime? _ultimaAtividade;

    public IAsyncRelayCommand LiberarUrnaCommand { get; }
    public IRelayCommand BloquearUrnaCommand { get; }
    public IRelayCommand LimparCampoCommand { get; }

    public MesaViewModel(IEventBus eventBus, IVotacaoStateService votacaoStateService, ITerminalLogService terminalLogService, IMesaSecurityService mesaSecurityService)
    {
        _eventBus = eventBus;
        _votacaoStateService = votacaoStateService;
        _terminalLogService = terminalLogService;
        _mesaSecurityService = mesaSecurityService;
        
        LiberarUrnaCommand = new AsyncRelayCommand(LiberarUrnaAsync, CanLiberarUrna);
        BloquearUrnaCommand = new RelayCommand(BloquearUrna, CanBloquearUrna);
        LimparCampoCommand = new RelayCommand(LimparCampo);

        _votacaoStateService.OnTerminalStateChanged += OnTerminalStateChanged;
        
        _eventBus.Subscribe<TerminalUnlockedEvent>(this);
        _eventBus.Subscribe<TerminalLockedEvent>(this);
        _eventBus.Subscribe<VoteCompletedEvent>(this);

        UpdateStatusMessage();
        LogMesaActivity("Sistema iniciado", "Mesa receptora ativa e aguardando operações");
    }
    
    private void OnTerminalStateChanged()
    {
        LiberarUrnaCommand.NotifyCanExecuteChanged();
        BloquearUrnaCommand.NotifyCanExecuteChanged();
        IsUrnaLiberada = !_votacaoStateService.IsTerminalLocked;
        UpdateStatusMessage();
        UltimaAtividade = DateTime.Now;
        
        if (_votacaoStateService.IsTerminalLocked)
        {
            IdentificacaoEleitor = "";
        }
    }
    
    private void UpdateStatusMessage()
    {
        if (_votacaoStateService.IsTerminalLocked)
        {
            StatusMessage = IsProcessing 
                ? "Processando liberação..."
                : "Aguardando identificação do eleitor.";
        }
        else
        {
            StatusMessage = $"Urna liberada para o eleitor: {_votacaoStateService.EleitorAutenticadoId}.\nAguardando finalização do voto.";
        }
    }

    private bool CanLiberarUrna()
    {
        return _votacaoStateService.IsTerminalLocked 
            && !string.IsNullOrWhiteSpace(IdentificacaoEleitor) 
            && !IsProcessing
            && IsValidEleitorFormat(IdentificacaoEleitor);
    }
    
    private bool CanBloquearUrna()
    {
        return !_votacaoStateService.IsTerminalLocked && !IsProcessing;
    }

    private async Task LiberarUrnaAsync()
    {
        if (IsProcessing || string.IsNullOrWhiteSpace(IdentificacaoEleitor))
            return;

        try
        {
            IsProcessing = true;
            UpdateStatusMessage();
            
            if (!await _mesaSecurityService.AuthorizeMesarioActionAsync("LIBERAR_URNA", $"Eleitor: {IdentificacaoEleitor}"))
            {
                StatusMessage = "Ação não autorizada ou sessão expirada.";
                return;
            }
            
            if (!await _mesaSecurityService.ValidateEleitorCredentialsAsync(IdentificacaoEleitor))
            {
                StatusMessage = "Credenciais de eleitor inválidas ou voto duplicado.";
                return;
            }

            LogMesaActivity("Iniciando liberação", $"Validando eleitor: {IdentificacaoEleitor}");
            
            await Task.Delay(1000);
            
            _votacaoStateService.UnlockTerminal(IdentificacaoEleitor);
            
            await _eventBus.PublishAsync(new TerminalUnlockedEvent(IdentificacaoEleitor, DateTime.Now));
            await _eventBus.PublishAsync(new MesaActivityEvent("Liberação autorizada", $"Eleitor {IdentificacaoEleitor} autorizado a votar", DateTime.Now));
            
            LogMesaActivity("Liberação concluída", $"Terminal liberado para eleitor: {IdentificacaoEleitor}");
        }
        catch (Exception ex)
        {
            LogMesaActivity("Erro na liberação", $"Falha ao liberar terminal: {ex.Message}");
            StatusMessage = "Erro ao liberar urna. Tente novamente.";
        }
        finally
        {
            IsProcessing = false;
            UpdateStatusMessage();
        }
    }

    private async void BloquearUrna()
    {
        try
        {
            var eleitorAtual = _votacaoStateService.EleitorAutenticadoId;
            
            if (!await _mesaSecurityService.AuthorizeMesarioActionAsync("BLOQUEAR_URNA", $"Eleitor atual: {eleitorAtual ?? "nenhum"}"))
            {
                LogMesaActivity("Bloqueio negado", "Ação não autorizada");
                return;
            }
            
            _votacaoStateService.LockTerminal();
            
            await _eventBus.PublishAsync(new TerminalLockedEvent(eleitorAtual, DateTime.Now));
            await _eventBus.PublishAsync(new MesaActivityEvent("Bloqueio manual", $"Terminal bloqueado manualmente pelo mesário", DateTime.Now));
            
            LogMesaActivity("Bloqueio manual", eleitorAtual != null ? $"Terminal bloqueado - Eleitor: {eleitorAtual}" : "Terminal bloqueado");
        }
        catch (Exception ex)
        {
            LogMesaActivity("Erro no bloqueio", $"Falha ao bloquear terminal: {ex.Message}");
        }
    }
    
    private void LimparCampo()
    {
        IdentificacaoEleitor = "";
        LogMesaActivity("Campo limpo", "Campo de identificação limpo pelo mesário");
    }
    
    private static bool IsValidEleitorFormat(string eleitorId)
    {
        return !string.IsNullOrWhiteSpace(eleitorId) 
            && eleitorId.All(char.IsDigit) 
            && eleitorId.Length >= 4 
            && eleitorId.Length <= 12;
    }
    
    private void LogMesaActivity(string activity, string details)
    {
        _terminalLogService.Registrar($"[MESA] {activity}: {details}");
    }

    public Task Handle(TerminalUnlockedEvent notification, CancellationToken cancellationToken = default)
    {
        OnTerminalStateChanged();
        return Task.CompletedTask;
    }

    public Task Handle(TerminalLockedEvent notification, CancellationToken cancellationToken = default)
    {
        OnTerminalStateChanged();
        return Task.CompletedTask;
    }

    public Task Handle(VoteCompletedEvent notification, CancellationToken cancellationToken = default)
    {
        LogMesaActivity("Voto finalizado", $"Eleitor {notification.EleitorId} concluiu votação - ID: {notification.VoteId}");
        OnTerminalStateChanged();
        return Task.CompletedTask;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        
        if (e.PropertyName == nameof(IdentificacaoEleitor))
        {
            LiberarUrnaCommand.NotifyCanExecuteChanged();
        }
    }
}