# Módulo Core - UrnaEletronicaFake

## Visão Geral
O módulo Core fornece funcionalidades centrais para o sistema de urna eletrônica, incluindo sistema de eventos (Event Bus) baseado em MediatR e gerenciamento de estado do terminal.

## Arquitetura
- **Event Bus**: Sistema de mensageria in-memory usando MediatR
- **Estado do Terminal**: Gerenciamento centralizado do estado do terminal
- **Event Handlers**: Processamento automático de eventos do sistema
- **Dependency Injection**: Configuração modular dos serviços

## Componentes

### 1. Eventos (Events)
- **TerminalStartedEvent**: Disparado quando o terminal é iniciado
- **TerminalStoppedEvent**: Disparado quando o terminal é parado
- **TerminalStateChangedEvent**: Disparado quando o estado do terminal muda
- **TerminalErrorEvent**: Disparado quando ocorre um erro no terminal
- **TerminalLogEvent**: Disparado para registrar logs do terminal

### 2. Serviços (Services)
- **IEventBus**: Interface para publicar eventos e enviar comandos
- **EventBusService**: Implementação do Event Bus usando MediatR
- **ITerminalStateService**: Interface para gerenciar o estado do terminal
- **TerminalStateService**: Implementação do gerenciamento de estado

### 3. Event Handlers (EventHandlers)
- **TerminalStartedEventHandler**: Processa eventos de início do terminal
- **TerminalStoppedEventHandler**: Processa eventos de parada do terminal
- **TerminalStateChangedEventHandler**: Processa mudanças de estado
- **TerminalErrorEventHandler**: Processa eventos de erro
- **TerminalLogEventHandler**: Processa eventos de log

## Uso

### 1. Registrar o módulo
```csharp
// Em App.axaml.cs ou Program.cs
services.AddCoreModule();
```

### 2. Injetar serviços em ViewModels
```csharp
public class MyViewModel : ObservableObject
{
    private readonly IEventBus _eventBus;
    private readonly ITerminalStateService _terminalStateService;

    public MyViewModel(IEventBus eventBus, ITerminalStateService terminalStateService)
    {
        _eventBus = eventBus;
        _terminalStateService = terminalStateService;
    }
}
```

### 3. Publicar eventos
```csharp
await _eventBus.PublishAsync(new TerminalLogEvent
{
    TerminalId = "TERMINAL_001",
    Message = "Operação concluída",
    Level = "Info"
});
```

### 4. Gerenciar estado do terminal
```csharp
// Iniciar terminal
await _terminalStateService.StartAsync();

// Mudar estado
await _terminalStateService.ChangeStateAsync("Voting");

// Parar terminal
await _terminalStateService.StopAsync();
```

## Dependências
- MediatR 12.4.1
- MediatR.Extensions.Microsoft.DependencyInjection 11.1.0
- Microsoft.Extensions.Logging
- Microsoft.Extensions.DependencyInjection

## Exemplo Completo
Veja o arquivo `Examples/CoreModuleUsageExample.cs` para um exemplo completo de uso do módulo Core.