# Exemplos de Uso do Módulo Mesa

## Como usar o novo sistema modular da Mesa

### 1. Operação Básica da Mesa

#### Liberação Normal de Terminal
```csharp
// No MesaViewModel
public async Task LiberarUrnaAsync()
{
    // 1. Mesário informa seu ID
    MesarioId = "MESARIO_001";
    
    // 2. Eleitor se identifica
    IdentificacaoEleitor = "12345678901";
    
    // 3. Mesa libera o terminal
    await _terminalStateService.UnlockTerminalAsync(IdentificacaoEleitor, MesarioId);
    
    // 4. Log automático: "Terminal LIBERADO - Eleitor: 12345678901 | Mesário: MESARIO_001"
    // 5. Interface atualizada automaticamente
}
```

#### Bloqueio de Emergência
```csharp
// Mesário pode bloquear terminal a qualquer momento
await _terminalStateService.LockTerminalAsync("Problema técnico reportado");

// Log automático: "Terminal BLOQUEADO - Motivo: Problema técnico reportado"
```

### 2. Simulação de Processo Completo

```csharp
// Exemplo usando MesaIntegrationService
var mesaService = serviceProvider.GetService<IMesaIntegrationService>();

// Simular processo completo de votação
await mesaService.SimulateVotingProcessAsync("12345678901");

/*
Logs gerados:
[10:15:20] Terminal LAPTOP-123 LIBERADO - Eleitor: 12345678901 | Mesário: MESARIO_001
[10:15:21] VOTAÇÃO INICIADA - Eleitor: 12345678901 | Terminal: LAPTOP-123
[10:15:26] VOTAÇÃO FINALIZADA - Eleitor: 12345678901 | Status: CONFIRMADO | Terminal: LAPTOP-123
[10:15:26] Terminal LAPTOP-123 BLOQUEADO - Motivo: Voto confirmado | Eleitor anterior: 12345678901
*/
```

### 3. Tratamento de Eventos

```csharp
// Em qualquer serviço, você pode escutar eventos
_terminalStateService.OnTerminalUnlocked += async (evento) => {
    Console.WriteLine($"Terminal liberado para {evento.EleitorId} pelo mesário {evento.MesarioId}");
    
    // Sua lógica personalizada aqui
    await MinhaFuncaoPersonalizada(evento);
};

_terminalStateService.OnVoteCompleted += async (evento) => {
    if (evento.VoteConfirmed) {
        Console.WriteLine($"Voto de {evento.EleitorId} foi CONFIRMADO!");
        await RegistrarVotoNoBanco(evento.EleitorId);
    }
};
```

### 4. Geração de Relatórios

```csharp
// Gerar relatório de auditoria
await mesaService.GenerateAuditReportAsync();

/*
Output:
===== RELATÓRIO DE AUDITORIA DA MESA =====
Terminal: LAPTOP-123
Data/Hora: 2025-09-30 10:30:15
Status Atual: BLOQUEADO
Eleitor Atual: Nenhum
Mesário Atual: Nenhum
Última Liberação: 2025-09-30 10:25:20
Último Bloqueio: 2025-09-30 10:29:45
=========================================
*/
```

### 5. Integração com Sistema Legacy

```csharp
// O sistema antigo continua funcionando transparentemente
public class MeuViewModelAntigo : ViewModelBase 
{
    private readonly IVotacaoStateService _votacaoService; // Interface antiga
    
    public MeuViewModelAntigo(IVotacaoStateService votacaoService) 
    {
        _votacaoService = votacaoService; // Na verdade é o LegacyIntegrationService
        
        // Código antigo funciona sem modificação
        _votacaoService.OnTerminalStateChanged += () => {
            // Evento disparado quando novo sistema muda estado
            UpdateUI();
        };
    }
    
    private void LiberarUrna() 
    {
        // Método síncrono antigo
        _votacaoService.UnlockTerminal("12345678901");
        
        // Internamente é convertido para:
        // await _terminalStateService.UnlockTerminalAsync("12345678901", Environment.UserName);
    }
}
```

### 6. Cenários de Erro

```csharp
// Simulando erro durante votação
try {
    await _terminalStateService.HandleVoteStartedAsync("12345678901");
    
    // Simular erro
    throw new InvalidOperationException("Falha na urna eletrônica");
}
catch (Exception ex) {
    // Sistema automaticamente trata o erro
    await _terminalStateService.HandleTerminalErrorAsync($"Erro crítico: {ex.Message}", ex.ToString());
    
    // Log automático: "ERRO NO TERMINAL - Erro crítico: Falha na urna eletrônica | Eleitor: 12345678901"
}
```

### 7. Customização de Interface

```xml
<!-- Na View, novos campos e botões -->
<StackPanel>
    <!-- Campo para ID do Mesário -->
    <TextBox Text="{Binding MesarioId}" Watermark="ID do Mesário"/>
    
    <!-- Campo para ID do Eleitor -->
    <TextBox Text="{Binding IdentificacaoEleitor}" Watermark="CPF do Eleitor"/>
    
    <!-- Botão de Liberação (habilitado apenas se ambos preenchidos) -->
    <Button Content="Liberar Urna" Command="{Binding LiberarUrnaCommand}"/>
    
    <!-- Botão de Emergência (habilitado apenas se terminal liberado) -->
    <Button Content="Bloquear Emergência" Command="{Binding BloquearUrnaCommand}"/>
    
    <!-- Informações em tempo real -->
    <TextBlock Text="{Binding StatusMessage}"/>
    <TextBlock Text="{Binding TerminalId, StringFormat='Terminal: {0}'}"/>
    <TextBlock Text="{Binding UltimaLiberacao, StringFormat='Última Liberação: {0:HH:mm:ss}'}"/>
</StackPanel>
```

### 8. Debugging e Monitoramento

```csharp
// Habilitar logs detalhados
services.AddLogging(builder => {
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug); // Mostra todos os eventos
});

// No código, logs aparecerão automaticamente:
// [INFO] MesaViewModel inicializado para Terminal LAPTOP-123
// [DEBUG] Propagating terminal unlocked event from new service to legacy interface
// [INFO] Terminal unlocked. EleitorId: 12345678901, MesarioId: MESARIO_001, TerminalId: LAPTOP-123
```

### 9. Testes Automatizados

```csharp
// Exemplo de teste unitário
[Test]
public async Task DeveBloquearTerminalAposVotoConfirmado()
{
    // Arrange
    var terminal = new TerminalStateService(eventBus);
    
    // Act
    await terminal.UnlockTerminalAsync("12345678901", "MESARIO_001");
    await terminal.HandleVoteCompletedAsync("12345678901", confirmed: true);
    
    // Assert
    Assert.IsTrue(terminal.IsTerminalLocked);
    Assert.IsNull(terminal.EleitorAutenticadoId);
}
```

### 10. Extensões Futuras

```csharp
// Exemplo: Adicionando autenticação biométrica
public class BiometricEvent : TerminalEvent 
{
    public string BiometricHash { get; init; } = "";
    public bool BiometricMatch { get; init; }
}

// Handler personalizado
public class BiometricHandler : INotificationHandler<BiometricEvent>
{
    public Task Handle(BiometricEvent notification, CancellationToken cancellationToken)
    {
        if (notification.BiometricMatch) {
            _logService.Registrar($"Biometria confirmada: {notification.BiometricHash}");
        } else {
            _logService.Registrar($"ALERTA: Falha na biometria - {notification.BiometricHash}");
        }
        return Task.CompletedTask;
    }
}
```

## Principais Benefícios

1. **Segurança**: Todos os eventos são logados automaticamente
2. **Auditoria**: Histórico completo de todas as ações
3. **Flexibilidade**: Sistema modular permite extensões
4. **Compatibilidade**: Funciona com código legado
5. **Monitoramento**: Logs estruturados para análise
6. **Eventos**: Comunicação assíncrona entre componentes
7. **Testabilidade**: Interfaces bem definidas para testes