# Urna Eletrônica Fake (Multiplataforma)

Um projeto de simulador de urna eletrônica multiplataforma, desenvolvido com Avalonia UI, .NET 8 e SQLite. O sistema emula o processo de votação brasileiro, com interfaces distintas para administração, mesário e eleitor.

## Visão Geral

Este projeto foi modernizado de uma versão antiga em WPF para uma solução moderna e multiplataforma usando Avalonia UI. O objetivo é simular as principais interações de um sistema de votação eletrônica, desde o cadastro de eleições e candidatos até a apuração e auditoria dos votos.

## Funcionalidades Principais

*   **Painel Administrativo:**
    *   Criação, edição e exclusão de eleições.
    *   Gerenciamento completo de candidatos por eleição (adicionar, editar, remover).
    *   Ativação e desativação de eleições.

*   **Mesa Receptora de Votos:**
    *   Tela dedicada ao "mesário" para liberar a urna para o próximo eleitor.
    *   A urna de votação permanece bloqueada até ser liberada, garantindo o fluxo de um eleitor por vez.

*   **Terminal de Votação:**
    *   Interface visualmente fiel à urna eletrônica brasileira.
    *   Teclado numérico e botões de ação (BRANCO, CORRIGE, CONFIRMA).
    *   Exibição de foto, nome e partido do candidato após a digitação do número.
    *   Lógica para votos nulos e em branco.
    *   Tela de "FIM" ao concluir o voto, seguida do bloqueio automático da urna.

*   **Resultados:**
    *   Apuração dos votos em tempo real, exibindo os resultados para cada eleição.

*   **Auditoria:**
    *   Registro detalhado de todas as ações críticas realizadas no sistema (criação de eleição, registro de voto, etc.), garantindo a rastreabilidade.

## Tecnologias Utilizadas

*   **Framework:** .NET 8
*   **Interface Gráfica:** Avalonia UI
*   **Padrão de Arquitetura:** Model-View-ViewModel (MVVM) com o CommunityToolkit.Mvvm
*   **Banco de Dados:** SQLite
*   **ORM:** Entity Framework Core 8
*   **Injeção de Dependência:** Microsoft.Extensions.DependencyInjection

## Como Executar o Projeto

1.  **Pré-requisitos:**
    *   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.

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
