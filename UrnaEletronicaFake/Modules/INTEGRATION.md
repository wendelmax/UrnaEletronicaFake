# Integração dos Módulos - Mesa e Core

## Visão Geral

Este documento descreve como integrar os módulos Mesa e Core no sistema UrnaEletronicaFake.

## Módulos Implementados

### Módulo Core
- **Event Bus**: Sistema de eventos assíncronos para coordenação entre componentes
- **Terminal State Service**: Gerenciamento centralizado do estado do terminal
- **Event Handlers**: Manipuladores de eventos do sistema
- **Eventos**: Definições de eventos do terminal e mesa

### Módulo Mesa  
- **Mesa ViewModel**: Interface para controle da mesa receptora
- **Mesa Security Service**: Validação de segurança e auditoria
- **Controle de Liberação**: Gerenciamento de acesso à urna
- **Logging Integrado**: Rastreamento detalhado de atividades

## Como Integrar

### 1. Registrar os Serviços

No arquivo `Program.cs` ou `App.axaml.cs`, adicione:

```csharp
using UrnaEletronicaFake.Modules.Mesa.DependencyInjection;

// No método de configuração de serviços:
services.AddMesaModule();
```

### 2. Substituir o MesaViewModel Existente

O novo `MesaViewModel` em `/Modules/Mesa/ViewModels/MesaViewModel.cs` substitui o existente em `/ViewModels/MesaViewModel.cs`.

**Mudanças principais:**
- Integração com Event Bus
- Validação de segurança aprimorada
- Controle de sessão
- Prevenção de votos duplicados
- Logging detalhado de auditoria

### 3. Event Bus - Comunicação entre Componentes

O Event Bus permite comunicação assíncrona entre módulos:

```csharp
// Publicar evento
await _eventBus.PublishAsync(new TerminalUnlockedEvent(eleitorId, DateTime.Now));

// Subscrever eventos
_eventBus.Subscribe<VoteCompletedEvent>(this);
```

### 4. Funcionalidades de Segurança

#### Validação de Eleitor:
- Formato numérico (4-12 dígitos)
- Prevenção de votos duplicados
- Validação de sessão ativa

#### Autorização de Mesário:
- Controle de ações restritas
- Verificação de tempo de sessão
- Logging de tentativas não autorizadas

#### Auditoria:
- Todos os eventos são logados
- Timestamps detalhados
- Rastreamento de atividades da mesa

## Eventos Disponíveis

### Eventos do Terminal:
- `TerminalUnlockedEvent`: Terminal liberado para votação
- `TerminalLockedEvent`: Terminal bloqueado
- `VoteCompletedEvent`: Voto concluído
- `VoteCancelledEvent`: Voto cancelado

### Eventos da Mesa:
- `MesaActivityEvent`: Atividades gerais da mesa
- `TerminalStateRequestEvent`: Solicitação de estado

## Exemplo de Uso

```csharp
public class ExampleViewModel : INotificationHandler<TerminalUnlockedEvent>
{
    private readonly IEventBus _eventBus;
    
    public ExampleViewModel(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _eventBus.Subscribe<TerminalUnlockedEvent>(this);
    }
    
    public Task Handle(TerminalUnlockedEvent notification, CancellationToken cancellationToken)
    {
        // Reagir ao desbloqueio do terminal
        Console.WriteLine($"Terminal desbloqueado para: {notification.EleitorId}");
        return Task.CompletedTask;
    }
}
```

## Benefícios da Arquitetura

1. **Separação de Responsabilidades**: Cada módulo tem função específica
2. **Comunicação Assíncrona**: Event Bus elimina acoplamento direto
3. **Segurança Reforçada**: Validações e auditoria integradas
4. **Extensibilidade**: Fácil adição de novos módulos
5. **Testabilidade**: Componentes isolados e mockáveis
6. **Logging Centralizado**: Rastreamento completo de atividades

## Próximos Passos

1. Integrar com módulo de Votação
2. Implementar módulo de Auditoria
3. Adicionar validação biométrica
4. Implementar backup de segurança
5. Criar dashboard de monitoramento

## Considerações de Segurança

- Senhas ou chaves não devem ser logadas
- Eventos sensíveis devem ser criptografados
- Implementar rate limiting para ações críticas
- Monitorar tentativas de acesso não autorizadas
- Implementar backup regular dos logs de auditoria