using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Modules.Core.Events;
using UrnaEletronicaFake.Modules.Core.Services;

namespace UrnaEletronicaFake.Services;

public interface IMesaIntegrationService
{
    Task SimulateVotingProcessAsync(string eleitorId);
    Task HandleEmergencyLockAsync(string reason);
    Task GenerateAuditReportAsync();
}

public class MesaIntegrationService : IMesaIntegrationService
{
    private readonly ITerminalStateService _terminalStateService;
    private readonly ITerminalLogService _terminalLogService;
    private readonly IVotoService _votoService;
    private readonly ILogger<MesaIntegrationService> _logger;

    public MesaIntegrationService(
        ITerminalStateService terminalStateService,
        ITerminalLogService terminalLogService,
        IVotoService votoService,
        ILogger<MesaIntegrationService> logger)
    {
        _terminalStateService = terminalStateService;
        _terminalLogService = terminalLogService;
        _votoService = votoService;
        _logger = logger;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _terminalStateService.OnVoteStarted += OnVoteStartedAsync;
        _terminalStateService.OnVoteCompleted += OnVoteCompletedAsync;
        _terminalStateService.OnVoteAborted += OnVoteAbortedAsync;
        _terminalStateService.OnTerminalError += OnTerminalErrorAsync;
    }

    public async Task SimulateVotingProcessAsync(string eleitorId)
    {
        try 
        {
            _logger.LogInformation("Iniciando simulação de processo de votação para eleitor {EleitorId}", eleitorId);
            
            await _terminalStateService.HandleVoteStartedAsync(eleitorId);
            
            await Task.Delay(5000);
            
            var votoConfirmado = Random.Shared.NextDouble() > 0.1;
            
            await _terminalStateService.HandleVoteCompletedAsync(eleitorId, votoConfirmado);
            
            _logger.LogInformation("Simulação de votação concluída para eleitor {EleitorId}. Confirmado: {Confirmado}", 
                eleitorId, votoConfirmado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante simulação de votação para eleitor {EleitorId}", eleitorId);
            await _terminalStateService.HandleTerminalErrorAsync($"Erro na simulação: {ex.Message}");
        }
    }

    public async Task HandleEmergencyLockAsync(string reason)
    {
        try 
        {
            _logger.LogWarning("Bloqueio de emergência acionado: {Reason}", reason);
            
            var currentEleitor = _terminalStateService.EleitorAutenticadoId;
            if (!string.IsNullOrEmpty(currentEleitor))
            {
                await _terminalStateService.HandleVoteAbortedAsync(currentEleitor, reason);
            }
            else 
            {
                await _terminalStateService.LockTerminalAsync($"Emergência: {reason}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante bloqueio de emergência");
            await _terminalStateService.HandleTerminalErrorAsync($"Erro no bloqueio de emergência: {ex.Message}");
        }
    }

    public async Task GenerateAuditReportAsync()
    {
        try 
        {
            _logger.LogInformation("Gerando relatório de auditoria da mesa");
            
            var report = $@"
===== RELATÓRIO DE AUDITORIA DA MESA =====
Terminal: {Environment.MachineName}
Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
Status Atual: {(_terminalStateService.IsTerminalLocked ? "BLOQUEADO" : "LIBERADO")}
Eleitor Atual: {_terminalStateService.EleitorAutenticadoId ?? "Nenhum"}
Mesário Atual: {_terminalStateService.MesarioAtualId ?? "Nenhum"}
Última Liberação: {_terminalStateService.UltimaLiberacao?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}
Último Bloqueio: {_terminalStateService.UltimoBloqueio?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}
=========================================
";

            _terminalLogService.Registrar("Relatório de auditoria gerado");
            _logger.LogInformation("Relatório de auditoria: {Report}", report);
            
            Console.WriteLine(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório de auditoria");
            await _terminalStateService.HandleTerminalErrorAsync($"Erro no relatório: {ex.Message}");
        }
    }

    private async Task OnVoteStartedAsync(VoteStartedEvent eventArgs)
    {
        _logger.LogInformation("Evento: Votação iniciada para eleitor {EleitorId} no terminal {TerminalId}", 
            eventArgs.EleitorId, eventArgs.TerminalId);
        
        _terminalLogService.Registrar($"AUDITORIA: Início de votação detectado - {eventArgs.EleitorId}");
    }

    private async Task OnVoteCompletedAsync(VoteCompletedEvent eventArgs)
    {
        _logger.LogInformation("Evento: Votação finalizada para eleitor {EleitorId}. Confirmado: {Confirmed}", 
            eventArgs.EleitorId, eventArgs.VoteConfirmed);
        
        var status = eventArgs.VoteConfirmed ? "CONFIRMADO" : "CANCELADO";
        _terminalLogService.Registrar($"AUDITORIA: Fim de votação - {eventArgs.EleitorId} [{status}]");
        
        if (eventArgs.VoteConfirmed)
        {
            _terminalLogService.Registrar($"SEGURANÇA: Voto registrado com sucesso para {eventArgs.EleitorId}");
        }
    }

    private async Task OnVoteAbortedAsync(VoteAbortedEvent eventArgs)
    {
        _logger.LogWarning("Evento: Votação cancelada para eleitor {EleitorId}. Motivo: {Reason}", 
            eventArgs.EleitorId, eventArgs.Reason);
        
        _terminalLogService.Registrar($"ALERTA: Votação cancelada - {eventArgs.EleitorId} - {eventArgs.Reason}");
    }

    private async Task OnTerminalErrorAsync(TerminalErrorEvent eventArgs)
    {
        _logger.LogError("Evento: Erro no terminal {TerminalId}. Mensagem: {ErrorMessage}", 
            eventArgs.TerminalId, eventArgs.ErrorMessage);
        
        _terminalLogService.Registrar($"ERRO CRÍTICO: {eventArgs.ErrorMessage}");
        
        if (!string.IsNullOrEmpty(eventArgs.EleitorId))
        {
            _terminalLogService.Registrar($"IMPACTO: Erro afetou eleitor {eventArgs.EleitorId}");
        }
    }
}