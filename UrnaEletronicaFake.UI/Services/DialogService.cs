using UrnaEletronicaFake.UI.Models;
using UrnaEletronicaFake.UI.Views;

namespace UrnaEletronicaFake.UI.Services;

public class DialogService : IDialogService
{
    public async Task<bool> ShowConfirmationAsync(
        string message, 
        string title = "Confirmar", 
        string confirmText = "Confirmar",
        string cancelText = "Cancelar",
        ConfirmationType type = ConfirmationType.Warning)
    {
        var dialog = new ConfirmationDialog
        {
            DataContext = new ConfirmationDialogModel
            {
                Title = title,
                Message = message,
                ConfirmText = confirmText,
                CancelText = cancelText,
                Type = type
            }
        };
        
        var result = await dialog.ShowDialog<bool>(App.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null!);
        
        return result;
    }
}


