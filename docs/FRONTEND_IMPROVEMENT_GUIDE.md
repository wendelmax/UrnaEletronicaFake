# Guia Prático de Melhorias de Frontend

Este documento complementa o `FRONTEND_ANALYSIS_REPORT.md` com exemplos práticos de implementação para as correções prioritárias.

---

## 1. Sistema de Notificações Toast

### Implementação do Serviço

```csharp
// Services/INotificationService.cs
public interface INotificationService
{
    void ShowSuccess(string message, string? title = null, int durationMs = 3000);
    void ShowError(string message, string? title = null, int durationMs = 5000);
    void ShowWarning(string message, string? title = null, int durationMs = 4000);
    void ShowInfo(string message, string? title = null, int durationMs = 3000);
}

// Services/NotificationService.cs
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
    
    // Implementar outros métodos...
}

// Models/NotificationModel.cs
public class NotificationModel
{
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int DurationMs { get; set; }
}

public enum NotificationType
{
    Success,
    Error,
    Warning,
    Info
}
```

### Componente Toast

```xml
<!-- Components/ToastNotification.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="UrnaEletronicaFake.UI.Components.ToastNotification">
    
    <UserControl.Styles>
        <Style Selector="Border.toast">
            <Setter Property="Background" Value="{StaticResource BackgroundElevatedBrush}"/>
            <Setter Property="BorderBrush" Value="{StaticResource BorderLightBrush}"/>
            <Setter Property="BorderThickness" Value="1 1 1 4"/>
            <Setter Property="CornerRadius" Value="{StaticResource RadiusMedium}"/>
            <Setter Property="Padding" Value="16"/>
            <Setter Property="MinWidth" Value="300"/>
            <Setter Property="MaxWidth" Value="400"/>
            <Setter Property="Effect" Value="{StaticResource ShadowLarge}"/>
        </Style>
        
        <Style Selector="Border.toast.success">
            <Setter Property="BorderBrush" Value="{StaticResource SuccessBrush}"/>
        </Style>
        
        <Style Selector="Border.toast.error">
            <Setter Property="BorderBrush" Value="{StaticResource ErrorBrush}"/>
        </Style>
        
        <Style Selector="Border.toast.warning">
            <Setter Property="BorderBrush" Value="{StaticResource WarningBrush}"/>
        </Style>
        
        <Style Selector="Border.toast.info">
            <Setter Property="BorderBrush" Value="{StaticResource InfoBrush}"/>
        </Style>
    </UserControl.Styles>
    
    <Border Classes="toast">
        <Grid ColumnDefinitions="Auto,*,Auto">
            <TextBlock Grid.Column="0" 
                       Text="{Binding Icon}"
                       FontSize="24"
                       Margin="0,0,12,0"
                       VerticalAlignment="Center"/>
            
            <StackPanel Grid.Column="1">
                <TextBlock Text="{Binding Title}"
                           Classes="label"
                           FontWeight="Bold"/>
                <TextBlock Text="{Binding Message}"
                           Classes="body"
                           TextWrapping="Wrap"/>
            </StackPanel>
            
            <Button Grid.Column="2"
                    Content="×"
                    Command="{Binding CloseCommand}"
                    Background="Transparent"
                    BorderThickness="0"
                    Padding="8"
                    FontSize="20"
                    Cursor="Hand"/>
        </Grid>
    </Border>
</UserControl>
```

### Overlay de Notificações

```xml
<!-- Components/NotificationOverlay.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="UrnaEletronicaFake.UI.Components.NotificationOverlay">
    
    <ItemsControl ItemsSource="{Binding Notifications}"
                  VerticalAlignment="Top"
                  HorizontalAlignment="Right"
                  Margin="24">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate>
                <StackPanel Spacing="8"/>
            </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
    </ItemsControl>
</UserControl>
```

### Uso nos ViewModels

```csharp
// Antes
StatusMessage = "Eleição criada com sucesso!";

// Depois
_notificationService.ShowSuccess(
    "A eleição foi criada e está pronta para adicionar candidatos.",
    "Eleição Criada"
);
```

---

## 2. Dialog de Confirmação

### Componente Reutilizável

```xml
<!-- Components/ConfirmationDialog.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="UrnaEletronicaFake.UI.Components.ConfirmationDialog">
    
    <Border Background="#80000000">
        <Border Background="{StaticResource BackgroundElevatedBrush}"
                CornerRadius="{StaticResource RadiusLarge}"
                Padding="32"
                Width="450"
                HorizontalAlignment="Center"
                VerticalAlignment="Center"
                Effect="{StaticResource ShadowLarge}">
            <StackPanel Spacing="24">
                
                <StackPanel Spacing="8" HorizontalAlignment="Center">
                    <TextBlock Text="{Binding Icon}"
                               FontSize="48"
                               HorizontalAlignment="Center"
                               Foreground="{Binding IconColor}"/>
                    
                    <TextBlock Text="{Binding Title}"
                               Classes="subheading"
                               HorizontalAlignment="Center"
                               TextAlignment="Center"/>
                </StackPanel>
                
                <TextBlock Text="{Binding Message}"
                           Classes="body"
                           TextWrapping="Wrap"
                           TextAlignment="Center"
                           Foreground="{StaticResource TextSecondaryBrush}"/>
                
                <Grid ColumnDefinitions="*,16,*">
                    <Button Grid.Column="0"
                            Content="{Binding CancelText}"
                            Classes="secondary"
                            Command="{Binding CancelCommand}"
                            HorizontalAlignment="Stretch"/>
                    
                    <Button Grid.Column="2"
                            Content="{Binding ConfirmText}"
                            Classes="{Binding ConfirmButtonClass}"
                            Command="{Binding ConfirmCommand}"
                            HorizontalAlignment="Stretch"/>
                </Grid>
            </StackPanel>
        </Border>
    </Border>
</UserControl>
```

### Serviço de Dialog

```csharp
// Services/IDialogService.cs
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

public enum ConfirmationType
{
    Warning,
    Danger,
    Info
}

// Services/DialogService.cs
public class DialogService : IDialogService
{
    private readonly INotificationService _notificationService;
    
    public async Task<bool> ShowConfirmationAsync(
        string message, 
        string title = "Confirmar", 
        string confirmText = "Confirmar",
        string cancelText = "Cancelar",
        ConfirmationType type = ConfirmationType.Warning)
    {
        var tcs = new TaskCompletionSource<bool>();
        
        var dialog = new ConfirmationDialog
        {
            DataContext = new ConfirmationDialogViewModel
            {
                Title = title,
                Message = message,
                ConfirmText = confirmText,
                CancelText = cancelText,
                Type = type,
                OnConfirm = () => tcs.SetResult(true),
                OnCancel = () => tcs.SetResult(false)
            }
        };
        
        return await tcs.Task;
    }
}
```

### Uso nos ViewModels

```csharp
// Antes
private async Task DeletarEleicao()
{
    var sucesso = await _eleicaoService.DeletarEleicaoAsync(EleicaoSelecionada.Id);
}

// Depois
private async Task DeletarEleicao()
{
    var confirmado = await _dialogService.ShowConfirmationAsync(
        message: $"Tem certeza que deseja excluir a eleição '{EleicaoSelecionada.Titulo}'? Esta ação não pode ser desfeita.",
        title: "Excluir Eleição",
        confirmText: "Sim, Excluir",
        cancelText: "Cancelar",
        type: ConfirmationType.Danger
    );
    
    if (!confirmado) return;
    
    try
    {
        var sucesso = await _eleicaoService.DeletarEleicaoAsync(EleicaoSelecionada.Id);
        
        if (sucesso)
        {
            _notificationService.ShowSuccess("Eleição excluída com sucesso");
            await CarregarEleicoes();
        }
        else
        {
            _notificationService.ShowError("Não foi possível excluir a eleição");
        }
    }
    catch (Exception ex)
    {
        _notificationService.ShowError($"Erro ao excluir eleição: {ex.Message}");
        Logger.LogError(ex, "Erro ao deletar eleição {EleicaoId}", EleicaoSelecionada.Id);
    }
}
```

---

## 3. Validação Visual de Formulários

### Behavior de Validação

```csharp
// Behaviors/ValidationBehavior.cs
public class ValidationBehavior : Behavior<TextBox>
{
    public static readonly StyledProperty<string?> ValidationErrorProperty =
        AvaloniaProperty.Register<ValidationBehavior, string?>(nameof(ValidationError));
    
    public string? ValidationError
    {
        get => GetValue(ValidationErrorProperty);
        set => SetValue(ValidationErrorProperty, value);
    }
    
    protected override void OnAttached()
    {
        base.OnAttached();
        
        if (AssociatedObject != null)
        {
            AssociatedObject.LostFocus += OnLostFocus;
        }
    }
    
    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.LostFocus -= OnLostFocus;
        }
        
        base.OnDetaching();
    }
    
    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject == null) return;
        
        if (string.IsNullOrWhiteSpace(ValidationError))
        {
            AssociatedObject.BorderBrush = Brushes.Green;
        }
        else
        {
            AssociatedObject.BorderBrush = Brushes.Red;
        }
    }
}
```

### Uso no XAML

```xml
<!-- AdminView.axaml -->
<StackPanel Spacing="4">
    <TextBlock Text="Título da Eleição *" 
               Classes="label"/>
    
    <TextBox Text="{Binding TituloEleicao, Mode=TwoWay}" 
             Watermark="Digite o título">
        <Interaction.Behaviors>
            <behaviors:ValidationBehavior ValidationError="{Binding TituloEleicaoError}"/>
        </Interaction.Behaviors>
    </TextBox>
    
    <TextBlock Text="{Binding TituloEleicaoError}"
               Foreground="{StaticResource ErrorBrush}"
               FontSize="{StaticResource FontSizeS}"
               IsVisible="{Binding TituloEleicaoError, Converter={x:Static StringConverters.IsNotNullOrEmpty}}"/>
</StackPanel>
```

### ViewModel com Validação

```csharp
public partial class AdminViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _tituloEleicao = "";
    
    [ObservableProperty]
    private string? _tituloEleicaoError;
    
    partial void OnTituloEleicaoChanged(string value)
    {
        ValidateTituloEleicao();
    }
    
    private void ValidateTituloEleicao()
    {
        if (string.IsNullOrWhiteSpace(TituloEleicao))
        {
            TituloEleicaoError = "O título é obrigatório";
        }
        else if (TituloEleicao.Length < 3)
        {
            TituloEleicaoError = "O título deve ter pelo menos 3 caracteres";
        }
        else if (TituloEleicao.Length > 100)
        {
            TituloEleicaoError = "O título não pode ter mais de 100 caracteres";
        }
        else
        {
            TituloEleicaoError = null;
        }
    }
}
```

---

## 4. Indicadores de Loading Consistentes

### Overlay de Loading Global

```xml
<!-- Components/LoadingOverlay.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="UrnaEletronicaFake.UI.Components.LoadingOverlay">
    
    <Border Background="#CC000000"
            IsVisible="{Binding IsVisible}">
        <StackPanel HorizontalAlignment="Center"
                    VerticalAlignment="Center"
                    Spacing="16">
            
            <Border Width="64"
                    Height="64"
                    BorderBrush="{StaticResource PrimaryBrush}"
                    BorderThickness="4"
                    CornerRadius="32">
                <Border.Styles>
                    <Style Selector="Border">
                        <Style.Animations>
                            <Animation Duration="0:0:1" 
                                       IterationCount="Infinite">
                                <KeyFrame Cue="0%">
                                    <Setter Property="(Border.RenderTransform).(RotateTransform.Angle)" Value="0"/>
                                </KeyFrame>
                                <KeyFrame Cue="100%">
                                    <Setter Property="(Border.RenderTransform).(RotateTransform.Angle)" Value="360"/>
                                </KeyFrame>
                            </Animation>
                        </Style.Animations>
                    </Style>
                </Border.Styles>
                <Border.RenderTransform>
                    <RotateTransform/>
                </Border.RenderTransform>
            </Border>
            
            <TextBlock Text="{Binding Message}"
                       FontSize="{StaticResource FontSizeL}"
                       Foreground="White"
                       HorizontalAlignment="Center"/>
        </StackPanel>
    </Border>
</UserControl>
```

### Uso nas Views

```xml
<!-- AdminView.axaml -->
<Grid>
    <!-- Conteúdo normal -->
    <Border Classes="card">
        <!-- ... -->
    </Border>
    
    <!-- Loading Overlay -->
    <components:LoadingOverlay IsVisible="{Binding IsLoading}"
                               Message="{Binding LoadingMessage}"/>
</Grid>
```

### ViewModel

```csharp
[ObservableProperty]
private bool _isLoading;

[ObservableProperty]
private string _loadingMessage = "Carregando...";

private async Task CarregarEleicoes()
{
    try
    {
        IsLoading = true;
        LoadingMessage = "Carregando eleições...";
        
        var eleicoes = await _eleicaoService.ObterTodasEleicoesAsync();
        
        Eleicoes.Clear();
        foreach (var eleicao in eleicoes)
        {
            Eleicoes.Add(eleicao);
        }
        
        _notificationService.ShowSuccess($"{Eleicoes.Count} eleições carregadas");
    }
    catch (Exception ex)
    {
        _notificationService.ShowError("Erro ao carregar eleições");
        Logger.LogError(ex, "Erro ao carregar eleições");
    }
    finally
    {
        IsLoading = false;
    }
}
```

---

## 5. Consolidação do Design System

### Arquivo Único de Recursos

```xml
<!-- Resources/DesignTokens.axaml -->
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- ============================================ -->
    <!-- PALETA DE CORES - MATERIAL DESIGN -->
    <!-- ============================================ -->
    
    <!-- Cores Primárias -->
    <Color x:Key="Primary50">#E3F2FD</Color>
    <Color x:Key="Primary100">#BBDEFB</Color>
    <Color x:Key="Primary200">#90CAF9</Color>
    <Color x:Key="Primary300">#64B5F6</Color>
    <Color x:Key="Primary400">#42A5F5</Color>
    <Color x:Key="Primary500">#2196F3</Color>
    <Color x:Key="Primary600">#1E88E5</Color>
    <Color x:Key="Primary700">#1976D2</Color>
    <Color x:Key="Primary800">#1565C0</Color>
    <Color x:Key="Primary900">#0D47A1</Color>
    
    <!-- Cores de Estado -->
    <Color x:Key="Success500">#4CAF50</Color>
    <Color x:Key="Warning500">#FF9800</Color>
    <Color x:Key="Error500">#F44336</Color>
    <Color x:Key="Info500">#2196F3</Color>
    
    <!-- Cores Neutras -->
    <Color x:Key="Gray50">#FAFAFA</Color>
    <Color x:Key="Gray100">#F5F5F5</Color>
    <Color x:Key="Gray200">#EEEEEE</Color>
    <Color x:Key="Gray300">#E0E0E0</Color>
    <Color x:Key="Gray400">#BDBDBD</Color>
    <Color x:Key="Gray500">#9E9E9E</Color>
    <Color x:Key="Gray600">#757575</Color>
    <Color x:Key="Gray700">#616161</Color>
    <Color x:Key="Gray800">#424242</Color>
    <Color x:Key="Gray900">#212121</Color>
    
    <!-- ============================================ -->
    <!-- BRUSHES SEMÂNTICOS -->
    <!-- ============================================ -->
    
    <!-- Backgrounds -->
    <SolidColorBrush x:Key="BackgroundPrimaryBrush" Color="{StaticResource Gray50}"/>
    <SolidColorBrush x:Key="BackgroundSecondaryBrush" Color="White"/>
    <SolidColorBrush x:Key="BackgroundElevatedBrush" Color="White"/>
    
    <!-- Texto -->
    <SolidColorBrush x:Key="TextPrimaryBrush" Color="{StaticResource Gray900}"/>
    <SolidColorBrush x:Key="TextSecondaryBrush" Color="{StaticResource Gray600}"/>
    <SolidColorBrush x:Key="TextDisabledBrush" Color="{StaticResource Gray400}"/>
    <SolidColorBrush x:Key="TextOnPrimaryBrush" Color="White"/>
    
    <!-- Bordas -->
    <SolidColorBrush x:Key="BorderLightBrush" Color="{StaticResource Gray300}"/>
    <SolidColorBrush x:Key="BorderMediumBrush" Color="{StaticResource Gray400}"/>
    <SolidColorBrush x:Key="BorderDarkBrush" Color="{StaticResource Gray600}"/>
    
    <!-- Estados -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource Primary500}"/>
    <SolidColorBrush x:Key="PrimaryDarkBrush" Color="{StaticResource Primary700}"/>
    <SolidColorBrush x:Key="PrimaryLightBrush" Color="{StaticResource Primary300}"/>
    
    <SolidColorBrush x:Key="SuccessBrush" Color="{StaticResource Success500}"/>
    <SolidColorBrush x:Key="WarningBrush" Color="{StaticResource Warning500}"/>
    <SolidColorBrush x:Key="ErrorBrush" Color="{StaticResource Error500}"/>
    <SolidColorBrush x:Key="InfoBrush" Color="{StaticResource Info500}"/>
    
    <!-- ============================================ -->
    <!-- TIPOGRAFIA -->
    <!-- ============================================ -->
    
    <x:Double x:Key="FontSizeXS">10</x:Double>
    <x:Double x:Key="FontSizeS">12</x:Double>
    <x:Double x:Key="FontSizeM">14</x:Double>
    <x:Double x:Key="FontSizeL">16</x:Double>
    <x:Double x:Key="FontSizeXL">20</x:Double>
    <x:Double x:Key="FontSizeXXL">24</x:Double>
    <x:Double x:Key="FontSizeXXXL">32</x:Double>
    
    <!-- ============================================ -->
    <!-- ESPAÇAMENTOS -->
    <!-- ============================================ -->
    
    <Thickness x:Key="SpacingXS">4</Thickness>
    <Thickness x:Key="SpacingS">8</Thickness>
    <Thickness x:Key="SpacingM">16</Thickness>
    <Thickness x:Key="SpacingL">24</Thickness>
    <Thickness x:Key="SpacingXL">32</Thickness>
    <Thickness x:Key="SpacingXXL">48</Thickness>
    
    <!-- ============================================ -->
    <!-- BORDAS ARREDONDADAS -->
    <!-- ============================================ -->
    
    <CornerRadius x:Key="RadiusSmall">4</CornerRadius>
    <CornerRadius x:Key="RadiusMedium">8</CornerRadius>
    <CornerRadius x:Key="RadiusLarge">12</CornerRadius>
    <CornerRadius x:Key="RadiusXLarge">16</CornerRadius>
    <CornerRadius x:Key="RadiusFull">9999</CornerRadius>
    
    <!-- ============================================ -->
    <!-- SOMBRAS -->
    <!-- ============================================ -->
    
    <DropShadowEffect x:Key="ShadowSmall" 
                      BlurRadius="4" 
                      OffsetX="0" 
                      OffsetY="2" 
                      Color="#10000000"/>
    
    <DropShadowEffect x:Key="ShadowMedium" 
                      BlurRadius="8" 
                      OffsetX="0" 
                      OffsetY="4" 
                      Color="#20000000"/>
    
    <DropShadowEffect x:Key="ShadowLarge" 
                      BlurRadius="16" 
                      OffsetX="0" 
                      OffsetY="8" 
                      Color="#30000000"/>
    
    <!-- ============================================ -->
    <!-- TRANSIÇÕES -->
    <!-- ============================================ -->
    
    <Transitions x:Key="DefaultTransitions">
        <TransformOperationsTransition Property="RenderTransform" Duration="0:0:0.2"/>
        <BrushTransition Property="Background" Duration="0:0:0.2"/>
        <BrushTransition Property="BorderBrush" Duration="0:0:0.2"/>
    </Transitions>
    
</ResourceDictionary>
```

### Atualizar App.axaml

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="UrnaEletronicaFake.UI.App"
             RequestedThemeVariant="Default">
    <Application.Styles>
        <FluentTheme />
    </Application.Styles>
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- APENAS UM ARQUIVO DE RECURSOS -->
                <ResourceInclude Source="avares://UrnaEletronicaFake.UI/Resources/DesignTokens.axaml"/>
                <ResourceInclude Source="avares://UrnaEletronicaFake.UI/Resources/ComponentStyles.axaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

---

## 6. Acessibilidade - Suporte a Teclado

### Definir TabIndex

```xml
<!-- AdminView.axaml -->
<StackPanel Spacing="24">
    <TextBox TabIndex="1"
             Text="{Binding TituloEleicao}"
             Watermark="Título da Eleição"/>
    
    <TextBox TabIndex="2"
             Text="{Binding DescricaoEleicao}"
             Watermark="Descrição"/>
    
    <DatePicker TabIndex="3"
                SelectedDate="{Binding DataInicio}"/>
    
    <DatePicker TabIndex="4"
                SelectedDate="{Binding DataFim}"/>
    
    <StackPanel Orientation="Horizontal" Spacing="8">
        <Button TabIndex="6"
                Content="Cancelar"
                Command="{Binding CancelarFormularioCommand}"/>
        
        <Button TabIndex="5"
                Content="Salvar"
                IsDefault="True"
                Command="{Binding SalvarEleicaoCommand}"/>
    </StackPanel>
</StackPanel>
```

### Implementar AccessKeys

```xml
<Button Content="_Nova Eleição"
        Command="{Binding MostrarFormularioCommand}"
        HotKey="Ctrl+N"/>

<Button Content="_Salvar"
        Command="{Binding SalvarCommand}"
        HotKey="Ctrl+S"/>

<Button Content="_Cancelar"
        Command="{Binding CancelarCommand}"
        HotKey="Escape"/>
```

### AutomationProperties

```xml
<TextBox AutomationProperties.LabeledBy="{Binding #TituloLabel}"
         AutomationProperties.Name="Título da Eleição"
         AutomationProperties.HelpText="Digite um título único para identificar a eleição"
         Text="{Binding TituloEleicao}"/>
```

---

## 7. Tratamento de Erros Contextualizado

### Hierarquia de Exceções

```csharp
// Exceptions/ElectionException.cs
public abstract class ElectionException : Exception
{
    public string ErrorCode { get; }
    public string UserMessage { get; }
    
    protected ElectionException(
        string errorCode, 
        string userMessage, 
        string technicalMessage,
        Exception? innerException = null)
        : base(technicalMessage, innerException)
    {
        ErrorCode = errorCode;
        UserMessage = userMessage;
    }
}

// Exceptions/ValidationException.cs
public class ValidationException : ElectionException
{
    public Dictionary<string, string[]> Errors { get; }
    
    public ValidationException(
        Dictionary<string, string[]> errors,
        string userMessage = "Os dados fornecidos são inválidos")
        : base("VALIDATION_ERROR", userMessage, "Validation failed")
    {
        Errors = errors;
    }
}

// Exceptions/EntityNotFoundException.cs
public class EntityNotFoundException : ElectionException
{
    public string EntityType { get; }
    public string EntityId { get; }
    
    public EntityNotFoundException(
        string entityType,
        string entityId)
        : base(
            "ENTITY_NOT_FOUND",
            $"O {entityType} solicitado não foi encontrado.",
            $"{entityType} with ID {entityId} not found")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}
```

### ErrorHandler Centralizado

```csharp
// Services/ErrorHandler.cs
public class ErrorHandler
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<ErrorHandler> _logger;
    
    public void Handle(Exception exception, string context)
    {
        var errorId = Guid.NewGuid().ToString("N")[..8].ToUpper();
        
        _logger.LogError(
            exception,
            "Error {ErrorId} in {Context}: {Message}",
            errorId,
            context,
            exception.Message
        );
        
        string userMessage = exception switch
        {
            ValidationException ve => FormatValidationError(ve),
            EntityNotFoundException enf => enf.UserMessage,
            ElectionException ee => $"{ee.UserMessage} (Código: {ee.ErrorCode})",
            _ => $"Ocorreu um erro inesperado. Código de referência: {errorId}"
        };
        
        _notificationService.ShowError(userMessage, "Erro");
    }
    
    private string FormatValidationError(ValidationException ve)
    {
        var errors = ve.Errors.SelectMany(e => e.Value);
        return string.Join("\n", errors);
    }
}
```

### Uso nos ViewModels

```csharp
private async Task SalvarEleicao()
{
    try
    {
        // Validação
        if (string.IsNullOrWhiteSpace(TituloEleicao))
        {
            throw new ValidationException(
                new Dictionary<string, string[]>
                {
                    ["TituloEleicao"] = new[] { "O título é obrigatório" }
                }
            );
        }
        
        // Lógica de negócio
        await _eleicaoService.CriarEleicaoAsync(novaEleicao);
        
        _notificationService.ShowSuccess("Eleição criada com sucesso!");
    }
    catch (Exception ex)
    {
        _errorHandler.Handle(ex, "AdminViewModel.SalvarEleicao");
    }
}
```

---

## Conclusão

Estes exemplos práticos cobrem as melhorias mais críticas identificadas no relatório de análise. A implementação dessas soluções deve seguir a ordem de prioridade:

1. Sistema de Notificações
2. Dialog de Confirmação
3. Indicadores de Loading
4. Validação Visual
5. Consolidação do Design System
6. Acessibilidade
7. Tratamento de Erros

Cada componente foi projetado para ser reutilizável e seguir as melhores práticas de MVVM e Avalonia UI.


