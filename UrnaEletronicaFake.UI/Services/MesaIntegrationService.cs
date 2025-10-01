using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Core.Interfaces;

namespace UrnaEletronicaFake.UI.Services;

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
    }

    public async Task SimulateVotingProcessAsync(string eleitorId)
    {
        try 
        {
            _logger.LogInformation("Iniciando simulação de processo de votação para eleitor {EleitorId}", eleitorId);
            
            // Libera terminal para o eleitor
            var unlocked = await _terminalStateService.TryLockTerminalAsync(eleitorId);
            if (!unlocked)
            {
                _logger.LogWarning("Falha ao liberar terminal para {EleitorId}", eleitorId);
                return;
            }
            
            await Task.Delay(5000);
            
            var votoConfirmado = Random.Shared.NextDouble() > 0.1;
            
            // Finaliza sessão: define estado conforme resultado
            await _terminalStateService.SetTerminalStateAsync(
                votoConfirmado ? Shared.Enums.TerminalState.Unlocked : Shared.Enums.TerminalState.Locked,
                reason: votoConfirmado ? "Voto confirmado" : "Voto cancelado");
            
            _logger.LogInformation("Simulação de votação concluída para eleitor {EleitorId}. Confirmado: {Confirmado}", 
                eleitorId, votoConfirmado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante simulação de votação para eleitor {EleitorId}", eleitorId);
            // Registra erro no terminal state service
            await _terminalStateService.SetTerminalStateAsync(Shared.Enums.TerminalState.Error, reason: ex.Message);
        }
    }

    public async Task HandleEmergencyLockAsync(string reason)
    {
        try 
        {
            _logger.LogWarning("Bloqueio de emergência acionado: {Reason}", reason);
            
            var currentEleitor = await _terminalStateService.GetCurrentEleitorAsync();
            if (!string.IsNullOrEmpty(currentEleitor))
            {
                await _terminalStateService.SetTerminalStateAsync(Shared.Enums.TerminalState.Locked, reason: $"Emergência: {reason}");
            }
            else 
            {
                await _terminalStateService.SetTerminalStateAsync(Shared.Enums.TerminalState.Locked, reason: $"Emergência: {reason}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante bloqueio de emergência");
            await _terminalStateService.SetTerminalStateAsync(Shared.Enums.TerminalState.Error, reason: ex.Message);
        }
    }

    public async Task GenerateAuditReportAsync()
    {
        try 
        {
            _logger.LogInformation("Gerando relatório de auditoria da mesa");
            
            var currentState = await _terminalStateService.GetCurrentStateAsync();
            var isLocked = currentState == Shared.Enums.TerminalState.Locked;
            var eleitor = await _terminalStateService.GetCurrentEleitorAsync();
            var lastActivity = await _terminalStateService.GetLastActivityAsync();
            
            var report = $@"
===== RELATÓRIO DE AUDITORIA DA MESA =====
Terminal: {Environment.MachineName}
Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
Status Atual: {(isLocked ? "BLOQUEADO" : "LIBERADO")}
Eleitor Atual: {eleitor ?? "Nenhum"}
Última Atividade: {lastActivity?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}
=========================================
";
            
            _terminalLogService.Registrar("Relatório de auditoria gerado");
            _logger.LogInformation("Relatório de auditoria: {Report}", report);
            
            Console.WriteLine(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório de auditoria");
            await _terminalStateService.SetTerminalStateAsync(Shared.Enums.TerminalState.Error, reason: ex.Message);
        }
    }
}
