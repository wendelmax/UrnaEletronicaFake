# Módulo Voting - Sistema UrnaEletronicaFake

## Visão Geral

O módulo Voting implementa o sistema de votação eletrônica usando arquitetura MVVM com eventos assíncronos via MediatR. Esta implementação modular permite melhor separação de responsabilidades e testabilidade.

## Funcionalidades Implementadas

### 1. VotingViewModel
- **INotificationHandler**: Reação a eventos do sistema via MediatR
- **Processamento de Votos**: Válidos, nulos e brancos
- **Comandos**: Digitação, correção e confirmação
- **Estados**: Gerenciamento de estado da interface

### 2. Eventos Terminal
- **TerminalUnlockedEvent**: Disparado quando terminal é liberado
- **TerminalLockedEvent**: Disparado quando terminal é bloqueado

### 3. Integração MediatR
- Publisher/Subscriber pattern para eventos
- Desacoplamento entre serviços
- Suporte a múltiplos handlers

## Arquitetura

```
Módulos/
├── Core/
│   └── Events/
│       └── TerminalEvents.cs
└── Voting/
    ├── ViewModels/
    │   └── VotingViewModel.cs
    └── DependencyInjection/
        └── VotingServiceCollectionExtensions.cs
```

## Como Usar

### 1. Injeção de Dependência
O módulo é registrado automaticamente via `AddVotingModule()`:

```csharp
services.AddVotingModule();
```

### 2. ViewModel
O VotingViewModel responde automaticamente aos eventos:

```csharp
// O terminal será desbloqueado e o VotingViewModel reagirá
await mediator.Publish(new TerminalUnlockedEvent(eleitorId));

// O terminal será bloqueado e a interface será resetada
await mediator.Publish(new TerminalLockedEvent());
```

### 3. Comandos Disponíveis
- `DigitarNumeroCommand`: Para inserir dígitos
- `CorrigirCommand`: Para corrigir entrada
- `VotarBrancoCommand`: Para voto em branco
- `ConfirmarCommand`: Para confirmar voto

## Melhorias Implementadas

1. **Desacoplamento**: VotingViewModel não depende diretamente de VotacaoStateService
2. **Eventos Assíncronos**: Reação via MediatR permite múltiplos handlers
3. **Modularidade**: Estrutura modular facilita manutenção
4. **Separação de Responsabilidades**: Cada classe tem uma responsabilidade específica
5. **Testabilidade**: Interface permite mock para testes unitários

## Compatibilidade

- ✅ Avalonia UI
- ✅ CommunityToolkit.Mvvm
- ✅ MediatR 12.1.1
- ✅ .NET 9.0
