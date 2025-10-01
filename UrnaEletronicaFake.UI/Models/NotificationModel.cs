namespace UrnaEletronicaFake.UI.Models;

public class NotificationModel
{
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int DurationMs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public string Icon => Type switch
    {
        NotificationType.Success => "✓",
        NotificationType.Error => "✕",
        NotificationType.Warning => "⚠",
        NotificationType.Info => "ℹ",
        _ => "•"
    };
    
    public string BorderColor => Type switch
    {
        NotificationType.Success => "#4CAF50",
        NotificationType.Error => "#F44336",
        NotificationType.Warning => "#FF9800",
        NotificationType.Info => "#2196F3",
        _ => "#757575"
    };
}

public enum NotificationType
{
    Success,
    Error,
    Warning,
    Info
}


