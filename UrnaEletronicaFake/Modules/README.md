# Módulos - UrnaEletronicaFake

## Estrutura Implementada

```
Modules/
├── Core/
│   ├── DependencyInjection/
│   │   └── CoreServiceCollectionExtensions.cs    # Registro de serviços Core
│   ├── Events/
│   │   └── TerminalEvents.cs                      # Eventos do sistema
│   ├── EventHandlers/
│   │   └── CoreEventHandlers.cs                   # Manipuladores de eventos
│   └── Services/
│       ├── IEventBus.cs                           # Interface e implementação Event Bus
│       ├── ITerminalStateService.cs               # Interface do serviço de estado
│       └── TerminalStateService.cs                # Implementação do serviço de estado
├── Mesa/
│   ├── DependencyInjection/
│   │   └── MesaServiceCollectionExtensions.cs     # Registro de serviços Mesa
│   ├── Services/
│   │   └── MesaSecurityService.cs                 # Serviço de segurança da mesa
│   └── ViewModels/
│       └── MesaViewModel.cs                       # ViewModel aprimorado da mesa
├── INTEGRATION.md                                 # Guia de integração
└── README.md                                      # Este arquivo
```

## Funcionalidades Implementadas

### 🔧 Módulo Core

#### Event Bus
- Sistema de eventos assíncronos thread-safe
- Pub/Sub pattern para desacoplamento
- Suporte a múltiplos handlers por evento
- Gerenciamento automático de subscriptions

#### Terminal State Service  
- Gerenciamento centralizado do estado do terminal
- Validação de eleitor integrada
- Publicação automática de eventos de estado
- Logging detalhado de mudanças

#### Event Handlers
- Manipuladores centralizados para eventos do terminal
- Integração com serviços existentes
- Logging automático de atividades

### 🏛️ Módulo Mesa

#### Mesa ViewModel
- **Controle Aprimorado**: Liberação/bloqueio com validação de segurança
- **Event Bus Integration**: Comunicação assíncrona com outros módulos
- **Commands**: AsyncRelayCommand para operações assíncronas
- **Security**: Validação de credenciais e prevenção de votos duplicados
- **Audit Trail**: Logging detalhado de todas as ações

#### Mesa Security Service
- **Validação de Eleitor**: Formato, duplicatas, sessão
- **Autorização de Mesário**: Controle de ações restritas
- **Audit Logging**: Eventos de segurança detalhados
- **Session Management**: Controle de tempo de sessão

## Eventos Disponíveis

| Evento | Descrição | Payload |
|--------|-----------|---------|
| `TerminalUnlockedEvent` | Terminal liberado para votação | EleitorId, Timestamp |
| `TerminalLockedEvent` | Terminal bloqueado | EleitorId?, Timestamp |
| `VoteCompletedEvent` | Voto concluído | EleitorId, VoteId, Timestamp |
| `VoteCancelledEvent` | Voto cancelado | EleitorId, Reason, Timestamp |
| `MesaActivityEvent` | Atividade da mesa | Activity, Details, Timestamp |
| `TerminalStateRequestEvent` | Solicitação de estado | Timestamp |

## Padrões de Segurança

### ✅ Implementados
- Validação de formato de eleitor (4-12 dígitos numéricos)
- Prevenção de votos duplicados
- Controle de sessão com timeout
- Logging de auditoria completo
- Autorização de ações de mesário
- Eventos de segurança detalhados

### 🔒 Recomendações Adicionais
- Implementar criptografia para dados sensíveis
- Adicionar autenticação biométrica
- Rate limiting para ações críticas
- Backup automático de logs
- Monitoramento de tentativas suspeitas

## Como Usar

### 1. Registro no DI Container
```csharp
services.AddMesaModule(); // Adiciona Core + Mesa
```

### 2. Injeção em ViewModels
```csharp
public MyViewModel(IEventBus eventBus, IMesaSecurityService security)
{
    _eventBus = eventBus;
    _security = security;
}
```

### 3. Publicação de Eventos
```csharp
await _eventBus.PublishAsync(new TerminalUnlockedEvent(eleitorId, DateTime.Now));
```

### 4. Subscição a Eventos
```csharp
public class MyHandler : INotificationHandler<TerminalUnlockedEvent>
{
    public Task Handle(TerminalUnlockedEvent notification, CancellationToken cancellationToken)
    {
        // Processar evento
        return Task.CompletedTask;
    }
}
```

## Status da Implementação

| Componente | Status | Notas |
|------------|--------|-------|
| ✅ Event Bus | Completo | Thread-safe, testado |
| ✅ Terminal State Service | Completo | Validação integrada |
| ✅ Event Handlers | Completo | Logging automático |
| ✅ Mesa ViewModel | Completo | Security integrada |
| ✅ Mesa Security Service | Completo | Audit trail completo |
| ✅ Dependency Injection | Completo | Auto-registro |
| ✅ Documentation | Completo | Guias detalhados |

## Próximos Passos

1. **Teste de Integração**: Verificar funcionamento com sistema existente
2. **Performance**: Otimizar Event Bus para alta concorrência
3. **Módulo Audit**: Implementar dashboard de auditoria
4. **Módulo Voting**: Integrar com controle de votação
5. **Módulo Results**: Sistema de apuração integrado

## Arquivos de Configuração

Lembre-se de:
1. Adicionar as dependências do NuGet ao .csproj
2. Registrar os módulos no Program.cs ou App.axaml.cs
3. Atualizar Views para usar novo MesaViewModel
4. Configurar logging se necessário