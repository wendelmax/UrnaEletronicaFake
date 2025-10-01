using UrnaEletronicaFake.UI.Models;

namespace UrnaEletronicaFake.UI.Services;

public class NotificationService : INotificationService
{
    public event Action<NotificationModel>? OnNotificationRequested;
    
    public void ShowSuccess(string message, string? title = null, int durationMs = 3000)
    {
        OnNotificationRequested?.Invoke(new NotificationModel
        {
            Type = NotificationType.Success,
            Title = title ?? "Sucesso",
            Message = message,
            DurationMs = durationMs
        });
    }
    
    public void ShowError(string message, string? title = null, int durationMs = 5000)
    {
        OnNotificationRequested?.Invoke(new NotificationModel
        {
            Type = NotificationType.Error,
            Title = title ?? "Erro",
            Message = message,
            DurationMs = durationMs
        });
    }
    
    public void ShowWarning(string message, string? title = null, int durationMs = 4000)
    {
        OnNotificationRequested?.Invoke(new NotificationModel
        {
            Type = NotificationType.Warning,
            Title = title ?? "Atenção",
            Message = message,
            DurationMs = durationMs
        });
    }
    
    public void ShowInfo(string message, string? title = null, int durationMs = 3000)
    {
        OnNotificationRequested?.Invoke(new NotificationModel
        {
            Type = NotificationType.Info,
            Title = title ?? "Informação",
            Message = message,
            DurationMs = durationMs
        });
    }
}


