# Arquitetura Multi-Projeto - UrnaEletronicaFake

## Visão Geral

Esta proposta divide o sistema em projetos separados para melhor organização, manutenibilidade e escalabilidade.

## Estrutura de Projetos

```
UrnaEletronicaFake.sln
├── 📁 UrnaEletronicaFake.Core/
│   ├── Events/                    # Eventos do sistema
│   ├── Services/                  # Serviços base
│   ├── Interfaces/               # Interfaces compartilhadas
│   └── Extensions/               # Extensões de DI
│
├── 📁 UrnaEletronicaFake.Shared/
│   ├── Models/                   # Modelos de dados
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Enums/                    # Enumerações
│   └── Constants/                # Constantes do sistema
│
├── 📁 UrnaEletronicaFake.Voting/
│   ├── ViewModels/               # ViewModels de votação
│   ├── Services/                 # Serviços de votação
│   ├── Events/                   # Eventos específicos
│   └── Extensions/               # Configuração DI
│
├── 📁 UrnaEletronicaFake.Mesa/
│   ├── ViewModels/               # ViewModels da mesa
│   ├── Services/                 # Serviços da mesa
│   ├── Security/                 # Validações de segurança
│   └── Extensions/               # Configuração DI
│
├── 📁 UrnaEletronicaFake.Dashboard/
│   ├── ViewModels/               # ViewModels do dashboard
│   ├── Services/                 # Serviços de monitoramento
│   ├── Reports/                  # Geração de relatórios
│   └── Extensions/               # Configuração DI
│
├── 📁 UrnaEletronicaFake.Administration/
│   ├── ViewModels/               # ViewModels administrativos
│   ├── Services/                 # Serviços administrativos
│   ├── Validation/               # Validações de negócio
│   └── Extensions/               # Configuração DI
│
├── 📁 UrnaEletronicaFake.Audit/
│   ├── Services/                 # Serviços de auditoria
│   ├── Reports/                  # Relatórios de auditoria
│   ├── Storage/                  # Armazenamento de logs
│   └── Extensions/               # Configuração DI
│
├── 📁 UrnaEletronicaFake.Data/
│   ├── DbContext/                # Contexto do Entity Framework
│   ├── Migrations/               # Migrações do banco
│   ├── Repositories/             # Repositórios de dados
│   └── Extensions/               # Configuração DI
│
├── 📁 UrnaEletronicaFake.UI/
│   ├── Views/                    # Views Avalonia
│   ├── ViewModels/               # ViewModels principais
│   ├── Services/                 # Serviços da UI
│   ├── Converters/               # Conversores XAML
│   └── Resources/                # Recursos (estilos, temas)
│
└── 📁 UrnaEletronicaFake.Tests/
    ├── Core.Tests/               # Testes do módulo Core
    ├── Voting.Tests/             # Testes do módulo Voting
    ├── Mesa.Tests/               # Testes do módulo Mesa
    ├── Dashboard.Tests/          # Testes do módulo Dashboard
    ├── Administration.Tests/     # Testes do módulo Administration
    ├── Audit.Tests/              # Testes do módulo Audit
    ├── Data.Tests/               # Testes do módulo Data
    ├── UI.Tests/                 # Testes do módulo UI
    └── Integration.Tests/        # Testes de integração
```

## Dependências entre Projetos

### Hierarquia de Dependências:
```
UI → [Voting, Mesa, Dashboard, Administration, Audit, Data, Core, Shared]
Voting → [Core, Shared, Data]
Mesa → [Core, Shared, Data]
Dashboard → [Core, Shared, Data, Audit]
Administration → [Core, Shared, Data]
Audit → [Core, Shared, Data]
Data → [Core, Shared]
Core → [Shared]
Shared → (sem dependências)
```

## Benefícios da Arquitetura

### 1. **Separação de Responsabilidades**
- Cada projeto tem uma responsabilidade específica
- Facilita manutenção e evolução independente
- Reduz acoplamento entre componentes

### 2. **Compilação e Deploy**
- Compilação incremental mais rápida
- Deploy independente de módulos
- Versionamento separado de componentes

### 3. **Desenvolvimento em Equipe**
- Equipes podem trabalhar em projetos diferentes
- Reduz conflitos de merge
- Facilita code reviews focados

### 4. **Testabilidade**
- Testes isolados por módulo
- Mocking mais eficiente
- Cobertura de código mais precisa

### 5. **Reutilização**
- Módulos podem ser reutilizados em outros projetos
- APIs bem definidas entre projetos
- Facilita criação de SDKs

## Configuração dos Projetos

### 1. **Projeto Core (.NET Standard)**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.4.1" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
  </ItemGroup>
</Project>
```

### 2. **Projeto Shared (.NET Standard)**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
  </ItemGroup>
</Project>
```

### 3. **Projeto UI (.NET 9 Avalonia)**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <UseAvalonia>true</UseAvalonia>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Avalonia" Version="11.0.10" />
    <PackageReference Include="Avalonia.Desktop" Version="11.0.10" />
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\UrnaEletronicaFake.Core\UrnaEletronicaFake.Core.csproj" />
    <ProjectReference Include="..\UrnaEletronicaFake.Shared\UrnaEletronicaFake.Shared.csproj" />
    <!-- Outras referências de projeto -->
  </ItemGroup>
</Project>
```

## Migração Gradual

### Fase 1: Preparação
1. Criar estrutura de projetos
2. Mover código existente para projetos apropriados
3. Configurar dependências entre projetos

### Fase 2: Refatoração
1. Separar interfaces e implementações
2. Implementar padrões de injeção de dependência
3. Criar APIs bem definidas entre projetos

### Fase 3: Otimização
1. Implementar testes por projeto
2. Configurar CI/CD para cada projeto
3. Documentar APIs entre projetos

## Exemplo de Implementação

### Core Project - IEventBus
```csharp
// UrnaEletronicaFake.Core/Interfaces/IEventBus.cs
namespace UrnaEletronicaFake.Core.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T notification, CancellationToken cancellationToken = default)
        where T : INotification;
}
```

### Voting Project - VotingViewModel
```csharp
// UrnaEletronicaFake.Voting/ViewModels/VotingViewModel.cs
using UrnaEletronicaFake.Core.Interfaces;
using UrnaEletronicaFake.Shared.Models;

namespace UrnaEletronicaFake.Voting.ViewModels;

public partial class VotingViewModel : ObservableObject
{
    private readonly IEventBus _eventBus;
    
    public VotingViewModel(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
}
```

### UI Project - App.axaml.cs
```csharp
// UrnaEletronicaFake.UI/App.axaml.cs
using UrnaEletronicaFake.Core.Extensions;
using UrnaEletronicaFake.Voting.Extensions;
using UrnaEletronicaFake.Mesa.Extensions;

public partial class App : Application
{
    private void ConfigureServices(IServiceCollection services)
    {
        services.AddCoreServices();
        services.AddVotingServices();
        services.AddMesaServices();
        // ... outros módulos
    }
}
```

## Considerações de Performance

### 1. **Startup Time**
- Mais projetos = mais assemblies para carregar
- Usar lazy loading quando possível
- Considerar IL trimming para release

### 2. **Memory Usage**
- Cada projeto = assembly separado na memória
- Monitorar uso de memória em produção
- Considerar Assembly.LoadFrom se necessário

### 3. **Build Time**
- Incremental builds são mais eficientes
- Usar dotnet build --no-restore para builds rápidos
- Considerar parallel builds em CI/CD

## Conclusão

A divisão em projetos separados oferece:
- **Melhor organização** do código
- **Facilita manutenção** e evolução
- **Permite desenvolvimento** em equipe
- **Melhora testabilidade** e qualidade
- **Facilita reutilização** de componentes

Esta arquitetura é ideal para projetos de médio a grande porte que precisam de escalabilidade e manutenibilidade a longo prazo.

## Implementação Detalhada por Módulo

### UrnaEletronicaFake.Core

#### Responsabilidades:
- Gerenciamento de eventos do sistema
- Serviços base compartilhados
- Interfaces fundamentais
- Configuração de DI

#### Estrutura Detalhada:
```
UrnaEletronicaFake.Core/
├── Events/
│   ├── TerminalEvents.cs           # Eventos do terminal
│   ├── VotingEvents.cs             # Eventos de votação
│   ├── MesaEvents.cs               # Eventos da mesa
│   └── AuditEvents.cs              # Eventos de auditoria
├── Services/
│   ├── IEventBus.cs                # Interface do barramento de eventos
│   ├── EventBusService.cs          # Implementação do Event Bus
│   ├── ITerminalStateService.cs    # Interface do estado do terminal
│   └── TerminalStateService.cs     # Implementação do estado
├── Interfaces/
│   ├── IService.cs                 # Interface base para serviços
│   ├── IRepository.cs              # Interface base para repositórios
│   └── IValidator.cs               # Interface para validações
└── Extensions/
    ├── CoreServiceCollectionExtensions.cs
    └── EventBusExtensions.cs
```

### UrnaEletronicaFake.Shared

#### Responsabilidades:
- Modelos de dados compartilhados
- DTOs para comunicação
- Enumerações do sistema
- Constantes e configurações

#### Estrutura Detalhada:
```
UrnaEletronicaFake.Shared/
├── Models/
│   ├── Base/
│   │   ├── Entity.cs               # Classe base para entidades
│   │   ├── AuditableEntity.cs      # Entidade auditável
│   │   └── ValueObject.cs          # Objeto de valor
│   ├── Domain/
│   │   ├── Candidato.cs            # Modelo do candidato
│   │   ├── Eleicao.cs              # Modelo da eleição
│   │   ├── Voto.cs                 # Modelo do voto
│   │   └── CargoEleitoral.cs       # Modelo do cargo
│   └── Infrastructure/
│       ├── Auditoria.cs            # Modelo de auditoria
│       └── Terminal.cs             # Modelo do terminal
├── DTOs/
│   ├── Voting/
│   │   ├── VoteRequestDto.cs       # DTO para solicitação de voto
│   │   └── VoteResultDto.cs        # DTO para resultado do voto
│   ├── Mesa/
│   │   ├── MesaConfigDto.cs        # DTO para configuração da mesa
│   │   └── MesaStatusDto.cs        # DTO para status da mesa
│   └── Common/
│       ├── ApiResponseDto.cs       # DTO padrão de resposta
│       └── PaginationDto.cs        # DTO para paginação
├── Enums/
│   ├── VotingStatus.cs             # Status de votação
│   ├── TerminalState.cs            # Estados do terminal
│   ├── AuditAction.cs              # Ações de auditoria
│   └── ElectionType.cs             # Tipos de eleição
└── Constants/
    ├── SystemConstants.cs          # Constantes do sistema
    ├── ValidationMessages.cs       # Mensagens de validação
    └── ConfigurationKeys.cs        # Chaves de configuração
```

### UrnaEletronicaFake.Voting

#### Responsabilidades:
- Lógica de negócio da votação
- Validação de votos
- Processamento de eleições
- Integração com terminal

#### Estrutura Detalhada:
```
UrnaEletronicaFake.Voting/
├── ViewModels/
│   ├── VotingViewModel.cs          # ViewModel principal de votação
│   ├── CandidateSelectionViewModel.cs
│   └── VoteConfirmationViewModel.cs
├── Services/
│   ├── IVotingService.cs           # Interface do serviço de votação
│   ├── VotingService.cs            # Implementação do serviço
│   ├── ICandidateService.cs        # Interface do serviço de candidatos
│   └── CandidateService.cs         # Implementação do serviço
├── Events/
│   ├── VoteStartedEvent.cs         # Evento de início de votação
│   ├── VoteCompletedEvent.cs       # Evento de voto concluído
│   └── VoteCancelledEvent.cs       # Evento de voto cancelado
├── Validators/
│   ├── VoteValidator.cs            # Validador de votos
│   └── CandidateValidator.cs       # Validador de candidatos
└── Extensions/
    └── VotingServiceCollectionExtensions.cs
```

### UrnaEletronicaFake.Mesa

#### Responsabilidades:
- Controle da mesa receptora
- Validação de segurança
- Gerenciamento de sessões
- Auditoria de atividades

#### Estrutura Detalhada:
```
UrnaEletronicaFake.Mesa/
├── ViewModels/
│   ├── MesaViewModel.cs            # ViewModel principal da mesa
│   ├── SessionManagementViewModel.cs
│   └── SecurityViewModel.cs
├── Services/
│   ├── IMesaService.cs             # Interface do serviço da mesa
│   ├── MesaService.cs              # Implementação do serviço
│   ├── IMesaSecurityService.cs     # Interface de segurança
│   └── MesaSecurityService.cs      # Implementação de segurança
├── Security/
│   ├── Validators/
│   │   ├── VoterValidator.cs       # Validador de eleitor
│   │   └── SessionValidator.cs     # Validador de sessão
│   ├── Authentication/
│   │   ├── IAuthenticationService.cs
│   │   └── AuthenticationService.cs
│   └── Authorization/
│       ├── IAuthorizationService.cs
│       └── AuthorizationService.cs
├── Events/
│   ├── MesaUnlockedEvent.cs        # Evento de mesa desbloqueada
│   ├── MesaLockedEvent.cs          # Evento de mesa bloqueada
│   └── SessionStartedEvent.cs      # Evento de sessão iniciada
└── Extensions/
    └── MesaServiceCollectionExtensions.cs
```

### UrnaEletronicaFake.Dashboard

#### Responsabilidades:
- Monitoramento em tempo real
- Geração de relatórios
- Visualização de dados
- Controle administrativo

#### Estrutura Detalhada:
```
UrnaEletronicaFake.Dashboard/
├── ViewModels/
│   ├── DashboardViewModel.cs       # ViewModel principal
│   ├── RealTimeMonitoringViewModel.cs
│   └── ReportsViewModel.cs
├── Services/
│   ├── IDashboardService.cs        # Interface do serviço
│   ├── DashboardService.cs         # Implementação do serviço
│   ├── IReportingService.cs        # Interface de relatórios
│   └── ReportingService.cs         # Implementação de relatórios
├── Reports/
│   ├── Generators/
│   │   ├── VoteReportGenerator.cs  # Gerador de relatório de votos
│   │   ├── AuditReportGenerator.cs # Gerador de relatório de auditoria
│   │   └── StatisticsReportGenerator.cs
│   ├── Templates/
│   │   ├── VoteReportTemplate.cs   # Template de relatório
│   │   └── AuditReportTemplate.cs
│   └── Exporters/
│       ├── PdfExporter.cs          # Exportador para PDF
│       ├── ExcelExporter.cs        # Exportador para Excel
│       └── CsvExporter.cs          # Exportador para CSV
├── Monitoring/
│   ├── RealTimeMonitor.cs          # Monitor em tempo real
│   ├── HealthCheckService.cs       # Verificação de saúde
│   └── PerformanceMonitor.cs       # Monitor de performance
└── Extensions/
    └── DashboardServiceCollectionExtensions.cs
```

### UrnaEletronicaFake.Administration

#### Responsabilidades:
- Gestão de eleições
- Configuração do sistema
- Administração de usuários
- Validação de negócio

#### Estrutura Detalhada:
```
UrnaEletronicaFake.Administration/
├── ViewModels/
│   ├── AdminViewModel.cs           # ViewModel principal
│   ├── ElectionManagementViewModel.cs
│   └── UserManagementViewModel.cs
├── Services/
│   ├── IAdministrationService.cs   # Interface do serviço
│   ├── AdministrationService.cs    # Implementação do serviço
│   ├── IElectionManagementService.cs
│   └── ElectionManagementService.cs
├── Validation/
│   ├── ElectionValidator.cs        # Validador de eleições
│   ├── UserValidator.cs            # Validador de usuários
│   └── ConfigurationValidator.cs   # Validador de configurações
├── Managers/
│   ├── ElectionManager.cs          # Gerenciador de eleições
│   ├── UserManager.cs              # Gerenciador de usuários
│   └── ConfigurationManager.cs     # Gerenciador de configurações
└── Extensions/
    └── AdministrationServiceCollectionExtensions.cs
```

### UrnaEletronicaFake.Audit

#### Responsabilidades:
- Logging de auditoria
- Rastreamento de atividades
- Armazenamento seguro
- Análise de segurança

#### Estrutura Detalhada:
```
UrnaEletronicaFake.Audit/
├── Services/
│   ├── IAuditService.cs            # Interface do serviço
│   ├── AuditService.cs             # Implementação do serviço
│   ├── ILogStorageService.cs       # Interface de armazenamento
│   └── LogStorageService.cs        # Implementação de armazenamento
├── Reports/
│   ├── SecurityAuditReport.cs      # Relatório de segurança
│   ├── ActivityAuditReport.cs      # Relatório de atividades
│   └── ComplianceReport.cs         # Relatório de conformidade
├── Storage/
│   ├── FileAuditStorage.cs         # Armazenamento em arquivo
│   ├── DatabaseAuditStorage.cs     # Armazenamento em banco
│   └── CloudAuditStorage.cs        # Armazenamento em nuvem
├── Analyzers/
│   ├── SecurityAnalyzer.cs         # Analisador de segurança
│   ├── ActivityAnalyzer.cs         # Analisador de atividades
│   └── ComplianceAnalyzer.cs       # Analisador de conformidade
└── Extensions/
    └── AuditServiceCollectionExtensions.cs
```

### UrnaEletronicaFake.Data

#### Responsabilidades:
- Acesso a dados
- Migrações do banco
- Repositórios
- Configuração do Entity Framework

#### Estrutura Detalhada:
```
UrnaEletronicaFake.Data/
├── DbContext/
│   ├── UrnaDbContext.cs            # Contexto principal
│   ├── AuditDbContext.cs           # Contexto de auditoria
│   └── Configuration/
│       ├── CandidatoConfiguration.cs
│       ├── EleicaoConfiguration.cs
│       └── VotoConfiguration.cs
├── Migrations/
│   ├── InitialCreate/              # Migração inicial
│   ├── AddAuditTables/             # Adição de tabelas de auditoria
│   └── UpdateVotingSystem/         # Atualização do sistema
├── Repositories/
│   ├── Base/
│   │   ├── IRepository.cs          # Interface base
│   │   └── Repository.cs           # Implementação base
│   ├── Voting/
│   │   ├── IVotoRepository.cs      # Interface do repositório de votos
│   │   └── VotoRepository.cs       # Implementação do repositório
│   ├── Election/
│   │   ├── IEleicaoRepository.cs   # Interface do repositório de eleições
│   │   └── EleicaoRepository.cs    # Implementação do repositório
│   └── Audit/
│       ├── IAuditoriaRepository.cs # Interface do repositório de auditoria
│       └── AuditoriaRepository.cs  # Implementação do repositório
├── Seeders/
│   ├── DatabaseSeeder.cs           # Semeador principal
│   ├── ElectionSeeder.cs           # Semeador de eleições
│   └── CandidateSeeder.cs          # Semeador de candidatos
└── Extensions/
    ├── DataServiceCollectionExtensions.cs
    └── DatabaseExtensions.cs
```

### UrnaEletronicaFake.UI

#### Responsabilidades:
- Interface do usuário
- Views e ViewModels
- Serviços da UI
- Recursos visuais

#### Estrutura Detalhada:
```
UrnaEletronicaFake.UI/
├── Views/
│   ├── Windows/
│   │   ├── MainWindow.axaml        # Janela principal
│   │   ├── VotingWindow.axaml      # Janela de votação
│   │   ├── MesaWindow.axaml        # Janela da mesa
│   │   └── DashboardWindow.axaml   # Janela do dashboard
│   ├── Pages/
│   │   ├── VotacaoView.axaml       # View de votação
│   │   ├── MesaView.axaml          # View da mesa
│   │   ├── AdminView.axaml         # View administrativa
│   │   ├── AuditoriaView.axaml     # View de auditoria
│   │   └── ResultadosView.axaml    # View de resultados
│   └── Controls/
│       ├── CandidateCard.axaml     # Cartão de candidato
│       ├── VoteButton.axaml        # Botão de voto
│       └── StatusIndicator.axaml   # Indicador de status
├── ViewModels/
│   ├── MainWindowViewModel.cs      # ViewModel principal
│   ├── Base/
│   │   ├── ViewModelBase.cs        # ViewModel base
│   │   └── PageViewModelBase.cs    # ViewModel base de página
│   └── Windows/
│       ├── VotingWindowViewModel.cs
│       ├── MesaWindowViewModel.cs
│       └── DashboardWindowViewModel.cs
├── Services/
│   ├── IWindowManagerService.cs    # Interface do gerenciador de janelas
│   ├── WindowManagerService.cs     # Implementação do gerenciador
│   ├── INavigationService.cs       # Interface de navegação
│   └── NavigationService.cs        # Implementação de navegação
├── Converters/
│   ├── BoolToBackgroundConverter.cs
│   ├── BoolToColorConverter.cs
│   ├── BoolToStatusConverter.cs
│   └── DateTimeToStringConverter.cs
├── Resources/
│   ├── Colors.axaml                # Cores do sistema
│   ├── Styles.axaml                # Estilos globais
│   ├── Themes.axaml                # Temas
│   └── Icons/                      # Ícones do sistema
└── Extensions/
    ├── UIServiceCollectionExtensions.cs
    └── ViewLocatorExtensions.cs
```

## Configuração de Build e Deploy

### Arquivo de Solução (.sln)
```xml
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UrnaEletronicaFake.Core", "UrnaEletronicaFake.Core\UrnaEletronicaFake.Core.csproj", "{GUID1}"
EndProject

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UrnaEletronicaFake.Shared", "UrnaEletronicaFake.Shared\UrnaEletronicaFake.Shared.csproj", "{GUID2}"
EndProject

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UrnaEletronicaFake.Voting", "UrnaEletronicaFake.Voting\UrnaEletronicaFake.Voting.csproj", "{GUID3}"
EndProject

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UrnaEletronicaFake.Mesa", "UrnaEletronicaFake.Mesa\UrnaEletronicaFake.Mesa.csproj", "{GUID4}"
EndProject

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UrnaEletronicaFake.Dashboard", "UrnaEletronicaFake.Dashboard\UrnaEletronicaFake.Dashboard.csproj", "{GUID5}"
EndProject

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UrnaEletronicaFake.Administration", "UrnaEletronicaFake.Administration\UrnaEletronicaFake.Administration.csproj", "{GUID6}"
EndProject

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UrnaEletronicaFake.Audit", "UrnaEletronicaFake.Audit\UrnaEletronicaFake.Audit.csproj", "{GUID7}"
EndProject

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UrnaEletronicaFake.Data", "UrnaEletronicaFake.Data\UrnaEletronicaFake.Data.csproj", "{GUID8}"
EndProject

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UrnaEletronicaFake.UI", "UrnaEletronicaFake.UI\UrnaEletronicaFake.UI.csproj", "{GUID9}"
EndProject

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UrnaEletronicaFake.Tests", "UrnaEletronicaFake.Tests\UrnaEletronicaFake.Tests.csproj", "{GUID10}"
EndProject

Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{GUID1}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{GUID1}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{GUID1}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{GUID1}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
EndGlobal
```

### Configuração de CI/CD

#### GitHub Actions (.github/workflows/build.yml)
```yaml
name: Build and Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    strategy:
      matrix:
        project: 
          - UrnaEletronicaFake.Core
          - UrnaEletronicaFake.Shared
          - UrnaEletronicaFake.Voting
          - UrnaEletronicaFake.Mesa
          - UrnaEletronicaFake.Dashboard
          - UrnaEletronicaFake.Administration
          - UrnaEletronicaFake.Audit
          - UrnaEletronicaFake.Data
          - UrnaEletronicaFake.UI

    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Test
      run: dotnet test --no-build --configuration Release --verbosity normal
    
    - name: Publish
      run: dotnet publish --configuration Release --output ./publish

  integration-tests:
    runs-on: ubuntu-latest
    needs: build
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Run Integration Tests
      run: dotnet test UrnaEletronicaFake.Tests/Integration/ --configuration Release
```

## Scripts de Migração

### PowerShell Script (migrate-to-multi-project.ps1)
```powershell
# Script para migração automática para arquitetura multi-projeto

param(
    [string]$SourcePath = "UrnaEletronicaFake",
    [string]$TargetPath = "UrnaEletronicaFake.MultiProject"
)

Write-Host "Iniciando migração para arquitetura multi-projeto..." -ForegroundColor Green

# Criar estrutura de diretórios
$projects = @(
    "UrnaEletronicaFake.Core",
    "UrnaEletronicaFake.Shared", 
    "UrnaEletronicaFake.Voting",
    "UrnaEletronicaFake.Mesa",
    "UrnaEletronicaFake.Dashboard",
    "UrnaEletronicaFake.Administration",
    "UrnaEletronicaFake.Audit",
    "UrnaEletronicaFake.Data",
    "UrnaEletronicaFake.UI",
    "UrnaEletronicaFake.Tests"
)

foreach ($project in $projects) {
    New-Item -ItemType Directory -Path "$TargetPath\$project" -Force
    Write-Host "Criado diretório: $project" -ForegroundColor Yellow
}

# Mover arquivos por módulo
Write-Host "Movendo arquivos para módulos correspondentes..." -ForegroundColor Green

# Core Module
Move-Item "$SourcePath\Modules\Core\*" "$TargetPath\UrnaEletronicaFake.Core\" -Force

# Shared Module  
Move-Item "$SourcePath\Models\*" "$TargetPath\UrnaEletronicaFake.Shared\Models\" -Force

# Voting Module
Move-Item "$SourcePath\Modules\Voting\*" "$TargetPath\UrnaEletronicaFake.Voting\" -Force

# Mesa Module
Move-Item "$SourcePath\Modules\Mesa\*" "$TargetPath\UrnaEletronicaFake.Mesa\" -Force

# Data Module
Move-Item "$SourcePath\Data\*" "$TargetPath\UrnaEletronicaFake.Data\" -Force

# UI Module
Move-Item "$SourcePath\Views\*" "$TargetPath\UrnaEletronicaFake.UI\Views\" -Force
Move-Item "$SourcePath\ViewModels\*" "$TargetPath\UrnaEletronicaFake.UI\ViewModels\" -Force
Move-Item "$SourcePath\Resources\*" "$TargetPath\UrnaEletronicaFake.UI\Resources\" -Force
Move-Item "$SourcePath\Converters\*" "$TargetPath\UrnaEletronicaFake.UI\Converters\" -Force

Write-Host "Migração concluída com sucesso!" -ForegroundColor Green
Write-Host "Próximos passos:" -ForegroundColor Cyan
Write-Host "1. Configurar referências entre projetos" -ForegroundColor White
Write-Host "2. Atualizar namespaces" -ForegroundColor White
Write-Host "3. Configurar DI containers" -ForegroundColor White
Write-Host "4. Executar testes de integração" -ForegroundColor White
```

## Documentação de APIs

### Event Bus API
```csharp
public interface IEventBus
{
    Task PublishAsync<T>(T notification, CancellationToken cancellationToken = default)
        where T : INotification;
    
    void Subscribe<T>(INotificationHandler<T> handler)
        where T : INotification;
    
    void Unsubscribe<T>(INotificationHandler<T> handler)
        where T : INotification;
}
```

### Terminal State Service API
```csharp
public interface ITerminalStateService
{
    Task<bool> IsTerminalUnlockedAsync();
    Task<bool> UnlockTerminalAsync(string eleitorId);
    Task<bool> LockTerminalAsync();
    Task<string> GetCurrentEleitorIdAsync();
    Task<TerminalState> GetCurrentStateAsync();
}
```

### Voting Service API
```csharp
public interface IVotingService
{
    Task<VoteResult> ProcessVoteAsync(VoteRequest request);
    Task<IEnumerable<Candidato>> GetCandidatesAsync(int cargoId);
    Task<bool> ValidateVoterAsync(string eleitorId);
    Task<bool> CancelVoteAsync(string eleitorId);
}
```

## Monitoramento e Observabilidade

### Logging Estruturado
```csharp
public class StructuredLogger
{
    private readonly ILogger _logger;
    
    public void LogVoteProcessed(string eleitorId, int candidatoId, string cargo)
    {
        _logger.LogInformation("Voto processado: {EleitorId} votou em {CandidatoId} para {Cargo}", 
            eleitorId, candidatoId, cargo);
    }
    
    public void LogSecurityEvent(string eventType, string details)
    {
        _logger.LogWarning("Evento de segurança: {EventType} - {Details}", 
            eventType, details);
    }
}
```

### Métricas de Performance
```csharp
public class PerformanceMetrics
{
    private readonly IMetrics _metrics;
    
    public void RecordVoteProcessingTime(TimeSpan duration)
    {
        _metrics.RecordHistogram("vote_processing_duration", duration.TotalMilliseconds);
    }
    
    public void IncrementVoteCounter()
    {
        _metrics.IncrementCounter("total_votes_processed");
    }
}
```

## Considerações de Segurança

### Criptografia de Dados Sensíveis
```csharp
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
```

### Validação de Integridade
```csharp
public class IntegrityValidator
{
    public bool ValidateVoteIntegrity(Voto voto)
    {
        // Verificar assinatura digital
        // Validar timestamp
        // Verificar hash do voto
        return true;
    }
    
    public bool ValidateSystemIntegrity()
    {
        // Verificar integridade do banco
        // Validar logs de auditoria
        // Verificar configurações críticas
        return true;
    }
}
```

## Testes Automatizados

### Estrutura de Testes
```
UrnaEletronicaFake.Tests/
├── Unit/
│   ├── Core.Tests/
│   │   ├── EventBusServiceTests.cs
│   │   └── TerminalStateServiceTests.cs
│   ├── Voting.Tests/
│   │   ├── VotingServiceTests.cs
│   │   └── VoteValidatorTests.cs
│   └── Mesa.Tests/
│       ├── MesaSecurityServiceTests.cs
│       └── SessionValidatorTests.cs
├── Integration/
│   ├── VotingMesaIntegrationTests.cs
│   ├── DatabaseIntegrationTests.cs
│   └── EventBusIntegrationTests.cs
├── Performance/
│   ├── LoadTests.cs
│   ├── StressTests.cs
│   └── MemoryTests.cs
└── Security/
    ├── AuthenticationTests.cs
    ├── AuthorizationTests.cs
    └── EncryptionTests.cs
```

### Exemplo de Teste de Integração
```csharp
[TestClass]
public class VotingMesaIntegrationTests
{
    private ServiceProvider _serviceProvider;
    private IVotingService _votingService;
    private IMesaService _mesaService;
    private IEventBus _eventBus;
    
    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddCoreServices();
        services.AddVotingServices();
        services.AddMesaServices();
        _serviceProvider = services.BuildServiceProvider();
        
        _votingService = _serviceProvider.GetService<IVotingService>();
        _mesaService = _serviceProvider.GetService<IMesaService>();
        _eventBus = _serviceProvider.GetService<IEventBus>();
    }
    
    [TestMethod]
    public async Task VoteProcessed_ShouldUnlockTerminal()
    {
        // Arrange
        var eleitorId = "12345678901";
        var candidatoId = 1;
        
        // Act
        await _mesaService.UnlockTerminalAsync(eleitorId);
        var voteResult = await _votingService.ProcessVoteAsync(
            new VoteRequest { EleitorId = eleitorId, CandidatoId = candidatoId });
        
        // Assert
        Assert.IsTrue(voteResult.Success);
        Assert.IsTrue(await _mesaService.IsTerminalLockedAsync());
    }
}
```

Esta arquitetura multi-projeto fornece uma base sólida para desenvolvimento escalável, manutenível e testável do sistema UrnaEletronicaFake, seguindo as melhores práticas de engenharia de software e padrões de design modernos.

