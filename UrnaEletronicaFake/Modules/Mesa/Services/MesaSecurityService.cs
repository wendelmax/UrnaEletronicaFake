using UrnaEletronicaFake.Modules.Core.Services;
using UrnaEletronicaFake.Modules.Core.Events;
using UrnaEletronicaFake.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UrnaEletronicaFake.Modules.Mesa.Services;

public interface IMesaSecurityService
{
    Task<bool> ValidateEleitorCredentialsAsync(string eleitorId);
    Task<bool> AuthorizeMesarioActionAsync(string action, string details);
    Task LogSecurityEventAsync(string eventType, string details);
    bool IsValidSessionTime();
}

public class MesaSecurityService : IMesaSecurityService
{
    private readonly IEventBus _eventBus;
    private readonly ITerminalLogService _terminalLogService;
    private readonly DateTime _sessionStartTime;
    private readonly HashSet<string> _processedEleitores;
    
    private static readonly TimeSpan MaxSessionDuration = TimeSpan.FromHours(12);
    private static readonly HashSet<string> RestrictedActions = new() 
    { 
        "EMERGENCY_UNLOCK", 
        "FORCE_RESET", 
        "AUDIT_ACCESS" 
    };

    public MesaSecurityService(IEventBus eventBus, ITerminalLogService terminalLogService)
    {
        _eventBus = eventBus;
        _terminalLogService = terminalLogService;
        _sessionStartTime = DateTime.Now;
        _processedEleitores = new HashSet<string>();
    }

    public async Task<bool> ValidateEleitorCredentialsAsync(string eleitorId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(eleitorId))
            {
                await LogSecurityEventAsync("VALIDATION_FAILED", "ID de eleitor vazio ou nulo");
                return false;
            }

            if (_processedEleitores.Contains(eleitorId))
            {
                await LogSecurityEventAsync("DUPLICATE_VOTER", $"Tentativa de voto duplicado para eleitor: {eleitorId}");
                return false;
            }

            if (!IsValidSessionTime())
            {
                await LogSecurityEventAsync("SESSION_EXPIRED", "Sessão da mesa expirou");
                return false;
            }

            if (!IsValidEleitorFormat(eleitorId))
            {
                await LogSecurityEventAsync("INVALID_FORMAT", $"Formato inválido de ID: {eleitorId}");
                return false;
            }

            _processedEleitores.Add(eleitorId);
            await LogSecurityEventAsync("VALIDATION_SUCCESS", $"Eleitor validado: {eleitorId}");
            return true;
        }
        catch (Exception ex)
        {
            await LogSecurityEventAsync("VALIDATION_ERROR", $"Erro na validação: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> AuthorizeMesarioActionAsync(string action, string details)
    {
        if (RestrictedActions.Contains(action.ToUpperInvariant()))
        {
            await LogSecurityEventAsync("RESTRICTED_ACTION", $"Ação restrita tentada: {action} - {details}");
            return false;
        }

        if (!IsValidSessionTime())
        {
            await LogSecurityEventAsync("SESSION_EXPIRED", $"Tentativa de ação após expiração: {action}");
            return false;
        }

        await LogSecurityEventAsync("ACTION_AUTHORIZED", $"Ação autorizada: {action} - {details}");
        return true;
    }

    public async Task LogSecurityEventAsync(string eventType, string details)
    {
        var timestamp = DateTime.Now;
        var logEntry = $"[SECURITY] [{eventType}] {details}";
        
        _terminalLogService.Registrar(logEntry);
        
        await _eventBus.PublishAsync(new MesaActivityEvent($"Security: {eventType}", details, timestamp));
    }

    public bool IsValidSessionTime()
    {
        var sessionDuration = DateTime.Now - _sessionStartTime;
        return sessionDuration < MaxSessionDuration;
    }

    private static bool IsValidEleitorFormat(string eleitorId)
    {
        return !string.IsNullOrWhiteSpace(eleitorId)
            && eleitorId.All(char.IsDigit)
            && eleitorId.Length >= 4
            && eleitorId.Length <= 12;
    }
}