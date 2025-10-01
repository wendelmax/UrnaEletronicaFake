namespace UrnaEletronicaFake.UI.Models;

public class ConfirmationDialogModel
{
    public string Title { get; set; } = "Confirmar";
    public string Message { get; set; } = string.Empty;
    public string ConfirmText { get; set; } = "Confirmar";
    public string CancelText { get; set; } = "Cancelar";
    public ConfirmationType Type { get; set; } = ConfirmationType.Warning;
    
    public string Icon => Type switch
    {
        ConfirmationType.Warning => "⚠️",
        ConfirmationType.Danger => "🗑️",
        ConfirmationType.Info => "ℹ️",
        _ => "❓"
    };
    
    public string IconColor => Type switch
    {
        ConfirmationType.Warning => "#FF9800",
        ConfirmationType.Danger => "#F44336",
        ConfirmationType.Info => "#2196F3",
        _ => "#757575"
    };
    
    public string ConfirmButtonClass => Type switch
    {
        ConfirmationType.Danger => "error",
        ConfirmationType.Warning => "warning",
        _ => "primary"
    };
}

public enum ConfirmationType
{
    Warning,
    Danger,
    Info
}


