using Avalonia.Controls;
using Avalonia.Interactivity;
using UrnaEletronicaFake.UI.Services;

namespace UrnaEletronicaFake.UI.Views;

public partial class ExampleUsageView : Window
{
    private readonly INotificationService _notificationService;
    private readonly IDialogService _dialogService;
    
    public ExampleUsageView()
    {
        InitializeComponent();
        _notificationService = new NotificationService();
        _dialogService = new DialogService();
    }
    
    private void ShowSuccessNotification(object? sender, RoutedEventArgs e)
    {
        _notificationService.ShowSuccess("Operação realizada com sucesso!");
    }
    
    private void ShowWarningNotification(object? sender, RoutedEventArgs e)
    {
        _notificationService.ShowWarning("Atenção: Verifique os dados inseridos.");
    }
    
    private void ShowErrorNotification(object? sender, RoutedEventArgs e)
    {
        _notificationService.ShowError("Erro: Não foi possível processar a solicitação.");
    }
    
    private void ShowInfoNotification(object? sender, RoutedEventArgs e)
    {
        _notificationService.ShowInfo("Informação: Sistema atualizado com sucesso.");
    }
    
    private async void ShowConfirmationDialog(object? sender, RoutedEventArgs e)
    {
        var result = await _dialogService.ShowConfirmationAsync(
            "Confirmação",
            "Deseja realmente executar esta ação?",
            "Sim",
            "Não");
            
        if (result)
        {
            _notificationService.ShowSuccess("Ação confirmada!");
        }
        else
        {
            _notificationService.ShowInfo("Ação cancelada.");
        }
    }
}

