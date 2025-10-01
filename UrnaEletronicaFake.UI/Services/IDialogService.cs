using UrnaEletronicaFake.UI.Models;

namespace UrnaEletronicaFake.UI.Services;

public interface IDialogService
{
    Task<bool> ShowConfirmationAsync(
        string message, 
        string title = "Confirmar", 
        string confirmText = "Confirmar",
        string cancelText = "Cancelar",
        ConfirmationType type = ConfirmationType.Warning
    );
}


