# Urna Eletrônica Fake (Multiplataforma)

Um projeto de simulador de urna eletrônica multiplataforma, desenvolvido com Avalonia UI, .NET 9 e SQLite. O sistema emula o processo de votação brasileiro, com interfaces distintas para administração, mesário e eleitor.

## Visão Geral

Este projeto foi modernizado de uma versão antiga em WPF para uma solução moderna e multiplataforma usando Avalonia UI. O objetivo é simular as principais interações de um sistema de votação eletrônica, desde o cadastro de eleições e candidatos até a apuração e auditoria dos votos.

## Arquitetura do Sistema

O sistema utiliza uma **arquitetura de janelas separadas** onde cada funcionalidade é executada em sua própria janela independente, gerenciada pelo **Painel de Controle Central**:

- **MainWindow**: Painel de Controle Central que gerencia todas as outras janelas
- **DashboardWindow**: Exibe estatísticas e informações gerais
- **MesaWindow**: Interface do mesário para liberar a urna
- **VotacaoWindow**: Terminal de votação para o eleitor
- **AdminWindow**: Painel administrativo (integrado na MainWindow)

Esta arquitetura permite melhor organização, controle e experiência do usuário, simulando um ambiente real de urna eletrônica.

## Screenshots do Sistema

### Painel de Controle Central
<img src="docs/images/main-window.png" alt="MainWindow - Painel de Controle Central" width="600" style="max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 8px;">

*Interface principal que gerencia todas as janelas do sistema, com status em tempo real das janelas abertas.*

### Dashboard
<img src="docs/images/dashboard.png" alt="Dashboard - Estatísticas do Sistema" width="600" style="max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 8px;">

*Tela de estatísticas e informações gerais, exibindo dados da eleição em andamento.*

### Mesa Receptora de Votos
<img src="docs/images/mesa.png" alt="Mesa Receptora de Votos" width="600" style="max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 8px;">

*Interface do mesário para controlar o fluxo de votação e liberar a urna para o próximo eleitor.*

### Terminal de Votação
<img src="docs/images/urna.png" alt="Terminal de Votação - Urna Eletrônica" width="600" style="max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 8px;">

*Interface da urna eletrônica com teclado numérico, botões de ação e exibição do candidato com foto.*

### Painel Administrativo
<img src="docs/images/admin.png" alt="Painel Administrativo" width="600" style="max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 8px;">

*Interface para gerenciar eleições, candidatos e cargos eleitorais.*

### Preview de Fotos de Candidatos
<img src="docs/images/photo-preview.png" alt="Preview de Foto de Candidato" width="400" style="max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 8px;">

*Funcionalidade de preview em tempo real durante o cadastro de candidatos.*

## Diagrama de Arquitetura

<img src="docs/images/architecture-diagram.png" alt="Diagrama de Arquitetura do Sistema" width="600" style="max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 8px;">

*Visão geral da arquitetura de janelas separadas e como elas se relacionam.*

## Funcionalidades Principais

*   **Painel de Controle Central:**
    *   Interface principal para gerenciar todas as janelas do sistema.
    *   Controle centralizado de Dashboard, Mesa, Urna e Painel Administrativo.
    *   Status em tempo real de todas as janelas abertas.

*   **Painel Administrativo:**
    *   Criação, edição e exclusão de eleições.
    *   Gerenciamento completo de candidatos por eleição (adicionar, editar, remover).
    *   **Upload de fotos dos candidatos** via URL.
    *   Ativação e desativação de eleições.
    *   Gerenciamento de cargos eleitorais.

*   **Mesa Receptora de Votos:**
    *   Tela dedicada ao "mesário" para liberar a urna para o próximo eleitor.
    *   A urna de votação permanece bloqueada até ser liberada, garantindo o fluxo de um eleitor por vez.

*   **Terminal de Votação:**
    *   Interface visualmente fiel à urna eletrônica brasileira.
    *   Teclado numérico e botões de ação (BRANCO, CORRIGE, CONFIRMA).
    *   **Exibição de foto, nome e partido do candidato** após a digitação do número.
    *   **Preview em tempo real** das fotos dos candidatos durante o cadastro.
    *   Lógica para votos nulos e em branco.
    *   Tela de "FIM" ao concluir o voto, seguida do bloqueio automático da urna.
    *   Botão de reiniciar votação para nova sessão.

*   **Resultados:**
    *   Apuração dos votos em tempo real, exibindo os resultados para cada eleição.

*   **Auditoria:**
    *   Registro detalhado de todas as ações críticas realizadas no sistema (criação de eleição, registro de voto, etc.), garantindo a rastreabilidade.

## Funcionalidades Implementadas Recentemente

*   **Sistema de Fotos de Candidatos:**
    *   Upload de fotos via URL no cadastro de candidatos.
    *   Preview em tempo real durante o cadastro.
    *   Exibição das fotos na urna eletrônica durante a votação.
    *   Interface elegante quando não há foto disponível.

*   **Arquitetura de Janelas Separadas:**
    *   Migração de sistema monolítico para janelas independentes.
    *   Painel de Controle Central para gerenciar todas as janelas.
    *   Melhor organização e experiência do usuário.
    *   Remoção de funcionalidades obsoletas (botões de anexar/desanexar).

*   **Melhorias na Interface:**
    *   Design mais moderno e intuitivo.
    *   Navegação simplificada entre funcionalidades.
    *   Status em tempo real das janelas abertas.
    *   Botões de ação mais apropriados para cada contexto.

## Tecnologias Utilizadas

*   **Framework:** .NET 9
*   **Interface Gráfica:** Avalonia UI
*   **Padrão de Arquitetura:** Model-View-ViewModel (MVVM) com o CommunityToolkit.Mvvm
*   **Banco de Dados:** SQLite
*   **ORM:** Entity Framework Core 9
*   **Injeção de Dependência:** Microsoft.Extensions.DependencyInjection

## Como Executar o Projeto

1.  **Pré-requisitos:**
    *   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.

2.  **Clonando o repositório:**
    ```bash
    git clone <url-do-repositorio>
    cd UrnaEletronicaFake
    ```

3.  **Executando a aplicação:**
    Navegue até a pasta do projeto principal e execute o comando:
    ```bash
    dotnet run --project UrnaEletronicaFake/UrnaEletronicaFake.csproj
    ```
    O banco de dados `urna_eletronica.db` será criado automaticamente no primeiro uso, com uma eleição e candidatos de exemplo.

## Estrutura do Projeto

*   `UrnaEletronicaFake/`: Contém o projeto principal da aplicação Avalonia.
    *   `Data/`: Configuração do `DbContext` do Entity Framework.
    *   `Models/`: Classes de domínio (Eleicao, Candidato, Voto, etc.).
    *   `Services/`: Lógica de negócio e comunicação com o banco de dados.
    *   `ViewModels/`: Contém os ViewModels para cada tela, seguindo o padrão MVVM.
    *   `Views/`: Arquivos `.axaml` que definem a interface do usuário.
*   `UrnaEletronicaFake.sln`: Arquivo de solução para abrir no Visual Studio ou JetBrains Rider.

## Como Atualizar as Imagens

As imagens do README estão localizadas na pasta `docs/images/` e atualmente são arquivos placeholder. Para substituí-las pelas imagens reais:

### Estrutura de Arquivos:
```
docs/images/
├── main-window-placeholder.txt          → main-window.png
├── dashboard-placeholder.txt            → dashboard.png
├── mesa-placeholder.txt                 → mesa.png
├── urna-placeholder.txt                 → urna.png
├── admin-placeholder.txt                → admin.png
├── photo-preview-placeholder.txt        → photo-preview.png
└── architecture-diagram-placeholder.txt → architecture-diagram.png
```

### Instruções:
1. **Substitua os arquivos `.txt`** pelos arquivos PNG correspondentes
2. **Mantenha os mesmos nomes** (sem o sufixo `-placeholder`)
3. **Use formato PNG** para todas as imagens
4. **Dimensões recomendadas** estão especificadas em cada arquivo placeholder
5. **As imagens serão automaticamente redimensionadas** pelo CSS para manter a organização do README

### Controles de Tamanho:
- **Largura máxima**: 600px para telas principais, 400px para detalhes
- **Responsividade**: `max-width: 100%` garante que as imagens se adaptem a diferentes telas
- **Estilo**: Bordas arredondadas e sombra sutil para melhor apresentação
