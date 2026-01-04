# FCG-Users

Projeto: FCG-Users — serviço de gerenciamento de contas de usuário do conjunto de microserviços FCG.

## Visão Geral

Este repositório contém a implementação de um serviço de usuários com API e um consumer que consome eventos de usuários via Azure Service Bus. Inclui domínio, aplicação, infraestrutura e um worker para sincronização de usuários entre serviços.

Principais responsabilidades:
- Autenticação (JWT)
- Criação, recuperação e remoção de contas
- Sincronização via eventos (UserCreated, UserDeleted)
- Armazenamento seguro de senhas usando PBKDF2 (salt + hash)

## Arquitetura

O repositório está organizado em projetos separados por responsabilidade:

- `FCG-Users.Api` — API HTTP (controllers, endpoints, configuração de autenticação)
- `FCG-Users.Consumer` — Worker Service que consome eventos do Azure Service Bus
- `FCG-Users.Domain` — Entidades, Value Objects e regras de negócio
- `FCG-Users.Application` — Serviços de aplicação, DTOs, validações
- `FCG-Users.Infrastructure` — Implementações de repositórios, integração com base de dados e filas

A comunicação entre serviços é feita via eventos no Azure Service Bus (topic/subscription).

## Projetos

Verifique a solução para os projetos existentes e suas dependências. Os alvos de framework incluem .NET 8 e .NET 10 conforme cada projeto.

## Requisitos

- .NET SDK 8.0 e 10.0
- Azure Service Bus (Topic/Subscription configurados)
- Banco de dados suportado pela implementação em `Infrastructure` (string de conexão configurável)

## Configuração

Defina as configurações em `appsettings.json` / `appsettings.Development.json` ou via variáveis de ambiente:

- `ConnectionStrings:DefaultConnection` — string de conexão do banco
- `ServiceBus:ConnectionString` — conexão do Azure Service Bus
- `ServiceBus:Topics:Users` — nome do topic de usuários
- `ServiceBus:Subscriptions:Users` — nome da subscription
- Configurações de JWT (segredo, emissor, validade)

Exemplo mínimo (appsettings.Development.json):
