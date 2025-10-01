using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using UrnaEletronicaFake.Mesa.Services;
using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Mesa.ViewModels;

public partial class MesaViewModel : ObservableObject
{
    private readonly IMesaService _mesaService;
    private readonly ILogger<MesaViewModel> _logger;

    [ObservableProperty]
    private string _mesarioId = string.Empty;

    [ObservableProperty]
    private string _senha = string.Empty;

    [ObservableProperty]
    private string _eleitorId = string.Empty;

    [ObservableProperty]
    private bool _isLoggedIn = false;

    [ObservableProperty]
    private bool _isProcessing = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private TerminalStatusResponse _terminalStatus = new();

    [ObservableProperty]
    private DateTime? _sessionStartTime;

    [ObservableProperty]
    private bool _canUnlockTerminal = false;

    [ObservableProperty]
    private bool _canLockTerminal = false;

    [ObservableProperty]
    private string _currentEleitor = string.Empty;

    [ObservableProperty]
    private DateTime? _lastActivity;

    public MesaViewModel(
        IMesaService mesaService,
        ILogger<MesaViewModel> logger)
    {
        _mesaService = mesaService;
        _logger = logger;
        
    }


    [RelayCommand]
    private async Task LoginAsync()
    {
        try
        {
            IsProcessing = true;
            StatusMessage = "Fazendo login...";

            var success = await _mesaService.LoginAsync(MesarioId, Senha);

            if (success)
            {
                IsLoggedIn = true;
                StatusMessage = "Login realizado com sucesso";
                SessionStartTime = await _mesaService.GetSessionStartTimeAsync(MesarioId);
                
                await RefreshStatusAsync();
                
                _logger.LogInformation("Login successful for mesario {MesarioId}", MesarioId);
            }
            else
            {
                StatusMessage = "Credenciais inválidas";
                _logger.LogWarning("Login failed for mesario {MesarioId}", MesarioId);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro interno do sistema";
            _logger.LogError(ex, "Error during login for mesario {MesarioId}", MesarioId);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        try
        {
            IsProcessing = true;
            StatusMessage = "Fazendo logout...";

            var success = await _mesaService.LogoutAsync(MesarioId);

            if (success)
            {
                IsLoggedIn = false;
                StatusMessage = "Logout realizado com sucesso";
                SessionStartTime = null;
                TerminalStatus = new TerminalStatusResponse();
                
                _logger.LogInformation("Logout successful for mesario {MesarioId}", MesarioId);
            }
            else
            {
                StatusMessage = "Erro ao fazer logout";
                _logger.LogWarning("Logout failed for mesario {MesarioId}", MesarioId);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro interno do sistema";
            _logger.LogError(ex, "Error during logout for mesario {MesarioId}", MesarioId);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private async Task UnlockTerminalAsync()
    {
        try
        {
            IsProcessing = true;
            StatusMessage = "Desbloqueando terminal...";

            var success = await _mesaService.UnlockTerminalAsync(MesarioId, EleitorId);

            if (success)
            {
                StatusMessage = $"Terminal desbloqueado para eleitor {EleitorId}";
                await RefreshStatusAsync();
                
                _logger.LogInformation("Terminal unlocked for eleitor {EleitorId} by mesario {MesarioId}", 
                    EleitorId, MesarioId);
            }
            else
            {
                StatusMessage = "Erro ao desbloquear terminal";
                _logger.LogWarning("Failed to unlock terminal for eleitor {EleitorId} by mesario {MesarioId}", 
                    EleitorId, MesarioId);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro interno do sistema";
            _logger.LogError(ex, "Error unlocking terminal for eleitor {EleitorId} by mesario {MesarioId}", 
                EleitorId, MesarioId);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private async Task LockTerminalAsync()
    {
        try
        {
            IsProcessing = true;
            StatusMessage = "Bloqueando terminal...";

            var success = await _mesaService.LockTerminalAsync(MesarioId);

            if (success)
            {
                StatusMessage = "Terminal bloqueado com sucesso";
                await RefreshStatusAsync();
                
                _logger.LogInformation("Terminal locked by mesario {MesarioId}", MesarioId);
            }
            else
            {
                StatusMessage = "Erro ao bloquear terminal";
                _logger.LogWarning("Failed to lock terminal by mesario {MesarioId}", MesarioId);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro interno do sistema";
            _logger.LogError(ex, "Error locking terminal by mesario {MesarioId}", MesarioId);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshStatusAsync()
    {
        try
        {
            TerminalStatus = await _mesaService.GetTerminalStatusAsync();
            CurrentEleitor = TerminalStatus.CurrentEleitorId ?? string.Empty;
            LastActivity = TerminalStatus.LastActivity;
            
            
            _logger.LogDebug("Status refreshed for mesario {MesarioId}", MesarioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing status for mesario {MesarioId}", MesarioId);
        }
    }

    [RelayCommand]
    private void ClearForm()
    {
        EleitorId = string.Empty;
        StatusMessage = string.Empty;
    }

    private bool CanLogin()
    {
        return !string.IsNullOrWhiteSpace(MesarioId) && 
               !string.IsNullOrWhiteSpace(Senha) && 
               !IsProcessing && 
               !IsLoggedIn;
    }

    private bool CanLogout()
    {
        return !IsProcessing && IsLoggedIn;
    }


    partial void OnMesarioIdChanged(string value)
    {
        LoginCommand.NotifyCanExecuteChanged();
    }

    partial void OnSenhaChanged(string value)
    {
        LoginCommand.NotifyCanExecuteChanged();
    }

    partial void OnEleitorIdChanged(string value)
    {
        UnlockTerminalCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsProcessingChanged(bool value)
    {
        LoginCommand.NotifyCanExecuteChanged();
        LogoutCommand.NotifyCanExecuteChanged();
        UnlockTerminalCommand.NotifyCanExecuteChanged();
        LockTerminalCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsLoggedInChanged(bool value)
    {
        LoginCommand.NotifyCanExecuteChanged();
        LogoutCommand.NotifyCanExecuteChanged();
        UnlockTerminalCommand.NotifyCanExecuteChanged();
        LockTerminalCommand.NotifyCanExecuteChanged();
    }

}