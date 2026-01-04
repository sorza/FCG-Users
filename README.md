# FCG-Users

Projeto: FCG-Users — serviço de gerenciamento de contas de usuário do conjunto de microserviços FCG.

## Visão Geral

Este repositório implementa um serviço de usuários composto por uma API e um consumer (Worker Service) que consome eventos via Azure Service Bus. O sistema provê autenticação (JWT), criação/recuperação/remoção de contas e sincronização de usuários entre serviços, com senhas armazenadas de forma segura (PBKDF2).

## Arquitetura

Visão geral:
- Camadas bem definidas: Domain, Application, Infrastructure, Api/Consumer.
- Estilo arquitetural: Onion / Hexagonal — a camada de domínio é isolada de infraestruturas externas.
- Integração inter-serviços: orientada a eventos (pub/sub) via Azure Service Bus (Topics / Subscriptions).
- Consumer implementado como Worker Service (BackgroundService) para processamento contínuo.

Fluxos principais:
- Criação via API: validações → `Password.Create` (hash PBKDF2) → persistência → publicação de `UserCreated`.
- Sincronização Consumer: consome eventos → valida/instancia entidades (aceita hash via fábrica `Password.FromHash`) → persiste/remover localmente.
- Autenticação: credenciais validadas com `Password.Verify`, token JWT emitido.

## Tecnologias

- Linguagem: C# 
- Web/API: ASP.NET Core
- Worker: .NET Worker Service (BackgroundService)
- Mensageria: Azure.Messaging.ServiceBus
- Serialização: System.Text.Json
- Validação: FluentValidation
- Autenticação: JWT (ASP.NET Core)
- Criptografia: PBKDF2 (salt + hash)
- Logging: `ILogger<T>` (ASP.NET Core)

## Padrões de Projeto e Boas Práticas

- Domain-Driven Design (entidades, value objects, regras de negócio).
- Repository Pattern para abstração de persistência.
- Factory Methods para criação controlada de Value Objects (`Password.Create`, `Password.FromHash`).
- Dependency Injection nativa do ASP.NET Core.
- Event-driven architecture para desacoplamento entre microserviços.
- BackgroundService para consumers de longa execução.
- Tratamento seguro de senhas: hashing irreversível, verificação por comparação segura.
- Separação de responsabilidades e testes unitários/integração.

## Projetos (estrutura)

- `FCG-Users.Api` — API HTTP e configuração de autenticação/middlewares.
- `FCG-Users.Consumer` — Worker Service para consumir eventos do Service Bus.
- `FCG-Users.Domain` — Modelos do domínio (Entities, Value Objects, Exceptions).
- `FCG-Users.Application` — Casos de uso, serviços de aplicação, DTOs e validações.
- `FCG-Users.Infrastructure` — Implementações de repositórios, integrações e configurações de persistência.

## Requisitos

- .NET SDK 8.0 
- Azure Service Bus com Topic/Subscription configurados
- Banco de dados suportado pela implementação em `Infrastructure`
- Variáveis de ambiente ou __User Secrets__ para segredos

## Configuração

Chaves principais (appsettings.json / variáveis de ambiente):
- `ConnectionStrings:DefaultConnection`
- `ServiceBus:ConnectionString`
- `ServiceBus:Topics:Users`
- `ServiceBus:Subscriptions:Users`
- JWT: `Jwt:Key`, `Jwt:Issuer`, `Jwt:ExpiresInMinutes`

Exemplo mínimo (`appsettings.Development.json`):
