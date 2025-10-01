# Módulos do Sistema UrnaEletronicaFake

## Visão Geral

Este documento descreve a arquitetura modular implementada no sistema UrnaEletronicaFake, com foco especial no módulo Mesa que controla a liberação e bloqueio da urna eletrônica.

## Arquitetura

### Módulo Core (`Modules/Core/`)

O módulo Core fornece a infraestrutura base para todos os outros módulos:

#### Eventos (`Events/TerminalEvents.cs`)
- **TerminalUnlockedEvent**: Disparado quando o terminal é liberado
- **TerminalLockedEvent**: Disparado quando o terminal é bloqueado
- **VoteStartedEvent**: Disparado quando uma votação inicia
- **VoteCompletedEvent**: Disparado quando uma votação é finalizada
- **VoteAbortedEvent**: Disparado quando uma votação é cancelada
- **TerminalErrorEvent**: Disparado quando ocorre um erro no terminal

#### Serviços (`Services/`)

**IEventBus / EventBus**
- Abstração sobre o MediatR para publicação de eventos
- Permite comunicação assíncrona entre módulos
- Implementa padrão Observer para coordenação

**ITerminalStateService / TerminalStateService**
- Gerencia o estado do terminal (bloqueado/liberado)
- Controla identificação de eleitores e mesários
- Publica eventos de mudança de estado
- Mantém histórico de liberações e bloqueios

#### Event Handlers (`EventHandlers/CoreEventHandlers.cs`)
- **TerminalUnlockedEventHandler**: Registra liberações no log
- **TerminalLockedEventHandler**: Registra bloqueios no log
- **VoteStartedEventHandler**: Registra início de votações
- **VoteCompletedEventHandler**: Registra conclusões de votação
- **VoteAbortedEventHandler**: Registra cancelamentos
- **TerminalErrorEventHandler**: Registra erros críticos

### Módulo Mesa (`Modules/Mesa/`)

O módulo Mesa implementa a interface de controle para mesários:

#### ViewModel (`ViewModels/MesaViewModel.cs`)

**Funcionalidades:**
- **Liberação de Terminal**: Permite liberar urna para eleitor específico
- **Bloqueio de Emergência**: Permite bloquear urna manualmente
- **Monitoramento em Tempo Real**: Exibe status atual do terminal
- **Controle de Segurança**: Valida identificação de mesário e eleitor
- **Logging Integrado**: Registra todas as ações para auditoria

**Propriedades Observáveis:**
- `IsTerminalLocked`: Status de bloqueio/liberação
- `EleitorAtualId`: ID do eleitor atual (se liberado)
- `MesarioId`: ID do mesário responsável
- `IdentificacaoEleitor`: Campo para entrada de ID do eleitor
- `UltimaLiberacao`: Timestamp da última liberação
- `UltimoBloqueio`: Timestamp do último bloqueio
- `StatusMessage`: Mensagem de status atual

**Comandos:**
- `LiberarUrnaCommand`: Libera o terminal para votação
- `BloquearUrnaCommand`: Bloqueia o terminal em emergência
- `LimparLogCommand`: Limpa o log de atividades

## Integração com Sistema Legacy

### LegacyIntegrationService

Serviço de integração que mantém compatibilidade com o sistema antigo:
- Implementa `IVotacaoStateService` do sistema legacy
- Converte chamadas síncronas em assíncronas
- Propaga eventos do novo sistema para interfaces antigas
- Permite migração gradual sem quebrar funcionalidades existentes

### MesaIntegrationService

Serviço de demonstração que mostra integração avançada:
- **SimulateVotingProcessAsync**: Simula processo completo de votação
- **HandleEmergencyLockAsync**: Gerencia bloqueios de emergência
- **GenerateAuditReportAsync**: Gera relatórios de auditoria
- **Event Listeners**: Reage a todos os eventos do sistema

## Fluxo de Operação

### 1. Liberação de Terminal
```
Mesário informa ID → Mesário informa ID do Eleitor → 
MesaViewModel.LiberarUrnaAsync() → 
TerminalStateService.UnlockTerminalAsync() → 
TerminalUnlockedEvent → 
Handlers registram logs → 
Interface atualizada
```

### 2. Processo de Votação
```
Terminal liberado → VoteStartedEvent → 
Eleitor vota → VoteCompletedEvent → 
TerminalLockedEvent → Pronto para próximo eleitor
```

### 3. Tratamento de Erros
```
Erro ocorre → TerminalErrorEvent → 
Logs registrados → Interface notificada → 
Ações corretivas (se necessário)
```

## Configuração e Dependências

### Packages Necessários
- **MediatR**: Para padrão Mediator e eventos
- **Microsoft.Extensions.Hosting**: Para DI e logging
- **CommunityToolkit.Mvvm**: Para ViewModels reativas

### Registro de Serviços
```csharp
// Em App.axaml.cs
services.AddCoreModule();        // Registra Event Bus, handlers, etc.
services.AddMesaModule();        // Registra MesaViewModel
services.AddScoped<IMesaIntegrationService, MesaIntegrationService>();
```

## Segurança e Auditoria

### Logs de Segurança
- Todas as ações são registradas com timestamp
- IDs de mesário e eleitor são sempre logados
- Erros são rastreados com detalhes completos
- Relatórios de auditoria podem ser gerados a qualquer momento

### Validações
- Mesário deve se identificar antes de qualquer ação
- Eleitor deve ser identificado para liberação
- Terminal não pode ser liberado duas vezes sem bloqueio
- Comandos têm validação de estado (CanExecute)

### Padrões de Segurança
- Eventos imutáveis com timestamp automático
- Estado centralizado e controlado
- Separação clara entre apresentação e lógica de negócio
- Logging estruturado para análise posterior

## Extensibilidade

### Adicionando Novos Eventos
1. Definir evento em `TerminalEvents.cs`
2. Criar handler em `CoreEventHandlers.cs`
3. Registrar handler no DI
4. Publicar evento via `IEventBus`

### Adicionando Nova Funcionalidade
1. Criar novo módulo seguindo padrão existente
2. Implementar ViewModels com CommunityToolkit.Mvvm
3. Criar extensão de DI específica
4. Integrar com Core via eventos

## Troubleshooting

### Problemas Comuns
1. **Eventos não disparando**: Verificar se handlers estão registrados no DI
2. **Interface não atualizando**: Verificar se propriedades são ObservableProperty
3. **Erros de DI**: Verificar ordem de registro dos módulos
4. **Logs não aparecendo**: Verificar configuração do logging

### Debugging
- Habilitar logs de Debug para rastrear eventos
- Usar breakpoints em handlers de eventos
- Verificar estado do TerminalStateService
- Monitorar console para logs de sistema

## Conclusão

A arquitetura modular implementada fornece:
- **Separação clara de responsabilidades**
- **Comunicação assíncrona via eventos**
- **Logging abrangente para auditoria**
- **Compatibilidade com sistema legacy**
- **Extensibilidade para futuras funcionalidades**
- **Padrões de segurança eleitoral**

O módulo Mesa agora oferece controle completo e seguro da urna eletrônica, mantendo logs detalhados e permitindo integração com outros módulos do sistema.