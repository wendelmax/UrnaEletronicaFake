using UrnaEletronicaFake.UI.Models;

namespace UrnaEletronicaFake.UI.Services;

public interface INotificationService
{
    event Action<NotificationModel>? OnNotificationRequested;
    
    void ShowSuccess(string message, string? title = null, int durationMs = 3000);
    void ShowError(string message, string? title = null, int durationMs = 5000);
    void ShowWarning(string message, string? title = null, int durationMs = 4000);
    void ShowInfo(string message, string? title = null, int durationMs = 3000);
}


