# Relatório de Análise Frontend - Sistema de Urna Eletrônica

**Data:** 01 de Outubro de 2025  
**Analista:** Especialista Frontend - Análise Completa de UI/UX e Features  
**Versão do Sistema:** 1.0.0

---

## Sumário Executivo

Este documento apresenta uma análise minuciosa do frontend da aplicação Sistema de Urna Eletrônica, identificando pontas soltas, inconsistências, problemas de UX e oportunidades de melhorias. O sistema foi construído com Avalonia UI (versão 11.0.10) usando MVVM com CommunityToolkit.

### Status Geral
- **Design System:** Implementado parcialmente com inconsistências
- **Acessibilidade:** Básica, necessita melhorias significativas
- **Responsividade:** Limitada, focada em desktop
- **Features:** Várias funcionalidades incompletas ou ausentes

---

## 1. PROBLEMAS CRÍTICOS DE UI/UX

### 1.1 Sistema de Notificações AUSENTE ⚠️

**Problema:** Não existe um sistema centralizado de notificações toast/snackbar para feedback ao usuário.

**Impacto:**
- Erros são exibidos apenas em `StatusMessage` na barra de status
- Usuários podem não perceber mensagens importantes
- Sem feedback visual para ações bem-sucedidas
- Experiência de usuário comprometida

**Evidência:**
```csharp
// AdminViewModel.cs - Linha 196
StatusMessage = $"Erro ao salvar eleição: {ex.Message}";
```

**Solução Recomendada:**
- Implementar sistema de notificações toast/snackbar
- Criar componente `NotificationOverlay.axaml`
- Adicionar serviço `INotificationService`
- Categorizar notificações: sucesso, erro, aviso, informação

**Prioridade:** CRÍTICA

---

### 1.2 Gestão de Estados de Carregamento Inconsistente

**Problema:** Indicadores de loading são implementados de forma inconsistente entre views.

**Evidência:**
- `LoadingSpinner.axaml` existe mas não é usado em todas as views
- `IsLoading` está presente nos ViewModels mas não renderizado visualmente
- Nenhuma view mostra spinner durante operações assíncronas

**Views Afetadas:**
- `AdminView.axaml` - sem spinner nos formulários
- `DashboardView.axaml` - sem feedback visual ao carregar dados
- `MesaView.axaml` - sem indicação ao liberar urna

**Solução Recomendada:**
```xml
<!-- Adicionar em todas as views com operações async -->
<Border IsVisible="{Binding IsLoading}" 
        Background="#80000000"
        ZIndex="999">
    <components:LoadingSpinner />
</Border>
```

**Prioridade:** ALTA

---

### 1.3 Validação de Formulários Deficiente

**Problema:** Validação ocorre apenas no backend, sem feedback visual nos campos.

**Exemplos Críticos:**

1. **AdminView - Formulário de Eleição:**
```csharp
// Validação só ocorre no Command
if (string.IsNullOrWhiteSpace(TituloEleicao))
{
    StatusMessage = "Digite o título da eleição";
    return;
}
```

**Problemas:**
- Campos não mostram estado de erro visualmente
- Sem mensagens de erro inline abaixo dos campos
- Usuário descobre erro apenas ao tentar salvar
- FluentValidation configurado mas não integrado ao UI

2. **MesaView - Identificação do Eleitor:**
- Sem validação do formato do CPF/Título
- Sem máscaras de input
- Sem feedback de campo obrigatório

**Solução Recomendada:**
- Implementar `ValidationBehavior` para campos
- Adicionar TextBlocks de erro abaixo dos inputs
- Usar cores e ícones para indicar estado (válido/inválido)
- Integrar FluentValidation com UI

**Prioridade:** ALTA

---

### 1.4 Confirmação de Ações Destrutivas AUSENTE

**Problema:** Ações críticas não solicitam confirmação do usuário.

**Exemplos:**

1. **Exclusão de Eleição:**
```csharp
// AdminViewModel.cs - Linha 324
private async Task DeletarEleicao()
{
    // NENHUM DIALOG DE CONFIRMAÇÃO!
    var sucesso = await _eleicaoService.DeletarEleicaoAsync(EleicaoSelecionada.Id);
}
```

2. **Bloqueio de Urna (Emergência):**
- Mesma falta de confirmação
- Pode interromper votação em andamento

3. **Fechar Todas as Janelas:**
```csharp
// MainWindowViewModel.cs - Linha 284
private void FecharTodasJanelas()
{
    _windowManager.CloseAllWindows(); // SEM CONFIRMAÇÃO
}
```

**Solução Recomendada:**
- Criar componente `ConfirmationDialog.axaml`
- Implementar antes de ações destrutivas:
  - Deletar eleição/candidato/cargo
  - Bloquear urna
  - Fechar todas janelas
  - Finalizar eleição

**Prioridade:** CRÍTICA

---

### 1.5 Tratamento de Erros Genérico

**Problema:** Mensagens de erro não são amigáveis ao usuário.

**Evidência:**
```csharp
// VotacaoViewModel.cs - Linha 236
catch (Exception)
{
    Instrucoes = "Erro ao registrar voto. Contate o mesário.";
}
```

**Problemas:**
- Exceções não são logadas com detalhes
- Usuário não sabe o que causou o erro
- Sem códigos de erro para suporte técnico
- Mensagens técnicas expostas ao usuário

**Solução Recomendada:**
- Criar hierarquia de exceções customizadas
- Implementar `ErrorHandler` centralizado
- Gerar códigos de erro únicos
- Mensagens contextualizadas e acionáveis

**Prioridade:** ALTA

---

## 2. PROBLEMAS DE DESIGN SYSTEM

### 2.1 Inconsistência entre Arquivos de Recursos

**Problema:** Três sistemas de cores diferentes coexistem sem padrão claro.

**Arquivos:**
1. `Colors.axaml` - Sistema com cores dinâmicas
2. `DesignSystem.axaml` - Sistema Material Design
3. `ModernStyles.axaml` - Temas customizados
4. `Themes.axaml` - ControlThemes específicos

**Evidências de Conflito:**
```xml
<!-- Colors.axaml -->
<Color x:Key="PrimaryColor">#1976D2</Color>
<Color x:Key="PrimaryColorDynamic">#1976D2</Color>

<!-- DesignSystem.axaml -->
<Color x:Key="Primary500">#2196F3</Color>
```

**Problemas:**
- Cores primárias diferentes entre arquivos
- Nomes de recursos duplicados com valores diferentes
- Impossível manter consistência visual
- `StaticResource` vs `DynamicResource` sem padrão

**Solução Recomendada:**
- Consolidar em um único sistema de design
- Escolher paleta definitiva (sugiro Material Design)
- Documentar sistema de cores
- Remover duplicações

**Prioridade:** ALTA

---

### 2.2 Classes CSS Não Aplicadas Corretamente

**Problema:** Estilos definidos não são usados consistentemente.

**Exemplos:**

1. **Botões:**
```xml
<!-- MainWindow.axaml - Botão sem classe de estilo -->
<Button Content="Configurações" 
        Classes="secondary small"
        Padding="8"/>
<!-- Funciona -->

<!-- VotacaoView.axaml - Botão sem classe de estilo -->
<Button Content="CONFIRMA" 
        Classes="success"
        Height="70" 
        Width="110" />
<!-- Usa classes inline ao invés de style -->
```

2. **Typography:**
- Estilos `.heading`, `.subheading`, `.body`, `.caption` definidos
- Muitos TextBlocks não usam essas classes
- FontSize definido inline ao invés de usar classes

**Solução Recomendada:**
- Auditar todos os componentes
- Aplicar classes de estilo consistentemente
- Remover estilos inline redundantes

**Prioridade:** MÉDIA

---

### 2.3 Falta de Estados Visuais Interativos

**Problema:** Componentes carecem de feedback visual para interações.

**Ausências:**

1. **Loading Spinner sem Animação:**
```xml
<!-- LoadingSpinner.axaml - Linha 16 -->
<Setter Property="RenderTransform">
    <RotateTransform Angle="0"/>
</Setter>
<!-- Sem Animation definida! -->
```

2. **Botões sem Efeito de Ripple:**
- Todos os botões carecem de material ripple effect
- `:pressed` apenas escala, sem animação

3. **Cards sem Transição:**
```xml
<!-- ModernStyles.axaml - Linha 181 -->
<Style Selector="Border.card:pointerover">
    <Setter Property="RenderTransform" Value="scale(1.02)"/>
    <!-- SEM Transition definida! -->
</Style>
```

**Solução Recomendada:**
- Implementar animações com `Transitions`
- Adicionar ripple effect aos botões
- Criar hover states suaves

**Prioridade:** MÉDIA

---

## 3. FUNCIONALIDADES INCOMPLETAS

### 3.1 Janelas de Auditoria e Resultados NÃO IMPLEMENTADAS

**Problema:** WindowManager referencia views inexistentes.

**Evidência:**
```csharp
// WindowManagerService.cs - Linhas 139-147
public void ShowAuditWindow()
{
    _logger.LogWarning("AuditWindow view not implemented yet.");
}

public void ShowResultsWindow()
{
    _logger.LogWarning("ResultsWindow view not implemented yet.");
}
```

**Impacto:**
- Botões no MainWindow não funcionam
- Funcionalidades críticas ausentes
- Sistema incompleto

**Solução Recomendada:**
- Implementar `AuditoriaWindow.axaml` e ViewModel
- Implementar `ResultadosWindow.axaml` e ViewModel
- Conectar com serviços de backend

**Prioridade:** ALTA

---

### 3.2 Dashboard com Dados Mock

**Problema:** DashboardViewModel usa propriedades não vinculadas.

**Evidência:**
```csharp
// DashboardViewModel.cs
[ObservableProperty]
private object? _eleicaoSelecionada;

[ObservableProperty]
private object? _eleicoes;

[ObservableProperty]
private object? _dashboardData;
// Tipo genérico "object?" indica implementação incompleta
```

**DashboardView.axaml:**
```xml
<!-- Bindings para propriedades não definidas -->
<TextBlock Text="{Binding DashboardData.UltimaAtualizacaoFormatada}"/>
<TextBlock Text="{Binding DashboardData.Estatisticas.TotalVotos}"/>
<!-- DashboardData é object?, sem estrutura definida -->
```

**Solução Recomendada:**
- Criar DTOs: `DashboardDataDto`, `EstatisticasDto`
- Implementar propriedades tipadas
- Conectar com serviços reais

**Prioridade:** ALTA

---

### 3.3 Gerenciamento de Cargos Incomp

leto na AdminView

**Problema:** Painel de cargos não é exibido.

**Evidência:**
```csharp
// AdminViewModel.cs
[ObservableProperty]
private bool _mostrarPainelCargos; // Usado mas nunca true no XAML

// AdminView.axaml
<!-- Nenhum overlay para gerenciamento de cargos -->
<!-- Apenas candidatos tem overlay -->
```

**Impacto:**
- Administrador não consegue gerenciar cargos pela UI
- Funcionalidade crítica inacessível

**Solução Recomendada:**
- Criar overlay de gerenciamento de cargos
- Espelhar estrutura do painel de candidatos
- Adicionar CRUD completo

**Prioridade:** ALTA

---

### 3.4 Foto de Candidato Sem Upload

**Problema:** Sistema aceita apenas URLs de imagens.

**Evidência:**
```xml
<!-- AdminView.axaml - Linha 415 -->
<TextBox Text="{Binding FotoCandidato}" 
         Watermark="https://exemplo.com/foto.jpg (opcional)"/>
```

**Limitações:**
- Sem upload local de imagens
- Dependência de URLs externas
- Sem validação de URL de imagem
- Sem tratamento de erros de carregamento

**Solução Recomendada:**
- Implementar upload de arquivo local
- Adicionar preview de imagem
- Validar formatos (JPG, PNG)
- Salvar em storage local/cloud

**Prioridade:** MÉDIA

---

### 3.5 Integração com Mesa Incompleta

**Problema:** Estado compartilhado entre Mesa e Votação não é confiável.

**Evidência:**
```csharp
// VotacaoStateService presumivelmente gerencia estado
// Mas não há sincronização visual entre janelas

// MesaView libera urna
_votacaoStateService.UnlockTerminal(IdentificacaoEleitor);

// VotacaoView recebe evento
private async void OnTerminalStateChanged()
{
    IsTerminalLocked = _votacaoStateService.IsTerminalLocked;
    // Sem feedback de confirmação para Mesa
}
```

**Problemas:**
- Mesa não recebe confirmação de que Votação foi desbloqueada
- Sem indicador de que eleitor iniciou votação
- Estado pode dessincronizar

**Solução Recomendada:**
- Implementar padrão Observer robusto
- Adicionar eventos de confirmação bidirecional
- Criar indicadores visuais de sincronização

**Prioridade:** ALTA

---

## 4. PROBLEMAS DE ACESSIBILIDADE

### 4.1 Falta de Suporte a Teclado

**Problema:** Navegação por teclado não é consistente.

**Ausências:**
- Sem `TabIndex` definido
- Sem `AccessKey` para ações rápidas
- Foco visual pobre
- Atalhos de teclado ausentes

**Impacto:**
- Sistema inacessível para usuários com mobilidade reduzida
- Violação de WCAG 2.1

**Solução Recomendada:**
- Definir ordem de foco lógica
- Implementar atalhos: 
  - `Alt+N` Nova eleição
  - `Ctrl+S` Salvar
  - `Esc` Cancelar
- Melhorar indicador de foco

**Prioridade:** ALTA

---

### 4.2 Contraste de Cores Insuficiente

**Problema:** Várias combinações não atendem WCAG AA.

**Exemplos:**
```xml
<!-- Colors.axaml -->
<Color x:Key="TextHintColor">#757575</Color>
<!-- Contraste 4.6:1 sobre branco - marginal -->

<Color x:Key="PrimaryColor">#1976D2</Color>
<!-- Contraste texto branco: 4.8:1 - marginal para texto pequeno -->
```

**Solução Recomendada:**
- Auditar todas as combinações
- Garantir mínimo 4.5:1 para texto normal
- Garantir mínimo 3:1 para texto grande

**Prioridade:** MÉDIA

---

### 4.3 Sem Suporte a Screen Readers

**Problema:** Elementos não têm labels descritivas.

**Evidência:**
```xml
<!-- VotacaoView.axaml - Botões numéricos -->
<Button Content="1" Classes="keypad-button" 
        Command="{Binding DigitarNumeroCommand}" 
        CommandParameter="1"/>
<!-- Sem AutomationProperties.Name -->
```

**Solução Recomendada:**
```xml
<Button Content="1" 
        AutomationProperties.Name="Número um"
        AutomationProperties.HelpText="Digite o número um"/>
```

**Prioridade:** ALTA

---

## 5. PROBLEMAS DE PERFORMANCE

### 5.1 Sem Virtualização em Listas

**Problema:** ListBox sem virtualização para grandes volumes.

**Evidência:**
```xml
<!-- AdminView.axaml - Linha 79 -->
<ListBox ItemsSource="{Binding Eleicoes}" 
         SelectedItem="{Binding EleicaoSelecionada}">
    <!-- Sem VirtualizingStackPanel -->
</ListBox>
```

**Impacto:**
- Lentidão com muitas eleições/candidatos
- Alto uso de memória

**Solução Recomendada:**
```xml
<ListBox VirtualizingPanel.IsVirtualizing="True"
         VirtualizingPanel.VirtualizationMode="Recycling">
```

**Prioridade:** BAIXA (mas importante para escala)

---

### 5.2 Carregamento Síncrono de Dados

**Problema:** Algumas operações bloqueiam UI thread.

**Evidência:**
```csharp
// AdminViewModel construtor
_ = CarregarEleicoes(); // Fire-and-forget, mas UI pode travar
```

**Solução Recomendada:**
- Garantir todas operações async/await corretas
- Usar Progress<T> para indicadores

**Prioridade:** MÉDIA

---

## 6. PROBLEMAS DE SEGURANÇA

### 6.1 Validação de Input Fraca

**Problema:** SQL Injection e XSS potenciais (embora Entity Framework mitigue).

**Evidência:**
```csharp
// MesaViewModel.cs
[ObservableProperty]
private string _identificacaoEleitor = "";
// Sem sanitização, sem regex validation
```

**Solução Recomendada:**
- Validar formatos (CPF, Título)
- Sanitizar todos os inputs
- Limitar caracteres especiais

**Prioridade:** ALTA

---

### 6.2 Dados Sensíveis no Log

**Problema:** Logs podem conter informações de eleitores.

**Evidência:**
```csharp
// MesaViewModel.cs - Linha 104
_terminalLogService.Registrar($"Eleitor identificado: {IdentificacaoEleitor}");
// CPF/Título em plain text no log
```

**Solução Recomendada:**
- Anonimizar dados pessoais
- Usar hashing para IDs
- LGPD compliance

**Prioridade:** CRÍTICA

---

## 7. MELHORIAS DE UX RECOMENDADAS

### 7.1 Breadcrumbs e Navegação

**Recomendação:** Adicionar breadcrumbs nas views complexas.

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <TextBlock Text="Administração" />
    <TextBlock Text=">" />
    <TextBlock Text="Eleições" FontWeight="Bold"/>
</StackPanel>
```

**Prioridade:** BAIXA

---

### 7.2 Pesquisa e Filtros

**Problema:** AdminView não tem busca de eleições.

**Recomendação:**
- Adicionar campo de busca
- Filtros: Ativa/Inativa, Data
- Ordenação: Nome, Data

**Prioridade:** MÉDIA

---

### 7.3 Paginação

**Problema:** Todas as eleições carregadas de uma vez.

**Recomendação:**
- Implementar paginação
- Load more / infinite scroll
- Limite inicial de 20 itens

**Prioridade:** BAIXA

---

### 7.4 Temas Claro/Escuro

**Problema:** Apenas tema claro disponível.

**Recomendação:**
- Implementar alternância de tema
- Usar recursos dinâmicos existentes
- Persistir preferência

**Prioridade:** BAIXA

---

### 7.5 Undo/Redo

**Recomendação:** Implementar para formulários complexos.

**Prioridade:** BAIXA

---

## 8. ARQUITETURA E CÓDIGO

### 8.1 Separação de Concerns

**Problema:** ViewModels com lógica de negócio.

**Evidência:**
```csharp
// VotacaoViewModel.cs - Linha 140
private void BuscarCandidato()
{
    var candidato = _eleicaoAtiva?.Candidatos
        .FirstOrDefault(c => c.CargoEleitoralId == cargo?.Id && c.Numero == _numeroDigitado);
    // Lógica de busca no ViewModel
}
```

**Recomendação:**
- Mover para serviços
- ViewModel apenas coordena

**Prioridade:** MÉDIA

---

### 8.2 Falta de Testes de UI

**Problema:** Nenhum teste automatizado de UI identificado.

**Recomendação:**
- Implementar testes com Avalonia.Headless
- Testes de integração de ViewModels
- Snapshot testing de views

**Prioridade:** MÉDIA

---

### 8.3 Gestão de Estado Complexa

**Problema:** Estado distribuído entre múltiplas janelas sem source of truth único.

**Recomendação:**
- Implementar state management (Redux-like)
- Centralizar estado da aplicação
- Event sourcing para ações críticas

**Prioridade:** MÉDIA

---

## 9. DOCUMENTAÇÃO

### 9.1 Falta de Documentação de Componentes

**Problema:** Componentes sem comentários XML.

**Recomendação:**
- Adicionar XML docs em todos ViewModels
- Documentar custom components
- Criar guia de style guide

**Prioridade:** BAIXA

---

### 9.2 Sem Design System Documentado

**Problema:** Desenvolvedores não sabem quais recursos usar.

**Recomendação:**
- Criar `DESIGN_SYSTEM.md`
- Documentar paleta de cores
- Showcase de componentes

**Prioridade:** MÉDIA

---

## 10. FEATURES AUSENTES

### 10.1 Histórico de Ações

**Recomendação:**
- Timeline de ações administrativas
- Audit log visual
- Filtros e busca

**Prioridade:** MÉDIA

---

### 10.2 Exportação de Dados

**Problema:** Sem exportação de resultados.

**Recomendação:**
- Exportar para PDF
- Exportar para Excel
- Relatórios customizados

**Prioridade:** MÉDIA

---

### 10.3 Multi-idioma

**Problema:** Sistema apenas em português.

**Recomendação:**
- Implementar i18n
- Suporte a EN, ES

**Prioridade:** BAIXA

---

## RESUMO DE PRIORIDADES

### CRÍTICO (Implementar Imediatamente)
1. ✅ Sistema de notificações toast
2. ✅ Confirmação de ações destrutivas
3. ✅ Anonimização de dados sensíveis em logs
4. ✅ Implementar AuditoriaWindow e ResultadosWindow

### ALTA (Próximo Sprint)
1. ✅ Indicadores de loading consistentes
2. ✅ Validação visual de formulários
3. ✅ Tratamento de erros contextualizado
4. ✅ Consolidar design system
5. ✅ Completar gerenciamento de cargos
6. ✅ Melhorar integração Mesa-Votação
7. ✅ Suporte a teclado e screen readers

### MÉDIA (Backlog)
1. ✅ Aplicar classes CSS consistentemente
2. ✅ Upload de fotos de candidatos
3. ✅ Pesquisa e filtros
4. ✅ Documentar design system
5. ✅ Refatorar lógica de negócio dos ViewModels
6. ✅ Implementar testes de UI

### BAIXA (Nice to Have)
1. ✅ Animações e transições
2. ✅ Breadcrumbs
3. ✅ Paginação
4. ✅ Tema escuro
5. ✅ Multi-idioma

---

## CHECKLIST DE AÇÕES

### UI/UX
- [ ] Criar `NotificationService` e componente toast
- [ ] Implementar `ConfirmationDialog` reutilizável
- [ ] Adicionar `LoadingOverlay` em todas views async
- [ ] Criar sistema de validação visual inline
- [ ] Consolidar arquivos de recursos em sistema único
- [ ] Animar LoadingSpinner
- [ ] Adicionar transitions aos cards e botões
- [ ] Definir TabIndex e AccessKeys
- [ ] Adicionar AutomationProperties
- [ ] Auditar e corrigir contraste de cores

### Funcionalidades
- [ ] Implementar `AuditoriaWindow` completa
- [ ] Implementar `ResultadosWindow` completa
- [ ] Criar DTOs tipados para Dashboard
- [ ] Criar overlay de gerenciamento de cargos
- [ ] Implementar upload de fotos local
- [ ] Melhorar sincronização Mesa-Votação
- [ ] Adicionar campo de busca no AdminView
- [ ] Implementar filtros de eleições

### Código
- [ ] Mover lógica de negócio para serviços
- [ ] Implementar hierarquia de exceções customizadas
- [ ] Adicionar ErrorHandler centralizado
- [ ] Sanitizar e validar todos os inputs
- [ ] Anonimizar dados sensíveis em logs
- [ ] Adicionar virtualização nas ListBox
- [ ] Revisar todos async/await
- [ ] Criar testes de UI com Avalonia.Headless

### Documentação
- [ ] Criar `DESIGN_SYSTEM.md`
- [ ] Adicionar XML docs aos ViewModels
- [ ] Documentar componentes customizados
- [ ] Criar guia de contribuição para UI
- [ ] Documentar convenções de nomenclatura
- [ ] Criar changelog de UI

---

## MÉTRICAS SUGERIDAS

### Qualidade de Código
- Cobertura de testes: 0% → Meta: 70%
- Duplicação de código: Não medido → Meta: <5%
- Complexidade ciclomática: Não medido → Meta: <10

### Performance
- Tempo de carregamento inicial: Não medido → Meta: <2s
- Tempo de resposta de formulários: Não medido → Meta: <100ms
- Uso de memória com 1000 eleições: Não medido → Meta: <500MB

### Acessibilidade
- Conformidade WCAG: Desconhecido → Meta: AA
- Suporte a screen readers: 0% → Meta: 100%
- Navegação por teclado: 30% → Meta: 100%

---

## CONCLUSÃO

O sistema apresenta uma base sólida com Avalonia UI e MVVM, mas carece de polimento e funcionalidades críticas. As principais áreas de melhoria são:

1. **Sistema de Feedback ao Usuário:** Notificações, validações e confirmações
2. **Completude de Features:** Janelas ausentes e funcionalidades incompletas
3. **Acessibilidade:** Suporte a teclado e screen readers
4. **Consistência Visual:** Consolidar design system
5. **Segurança:** Validação e anonimização de dados

Com as correções e melhorias sugeridas, o sistema alcançará um nível profissional de qualidade, adequado para uso em produção.

---

**Próximos Passos:**
1. Priorizar itens CRÍTICOS e ALTOS
2. Criar issues no sistema de controle de versão
3. Estimar esforço e alocar recursos
4. Estabelecer ciclos de revisão de UI/UX
5. Implementar testes contínuos de acessibilidade

---

**Autor:** Análise realizada por especialista Frontend  
**Contato:** Para esclarecimentos sobre este relatório  
**Última Atualização:** 01/10/2025


