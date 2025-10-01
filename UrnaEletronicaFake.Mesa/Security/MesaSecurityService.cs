using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Core.Interfaces;
using UrnaEletronicaFake.Core.Events;
using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Shared.Constants;
using System.Collections.Concurrent;

namespace UrnaEletronicaFake.Mesa.Security;

public class MesaSecurityService : IMesaSecurityService
{
    private readonly ILogger<MesaSecurityService> _logger;
    private readonly IEventBus _eventBus;
    private readonly ITerminalStateService _terminalStateService;
    private readonly ConcurrentDictionary<string, MesaSession> _activeSessions = new();
    private readonly Dictionary<string, string> _mesarioCredentials = new();

    public MesaSecurityService(
        ILogger<MesaSecurityService> logger,
        IEventBus eventBus,
        ITerminalStateService terminalStateService)
    {
        _logger = logger;
        _eventBus = eventBus;
        _terminalStateService = terminalStateService;
        
        // Credenciais iniciais dos mesários (em produção, isso viria de um banco de dados)
        InitializeMesarioCredentials();
    }

    public async Task<bool> ValidateEleitorAsync(string eleitorId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(eleitorId))
            {
                await LogSecurityEventAsync("SYSTEM", "VALIDATE_ELEITOR", false, "Eleitor ID vazio");
                return false;
            }

            if (eleitorId.Length < SecurityConstants.PASSWORD_MIN_LENGTH || 
                eleitorId.Length > SecurityConstants.PASSWORD_MAX_LENGTH)
            {
                await LogSecurityEventAsync("SYSTEM", "VALIDATE_ELEITOR", false, $"Eleitor ID com tamanho inválido: {eleitorId.Length}");
                return false;
            }

            if (!eleitorId.All(char.IsDigit))
            {
                await LogSecurityEventAsync("SYSTEM", "VALIDATE_ELEITOR", false, "Eleitor ID contém caracteres não numéricos");
                return false;
            }

            await LogSecurityEventAsync("SYSTEM", "VALIDATE_ELEITOR", true, $"Eleitor ID válido: {eleitorId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating eleitor {EleitorId}", eleitorId);
            await LogSecurityEventAsync("SYSTEM", "VALIDATE_ELEITOR", false, $"Erro interno: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ValidateMesarioAsync(string mesarioId, string senha)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mesarioId) || string.IsNullOrWhiteSpace(senha))
            {
                await LogSecurityEventAsync(mesarioId ?? "UNKNOWN", "VALIDATE_MESARIO", false, "Credenciais vazias");
                return false;
            }

            if (_mesarioCredentials.TryGetValue(mesarioId, out var expectedPassword) && expectedPassword == senha)
            {
                await LogSecurityEventAsync(mesarioId, "VALIDATE_MESARIO", true, "Login bem-sucedido");
                return true;
            }

            await LogSecurityEventAsync(mesarioId, "VALIDATE_MESARIO", false, "Credenciais inválidas");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating mesario {MesarioId}", mesarioId);
            await LogSecurityEventAsync(mesarioId ?? "UNKNOWN", "VALIDATE_MESARIO", false, $"Erro interno: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CanUnlockTerminalAsync(string mesarioId)
    {
        try
        {
            if (!await IsSessionValidAsync(mesarioId))
            {
                await LogSecurityEventAsync(mesarioId, "UNLOCK_TERMINAL", false, "Sessão inválida");
                return false;
            }

            var isAvailable = await _terminalStateService.IsTerminalAvailableAsync();
            if (!isAvailable)
            {
                await LogSecurityEventAsync(mesarioId, "UNLOCK_TERMINAL", false, "Terminal já está em uso");
                return false;
            }

            await LogSecurityEventAsync(mesarioId, "UNLOCK_TERMINAL", true, "Permissão concedida");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking unlock permission for mesario {MesarioId}", mesarioId);
            await LogSecurityEventAsync(mesarioId, "UNLOCK_TERMINAL", false, $"Erro interno: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CanLockTerminalAsync(string mesarioId)
    {
        try
        {
            if (!await IsSessionValidAsync(mesarioId))
            {
                await LogSecurityEventAsync(mesarioId, "LOCK_TERMINAL", false, "Sessão inválida");
                return false;
            }

            await LogSecurityEventAsync(mesarioId, "LOCK_TERMINAL", true, "Permissão concedida");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking lock permission for mesario {MesarioId}", mesarioId);
            await LogSecurityEventAsync(mesarioId, "LOCK_TERMINAL", false, $"Erro interno: {ex.Message}");
            return false;
        }
    }

    public Task<bool> IsSessionValidAsync(string mesarioId)
    {
        if (!_activeSessions.TryGetValue(mesarioId, out var session))
            return Task.FromResult(false);

        var timeout = TimeSpan.FromMinutes(SecurityConstants.SESSION_TIMEOUT_MINUTES);
        return Task.FromResult(DateTime.Now - session.LastActivity < timeout);
    }

    public async Task<bool> IsTerminalAvailableAsync()
    {
        return await _terminalStateService.IsTerminalAvailableAsync();
    }

    public async Task<string?> GetCurrentEleitorAsync()
    {
        return await _terminalStateService.GetCurrentEleitorAsync();
    }

    public async Task<DateTime?> GetLastActivityAsync()
    {
        return await _terminalStateService.GetLastActivityAsync();
    }

    public async Task<bool> HasPermissionAsync(string mesarioId, string action)
    {
        try
        {
            if (!await IsSessionValidAsync(mesarioId))
                return false;

            // Verificar permissões baseadas na ação
            return action switch
            {
                "UNLOCK_TERMINAL" => await CanUnlockTerminalAsync(mesarioId),
                "LOCK_TERMINAL" => await CanLockTerminalAsync(mesarioId),
                "VIEW_TERMINAL_STATUS" => true,
                "MANAGE_SESSION" => true,
                _ => false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission for mesario {MesarioId}, action {Action}", mesarioId, action);
            return false;
        }
    }

    public async Task LogSecurityEventAsync(string mesarioId, string action, bool success, string? details = null)
    {
        try
        {
            await _eventBus.PublishAsync(new MesaActivityEvent
            {
                Activity = action,
                UserId = mesarioId,
                Success = success,
                Details = details
            });

            _logger.LogInformation("Security event: {Action} by {MesarioId}, Success: {Success}, Details: {Details}", 
                action, mesarioId, success, details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging security event for mesario {MesarioId}, action {Action}", mesarioId, action);
        }
    }

    private void InitializeMesarioCredentials()
    {
        _mesarioCredentials["MESARIO001"] = "12345678";
        _mesarioCredentials["MESARIO002"] = "87654321";
        _mesarioCredentials["ADMIN"] = "admin123";
    }

    private class MesaSession
    {
        public string MesarioId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime LastActivity { get; set; } = DateTime.Now;
        public string? TerminalId { get; set; }
        public string? SessionId { get; set; }
    }
}

