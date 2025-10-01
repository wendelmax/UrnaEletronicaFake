# Checklist de Melhorias de Frontend

Este documento serve como guia de acompanhamento para implementação das melhorias identificadas na análise de UI/UX.

---

## ✅ FASE 1: CRÍTICO (Bloqueador para Produção)

### Sistema de Notificações
- [ ] Criar `INotificationService` e implementação
- [ ] Criar model `NotificationModel` com enum `NotificationType`
- [ ] Criar componente `ToastNotification.axaml`
- [ ] Criar `NotificationOverlay.axaml` para container de toasts
- [ ] Registrar serviço no DI container
- [ ] Adicionar overlay em todas as Windows
- [ ] Substituir todos `StatusMessage` por notificações toast
- [ ] Testar com todos os tipos: Success, Error, Warning, Info
- [ ] Implementar auto-dismiss com timer
- [ ] Adicionar animações de entrada/saída

### Confirmação de Ações Destrutivas
- [ ] Criar `IDialogService` interface
- [ ] Implementar `DialogService`
- [ ] Criar componente `ConfirmationDialog.axaml`
- [ ] Criar `ConfirmationDialogViewModel`
- [ ] Registrar no DI
- [ ] Adicionar confirmação em `DeletarEleicao`
- [ ] Adicionar confirmação em `DeletarCandidato`
- [ ] Adicionar confirmação em `DeletarCargo`
- [ ] Adicionar confirmação em `BloquearUrna` (emergência)
- [ ] Adicionar confirmação em `FecharTodasJanelas`
- [ ] Adicionar confirmação em `FinalizarEleicao`
- [ ] Testar fluxo de aceitar e cancelar
- [ ] Adicionar ícones contextuais por tipo (Warning, Danger)

### Anonimização de Dados Sensíveis
- [ ] Criar `DataAnonymizer` utility class
- [ ] Implementar hash de CPF/Título para logs
- [ ] Auditar todos os `TerminalLogService.Registrar()`
- [ ] Remover dados de eleitores de logs
- [ ] Implementar mascaramento de dados na UI (ex: CPF com `***.**1-23`)
- [ ] Adicionar configuração de nível de logging
- [ ] Documentar política de privacidade de logs
- [ ] Testar compliance LGPD

### Views Ausentes
- [ ] Criar `AuditoriaWindow.axaml`
- [ ] Criar `AuditoriaWindow.axaml.cs` code-behind
- [ ] Criar `AuditoriaViewModel.cs`
- [ ] Implementar listagem de auditorias
- [ ] Implementar filtros (data, tipo, usuário)
- [ ] Conectar com `IAuditReportService`
- [ ] Adicionar no `WindowManagerService`
- [ ] Criar `ResultadosWindow.axaml`
- [ ] Criar `ResultadosWindow.axaml.cs` code-behind
- [ ] Criar `ResultadosViewModel.cs`
- [ ] Implementar visualização de resultados
- [ ] Implementar gráficos (barras, pizza)
- [ ] Adicionar exportação para PDF
- [ ] Conectar com serviços de relatórios
- [ ] Adicionar no `WindowManagerService`
- [ ] Testar navegação a partir do MainWindow

---

## ⚠️ FASE 2: ALTA PRIORIDADE (Impacto significativo na UX)

### Indicadores de Loading
- [ ] Criar `LoadingOverlay.axaml` reutilizável
- [ ] Adicionar animação de rotação no spinner
- [ ] Adicionar em `AdminView.axaml`
- [ ] Adicionar em `DashboardView.axaml`
- [ ] Adicionar em `MesaView.axaml`
- [ ] Adicionar em `VotacaoView.axaml`
- [ ] Adicionar propriedade `LoadingMessage` nos ViewModels
- [ ] Implementar estados: Loading, Success, Error
- [ ] Testar com operações longas (3+ segundos)
- [ ] Adicionar skeleton loaders para listas

### Validação Visual de Formulários
- [ ] Instalar pacote FluentValidation (já instalado, verificar)
- [ ] Criar `ValidationBehavior` para TextBox
- [ ] Criar validators com FluentValidation para:
  - [ ] Eleição (título, datas)
  - [ ] Candidato (nome, número, partido)
  - [ ] Cargo (nome, quantidade dígitos)
  - [ ] Eleitor (CPF, Título)
- [ ] Adicionar TextBlocks de erro abaixo dos campos
- [ ] Implementar cores de estado (vermelho=erro, verde=válido)
- [ ] Adicionar ícones de validação
- [ ] Validar em tempo real (on change)
- [ ] Validar ao submeter formulário
- [ ] Desabilitar botão de salvar se inválido
- [ ] Testar todos os cenários de validação

### Tratamento de Erros Contextualizado
- [ ] Criar hierarquia de exceções customizadas
  - [ ] `ElectionException` (base)
  - [ ] `ValidationException`
  - [ ] `EntityNotFoundException`
  - [ ] `UnauthorizedException`
  - [ ] `BusinessRuleViolationException`
- [ ] Criar `ErrorHandler` centralizado
- [ ] Implementar geração de códigos de erro únicos
- [ ] Criar dicionário de mensagens amigáveis
- [ ] Auditar todos try-catch nos ViewModels
- [ ] Substituir mensagens genéricas
- [ ] Adicionar logging estruturado com contexto
- [ ] Implementar retry mechanism para erros transientes
- [ ] Testar cenários de erro comuns

### Consolidar Design System
- [ ] Criar novo arquivo `DesignTokens.axaml`
- [ ] Migrar cores do `Colors.axaml`
- [ ] Migrar espaçamentos
- [ ] Migrar tipografia
- [ ] Migrar sombras
- [ ] Deletar arquivos antigos:
  - [ ] `Colors.axaml`
  - [ ] `Themes.axaml` (mover para ComponentStyles)
- [ ] Atualizar `ModernStyles.axaml`
- [ ] Atualizar todos `StaticResource` para usar novos nomes
- [ ] Criar `ComponentStyles.axaml` para estilos de componentes
- [ ] Documentar sistema de cores em `DESIGN_SYSTEM.md`
- [ ] Criar showcase de componentes
- [ ] Auditar consistência visual em todas as views

### Completar Gerenciamento de Cargos
- [ ] Criar overlay de gerenciamento em `AdminView.axaml`
- [ ] Espelhar estrutura do painel de candidatos
- [ ] Implementar listagem de cargos
- [ ] Implementar formulário de adição/edição
- [ ] Conectar comandos no ViewModel
- [ ] Adicionar validação de ordem (não duplicar)
- [ ] Adicionar validação de quantidade de dígitos (2-5)
- [ ] Implementar reordenação (arrastar e soltar?)
- [ ] Testar CRUD completo
- [ ] Testar com vários cargos

### Melhorar Integração Mesa-Votação
- [ ] Criar eventos de confirmação bidirecional
- [ ] Adicionar indicador visual em MesaView quando votação iniciou
- [ ] Adicionar indicador visual em VotacaoView quando liberada
- [ ] Implementar sincronização em tempo real
- [ ] Adicionar timeout de sessão de votação
- [ ] Adicionar log de eventos de sincronização
- [ ] Testar cenários de dessincronização
- [ ] Implementar mecanismo de reconexão
- [ ] Adicionar notificações de estado

### Acessibilidade - Suporte a Teclado
- [ ] Definir `TabIndex` em todos formulários
- [ ] Implementar `AccessKey` para ações principais:
  - [ ] `Alt+N` - Nova Eleição
  - [ ] `Alt+C` - Nova Candidato
  - [ ] `Alt+G` - Novo Cargo
  - [ ] `Ctrl+S` - Salvar
  - [ ] `Esc` - Cancelar
  - [ ] `Ctrl+F` - Buscar
- [ ] Melhorar indicador visual de foco
- [ ] Testar navegação completa por teclado
- [ ] Implementar atalhos na tela de votação (numérico)
- [ ] Adicionar tooltips com atalhos
- [ ] Documentar atalhos de teclado

### Acessibilidade - Screen Readers
- [ ] Adicionar `AutomationProperties.Name` em todos controles interativos
- [ ] Adicionar `AutomationProperties.HelpText` em campos complexos
- [ ] Adicionar `AutomationProperties.LabeledBy` nos inputs
- [ ] Testar com NVDA (Windows)
- [ ] Testar com Narrator (Windows)
- [ ] Corrigir problemas identificados
- [ ] Adicionar landmarks (regiões)
- [ ] Documentar suporte a acessibilidade

---

## 📊 FASE 3: MÉDIA PRIORIDADE (Melhorias de experiência)

### Aplicar Classes CSS Consistentemente
- [ ] Auditar todos os TextBlocks sem classe
- [ ] Aplicar classes semânticas (heading, subheading, body, caption)
- [ ] Auditar todos os Buttons sem classe
- [ ] Aplicar classes (primary, secondary, success, error, warning)
- [ ] Remover `FontSize` inline redundantes
- [ ] Remover `Foreground` inline redundantes
- [ ] Remover `Padding` inline onde houver classe
- [ ] Criar lint rule para detectar estilos inline
- [ ] Documentar classes disponíveis

### Upload de Fotos de Candidatos
- [ ] Criar `IFileService` para operações de arquivo
- [ ] Implementar seleção de arquivo local
- [ ] Adicionar validação de formato (JPG, PNG, WebP)
- [ ] Adicionar validação de tamanho (max 5MB)
- [ ] Implementar resize automático (max 800x600)
- [ ] Criar storage local para imagens
- [ ] Atualizar formulário de candidato
- [ ] Adicionar preview de imagem selecionada
- [ ] Adicionar botão "Remover foto"
- [ ] Implementar tratamento de erro de carregamento
- [ ] Testar com vários formatos e tamanhos

### Pesquisa e Filtros
- [ ] Adicionar campo de busca em `AdminView`
- [ ] Implementar filtro de texto (título, descrição)
- [ ] Implementar filtro de status (Ativa/Inativa)
- [ ] Implementar filtro de data (Período)
- [ ] Adicionar ordenação (Nome A-Z, Data mais recente)
- [ ] Adicionar contador de resultados
- [ ] Implementar debounce na busca (300ms)
- [ ] Salvar filtros no localStorage
- [ ] Testar com grande volume de dados
- [ ] Adicionar botão "Limpar filtros"

### Refatorar Lógica de Negócio
- [ ] Identificar lógica nos ViewModels
- [ ] Criar serviços específicos:
  - [ ] `ICandidateSearchService`
  - [ ] `IElectionValidationService`
- [ ] Mover lógica de busca de candidatos
- [ ] Mover lógica de validação complexa
- [ ] Simplificar ViewModels
- [ ] Adicionar testes unitários nos serviços
- [ ] Testar integração

### Dashboard com Dados Reais
- [ ] Criar DTOs tipados:
  - [ ] `DashboardDataDto`
  - [ ] `ElectionStatisticsDto`
  - [ ] `CandidateResultDto`
- [ ] Atualizar `DashboardViewModel` com propriedades tipadas
- [ ] Atualizar bindings no `DashboardView.axaml`
- [ ] Conectar com `IDashboardService`
- [ ] Implementar atualização automática (30s)
- [ ] Adicionar gráficos visuais
- [ ] Testar com dados de várias eleições
- [ ] Adicionar export de relatórios

### Documentação
- [ ] Criar `DESIGN_SYSTEM.md` com:
  - [ ] Paleta de cores
  - [ ] Tipografia
  - [ ] Espaçamentos
  - [ ] Componentes
  - [ ] Ícones
- [ ] Adicionar XML docs em todos ViewModels
- [ ] Documentar custom components
- [ ] Criar guia de contribuição para UI
- [ ] Documentar convenções de nomenclatura
- [ ] Criar storybook/showcase de componentes
- [ ] Documentar fluxos de navegação

---

## 🎨 FASE 4: BAIXA PRIORIDADE (Polimento)

### Animações e Transições
- [ ] Implementar animação de rotação no `LoadingSpinner`
- [ ] Adicionar transições de hover nos botões
- [ ] Adicionar transições de hover nos cards
- [ ] Implementar ripple effect nos botões
- [ ] Adicionar animações de entrada nos dialogs
- [ ] Adicionar animações de saída nos toasts
- [ ] Implementar transições de página
- [ ] Testar performance das animações
- [ ] Ajustar duração e easing

### Breadcrumbs
- [ ] Criar componente `Breadcrumb.axaml`
- [ ] Adicionar em `AdminView` (painel de candidatos/cargos)
- [ ] Adicionar serviço de navegação
- [ ] Implementar histórico de navegação
- [ ] Testar navegação profunda

### Paginação
- [ ] Criar componente `Pagination.axaml`
- [ ] Implementar paginação no backend (se necessário)
- [ ] Adicionar em listagem de eleições
- [ ] Adicionar em listagem de candidatos
- [ ] Implementar "Carregar mais"
- [ ] Adicionar opção de itens por página (10, 20, 50)
- [ ] Testar com grandes volumes

### Tema Escuro
- [ ] Criar paleta de cores dark
- [ ] Implementar alternância light/dark
- [ ] Adicionar toggle no menu de configurações
- [ ] Usar `ThemeVariantScope` do Avalonia
- [ ] Testar todas as views em dark mode
- [ ] Ajustar contrastes
- [ ] Salvar preferência do usuário
- [ ] Implementar detecção de tema do sistema

### Undo/Redo
- [ ] Implementar padrão Command
- [ ] Criar `CommandManager` para undo/redo
- [ ] Adicionar em formulários de eleição
- [ ] Adicionar em formulários de candidatos
- [ ] Implementar atalhos (Ctrl+Z, Ctrl+Y)
- [ ] Adicionar indicador visual de estado
- [ ] Limitar histórico (10 ações)

### Virtualização de Listas
- [ ] Habilitar virtualização em `ListBox` de eleições
- [ ] Habilitar virtualização em `ListBox` de candidatos
- [ ] Habilitar virtualização em `ListBox` de cargos
- [ ] Configurar `VirtualizationMode.Recycling`
- [ ] Testar com 1000+ itens
- [ ] Medir impacto de performance

### Multi-idioma (i18n)
- [ ] Escolher biblioteca de i18n (resx ou custom)
- [ ] Criar arquivos de recursos:
  - [ ] `Resources.pt-BR.resx`
  - [ ] `Resources.en-US.resx`
  - [ ] `Resources.es-ES.resx`
- [ ] Extrair todos os textos hardcoded
- [ ] Implementar serviço de localização
- [ ] Adicionar seletor de idioma
- [ ] Testar alternância de idioma em runtime
- [ ] Traduzir para inglês e espanhol

### Features Adicionais
- [ ] Histórico de ações administrativas
- [ ] Timeline visual de eventos
- [ ] Exportação de resultados para PDF
- [ ] Exportação de resultados para Excel
- [ ] Relatórios customizados
- [ ] Impressão de comprovantes
- [ ] Modo offline

---

## 🧪 TESTES

### Testes Unitários
- [ ] Configurar Avalonia.Headless
- [ ] Criar testes para ViewModels principais
- [ ] Testar Commands
- [ ] Testar validação
- [ ] Testar conversores
- [ ] Meta de cobertura: 70%

### Testes de Integração
- [ ] Testar fluxo completo de criação de eleição
- [ ] Testar fluxo completo de votação
- [ ] Testar integração Mesa-Votação
- [ ] Testar sincronização de estado
- [ ] Testar cenários de erro

### Testes de Acessibilidade
- [ ] Testar com NVDA
- [ ] Testar com Narrator
- [ ] Testar navegação por teclado
- [ ] Validar contraste de cores (WCAG AA)
- [ ] Validar ordem de foco
- [ ] Gerar relatório de conformidade

### Testes de Performance
- [ ] Medir tempo de carregamento inicial
- [ ] Medir tempo de resposta de formulários
- [ ] Medir uso de memória com grandes volumes
- [ ] Identificar e corrigir gargalos
- [ ] Otimizar queries ao banco
- [ ] Implementar caching onde apropriado

### Testes de Usabilidade
- [ ] Realizar sessões com usuários reais
- [ ] Coletar feedback
- [ ] Identificar pontos de fricção
- [ ] Implementar melhorias sugeridas
- [ ] Validar com usuários novamente

---

## 📝 MÉTRICAS E ACOMPANHAMENTO

### KPIs de Qualidade
- [ ] Definir baseline de cobertura de testes: ____%
- [ ] Meta de cobertura: 70%
- [ ] Definir baseline de complexidade ciclomática: ____
- [ ] Meta de complexidade: < 10
- [ ] Medir duplicação de código: ____%
- [ ] Meta de duplicação: < 5%

### KPIs de Performance
- [ ] Tempo de carregamento inicial: ____ ms → Meta: < 2000ms
- [ ] Tempo de resposta de formulários: ____ ms → Meta: < 100ms
- [ ] Uso de memória com 1000 eleições: ____ MB → Meta: < 500MB
- [ ] FPS de animações: ____ fps → Meta: 60fps

### KPIs de Acessibilidade
- [ ] Conformidade WCAG: ____ → Meta: AA
- [ ] Suporte a screen readers: ____% → Meta: 100%
- [ ] Navegação por teclado: ____% → Meta: 100%
- [ ] Contraste mínimo: ____ → Meta: 4.5:1

### KPIs de UX
- [ ] Taxa de conclusão de tarefas: ____%
- [ ] Tempo médio para criar eleição: ____ s
- [ ] Tempo médio para adicionar candidato: ____ s
- [ ] Satisfação do usuário (NPS): ____
- [ ] Taxa de erros de usuário: ____%

---

## 📅 CRONOGRAMA SUGERIDO

### Sprint 1 (2 semanas) - CRÍTICO
- Sistema de Notificações
- Confirmação de Ações Destrutivas
- Anonimização de Dados

### Sprint 2 (2 semanas) - CRÍTICO cont.
- AuditoriaWindow
- ResultadosWindow
- Testes básicos

### Sprint 3 (2 semanas) - ALTA
- Indicadores de Loading
- Validação Visual
- Tratamento de Erros

### Sprint 4 (2 semanas) - ALTA cont.
- Consolidar Design System
- Completar Gerenciamento de Cargos
- Melhorar Integração Mesa-Votação

### Sprint 5 (2 semanas) - ALTA cont.
- Acessibilidade - Teclado
- Acessibilidade - Screen Readers
- Testes de Acessibilidade

### Sprint 6 (2 semanas) - MÉDIA
- Classes CSS Consistentes
- Upload de Fotos
- Pesquisa e Filtros

### Sprint 7 (1 semana) - MÉDIA cont.
- Refatorar Lógica de Negócio
- Dashboard com Dados Reais
- Documentação

### Sprint 8+ - BAIXA
- Polimentos conforme necessidade
- Features adicionais
- Otimizações

---

## ✅ CRITÉRIOS DE ACEITAÇÃO

Cada item deve atender:
- [ ] Implementado conforme especificação
- [ ] Testado manualmente
- [ ] Testes automatizados (se aplicável)
- [ ] Revisado por par
- [ ] Documentado
- [ ] Sem regressões
- [ ] Aprovado por stakeholder (se necessário)

---

## 📌 OBSERVAÇÕES

- Marque os checkboxes conforme completar cada item
- Adicione comentários com data de conclusão
- Se um item não for aplicável, marque e justifique
- Priorize itens CRÍTICOS e ALTOS antes de MÉDIOS e BAIXOS
- Realize testes após cada fase
- Documente decisões técnicas importantes

---

**Última Atualização:** 01/10/2025
**Status Geral:** 0% completo (0/200+ itens)


