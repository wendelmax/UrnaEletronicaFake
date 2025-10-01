using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Core.Interfaces;
using UrnaEletronicaFake.Core.Events;
using UrnaEletronicaFake.Mesa.Security;
using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Shared.Constants;
using System.Collections.Concurrent;

namespace UrnaEletronicaFake.Mesa.Services;

public class MesaService : IMesaService
{
    private readonly ILogger<MesaService> _logger;
    private readonly IEventBus _eventBus;
    private readonly ITerminalStateService _terminalStateService;
    private readonly IMesaSecurityService _securityService;
    private readonly ConcurrentDictionary<string, MesaSession> _activeSessions = new();

    public MesaService(
        ILogger<MesaService> logger,
        IEventBus eventBus,
        ITerminalStateService terminalStateService,
        IMesaSecurityService securityService)
    {
        _logger = logger;
        _eventBus = eventBus;
        _terminalStateService = terminalStateService;
        _securityService = securityService;
    }

    public async Task<bool> LoginAsync(string mesarioId, string senha)
    {
        try
        {
            _logger.LogInformation("Attempting login for mesario {MesarioId}", mesarioId);

            if (!await _securityService.ValidateMesarioAsync(mesarioId, senha))
            {
                _logger.LogWarning("Login failed for mesario {MesarioId} - invalid credentials", mesarioId);
                return false;
            }

            var session = new MesaSession
            {
                MesarioId = mesarioId,
                StartTime = DateTime.Now,
                LastActivity = DateTime.Now,
                SessionId = Guid.NewGuid().ToString()
            };

            _activeSessions.AddOrUpdate(mesarioId, session, (_, _) => session);

            await _eventBus.PublishAsync(new MesaActivityEvent
            {
                Activity = "LOGIN",
                UserId = mesarioId,
                Success = true,
                Details = "Login bem-sucedido"
            });

            _logger.LogInformation("Login successful for mesario {MesarioId}", mesarioId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for mesario {MesarioId}", mesarioId);
            return false;
        }
    }

    public async Task<bool> LogoutAsync(string mesarioId)
    {
        try
        {
            _logger.LogInformation("Logging out mesario {MesarioId}", mesarioId);

            if (_activeSessions.TryRemove(mesarioId, out var session))
            {
                await _eventBus.PublishAsync(new MesaActivityEvent
                {
                    Activity = "LOGOUT",
                    UserId = mesarioId,
                    Success = true,
                    Details = $"Sessão encerrada. Duração: {(DateTime.Now - (session.StartTime ?? DateTime.Now)).TotalMinutes:F2} minutos"
                });

                _logger.LogInformation("Logout successful for mesario {MesarioId}", mesarioId);
                return true;
            }

            _logger.LogWarning("Logout failed for mesario {MesarioId} - no active session", mesarioId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for mesario {MesarioId}", mesarioId);
            return false;
        }
    }

    public async Task<bool> UnlockTerminalAsync(string mesarioId, string eleitorId)
    {
        try
        {
            _logger.LogInformation("Attempting to unlock terminal for eleitor {EleitorId} by mesario {MesarioId}", 
                eleitorId, mesarioId);

            if (!await _securityService.CanUnlockTerminalAsync(mesarioId))
            {
                _logger.LogWarning("Terminal unlock denied for mesario {MesarioId}", mesarioId);
                return false;
            }

            if (!await _securityService.ValidateEleitorAsync(eleitorId))
            {
                _logger.LogWarning("Invalid eleitor ID {EleitorId} provided by mesario {MesarioId}", eleitorId, mesarioId);
                return false;
            }

            var success = await _terminalStateService.TryLockTerminalAsync(eleitorId);
            
            if (success)
            {
                await _eventBus.PublishAsync(new MesaActivityEvent
                {
                    Activity = "UNLOCK_TERMINAL",
                    UserId = mesarioId,
                    Success = true,
                    Details = $"Terminal desbloqueado para eleitor {eleitorId}"
                });

                _logger.LogInformation("Terminal unlocked successfully for eleitor {EleitorId} by mesario {MesarioId}", 
                    eleitorId, mesarioId);
            }
            else
            {
                _logger.LogWarning("Failed to unlock terminal for eleitor {EleitorId} by mesario {MesarioId}", 
                    eleitorId, mesarioId);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking terminal for eleitor {EleitorId} by mesario {MesarioId}", 
                eleitorId, mesarioId);
            return false;
        }
    }

    public async Task<bool> LockTerminalAsync(string mesarioId)
    {
        try
        {
            _logger.LogInformation("Attempting to lock terminal by mesario {MesarioId}", mesarioId);

            if (!await _securityService.CanLockTerminalAsync(mesarioId))
            {
                _logger.LogWarning("Terminal lock denied for mesario {MesarioId}", mesarioId);
                return false;
            }

            var success = await _terminalStateService.UnlockTerminalAsync();
            
            if (success)
            {
                await _eventBus.PublishAsync(new MesaActivityEvent
                {
                    Activity = "LOCK_TERMINAL",
                    UserId = mesarioId,
                    Success = true,
                    Details = "Terminal bloqueado"
                });

                _logger.LogInformation("Terminal locked successfully by mesario {MesarioId}", mesarioId);
            }
            else
            {
                _logger.LogWarning("Failed to lock terminal by mesario {MesarioId}", mesarioId);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking terminal by mesario {MesarioId}", mesarioId);
            return false;
        }
    }

    public async Task<TerminalStatusResponse> GetTerminalStatusAsync()
    {
        return await _terminalStateService.GetTerminalStatusAsync();
    }

    public async Task<bool> IsLoggedInAsync(string mesarioId)
    {
        return await _securityService.IsSessionValidAsync(mesarioId);
    }

    public Task<DateTime?> GetSessionStartTimeAsync(string mesarioId)
    {
        if (_activeSessions.TryGetValue(mesarioId, out var session))
        {
            return Task.FromResult(session.StartTime);
        }
        return Task.FromResult<DateTime?>(null);
    }

    public Task<bool> ExtendSessionAsync(string mesarioId)
    {
        try
        {
            if (_activeSessions.TryGetValue(mesarioId, out var session))
            {
                session.LastActivity = DateTime.Now;
                _logger.LogDebug("Session extended for mesario {MesarioId}", mesarioId);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extending session for mesario {MesarioId}", mesarioId);
            return Task.FromResult(false);
        }
    }

    public Task<IEnumerable<AuditEntry>> GetMesaAuditLogAsync(string mesarioId)
    {
        // Em uma implementação real, isso buscaria do repositório de auditoria
        // Por agora, retornamos uma lista vazia
        return Task.FromResult<IEnumerable<AuditEntry>>(new List<AuditEntry>());
    }

    public async Task<bool> CanPerformActionAsync(string mesarioId, string action)
    {
        return await _securityService.HasPermissionAsync(mesarioId, action);
    }

    private class MesaSession
    {
        public string MesarioId { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; } = DateTime.Now;
        public DateTime LastActivity { get; set; } = DateTime.Now;
        public string? TerminalId { get; set; }
        public string? SessionId { get; set; }
    }
}

