# Guia Rápido - Novas Features Implementadas

Este guia mostra como usar as novas funcionalidades implementadas no frontend.

---

## 🚀 O Que Foi Implementado

### ✅ 1. Sistema de Notificações Toast
Feedback visual para o usuário sobre ações e erros.

### ✅ 2. Dialogs de Confirmação
Confirmação antes de executar ações críticas.

### ✅ 3. Loading Overlay
Indicador visual durante operações assíncronas.

---

## 💡 Como Usar

### Notificações

```csharp
// 1. Injete no construtor
public AdminViewModel(INotificationService notificationService)
{
    _notificationService = notificationService;
}

// 2. Use nos métodos
_notificationService.ShowSuccess("Operação concluída!");
_notificationService.ShowError("Algo deu errado");
_notificationService.ShowWarning("Atenção!");
_notificationService.ShowInfo("Processando...");
```

### Confirmações

```csharp
// 1. Injete no construtor
public AdminViewModel(IDialogService dialogService)
{
    _dialogService = dialogService;
}

// 2. Use antes de ações destrutivas
var confirmado = await _dialogService.ShowConfirmationAsync(
    message: "Deseja realmente excluir?",
    title: "Confirmar Exclusão",
    type: ConfirmationType.Danger
);

if (confirmado)
{
    // Execute a ação
}
```

### Loading

```csharp
// 1. Adicione propriedades no ViewModel
[ObservableProperty]
private bool _isLoading;

[ObservableProperty]
private string _loadingMessage = "Carregando...";

// 2. Use em operações async
try
{
    IsLoading = true;
    LoadingMessage = "Salvando dados...";
    
    await _service.SalvarAsync();
    
    _notificationService.ShowSuccess("Salvo!");
}
finally
{
    IsLoading = false;
}
```

```xml
<!-- 3. Adicione no XAML -->
<Grid>
    <!-- Seu conteúdo -->
    
    <components:LoadingOverlay DataContext="{Binding}" ZIndex="999"/>
</Grid>
```

---

## 📋 Checklist de Migração

Para atualizar um ViewModel existente:

- [ ] Injetar `INotificationService` no construtor
- [ ] Injetar `IDialogService` no construtor (se necessário)
- [ ] Substituir `StatusMessage` por `_notificationService.Show*()`
- [ ] Adicionar confirmação em métodos de exclusão
- [ ] Adicionar `IsLoading` e `LoadingMessage` properties
- [ ] Envolver operações async com `IsLoading = true/false`
- [ ] Adicionar `LoadingOverlay` no XAML da View

---

## 🎯 Exemplo Completo

```csharp
public partial class MeuViewModel : ViewModelBase
{
    private readonly IMeuService _service;
    private readonly INotificationService _notification;
    private readonly IDialogService _dialog;
    
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _loadingMessage = "";
    
    public MeuViewModel(
        IMeuService service,
        INotificationService notification,
        IDialogService dialog,
        ILogger<MeuViewModel> logger) : base(logger)
    {
        _service = service;
        _notification = notification;
        _dialog = dialog;
    }
    
    [RelayCommand]
    private async Task Salvar()
    {
        try
        {
            IsLoading = true;
            LoadingMessage = "Salvando...";
            
            await _service.SalvarAsync();
            
            _notification.ShowSuccess("Dados salvos com sucesso!");
        }
        catch (Exception ex)
        {
            _notification.ShowError("Erro ao salvar");
            Logger.LogError(ex, "Erro ao salvar");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task Deletar()
    {
        var confirmado = await _dialog.ShowConfirmationAsync(
            "Deseja realmente excluir?",
            type: ConfirmationType.Danger
        );
        
        if (!confirmado) return;
        
        try
        {
            IsLoading = true;
            LoadingMessage = "Excluindo...";
            
            await _service.DeletarAsync();
            
            _notification.ShowSuccess("Excluído com sucesso!");
        }
        catch (Exception ex)
        {
            _notification.ShowError("Erro ao excluir");
            Logger.LogError(ex, "Erro");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

```xml
<UserControl xmlns:components="using:UrnaEletronicaFake.UI.Components">
    <Grid>
        <StackPanel>
            <Button Content="Salvar" Command="{Binding SalvarCommand}"/>
            <Button Content="Deletar" Command="{Binding DeletarCommand}"/>
        </StackPanel>
        
        <components:LoadingOverlay DataContext="{Binding}" ZIndex="999"/>
    </Grid>
</UserControl>
```

---

## 📚 Documentação Completa

- **Detalhes:** `docs/IMPLEMENTED_FEATURES.md`
- **Análise:** `docs/FRONTEND_ANALYSIS_REPORT.md`
- **Guia de Código:** `docs/FRONTEND_IMPROVEMENT_GUIDE.md`

---

**Status:** ✅ Pronto para usar  
**Data:** 01/10/2025


