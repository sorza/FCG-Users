# FCG-Users

O projeto **FCG-Users** faz parte de um ecossistema de microsserviços voltado para gerenciamento de usuários e suas bibliotecas de jogos.  
Ele foi desenvolvido com foco em **event sourcing**, **arquitetura orientada a eventos** e **boas práticas de microsserviços**.

---

## Tecnologias Utilizadas
- **.NET 8 / ASP.NET Core** → APIs modernas e performáticas.
- **Entity Framework Core** → persistência e abstração de acesso ao banco de dados SQL Server.
- **MongoDB** → armazenamento de eventos (Event Store).
- **Azure Service Bus** → mensageria assíncrona baseada em tópicos e subscriptions.
- **Docker** → containerização e execução isolada dos microsserviços.
- **Swagger / Swashbuckle** → documentação interativa da API.
- **FluentValidation** → validação de requests
- **JWT** → autenticação e autorização
- **PBKDF2** → Criptografia (salt + hash)

---

## Arquitetura
- **Microsserviços** → cada contexto (Users, Games, Libraries, Payments) é isolado e independente.
- **Event-Driven Architecture (EDA)** → comunicação entre serviços via eventos publicados em tópicos do Service Bus.
- **Event Sourcing** → todas as mudanças de estado dos usuários são registradas como eventos imutáveis.
- **CQRS (Command Query Responsibility Segregation)** → separação entre comandos (alteração de estado) e queries (leitura).
- **Camadas bem definidas**:
  - **API** → exposição dos endpoints REST.
  - **Application** → regras de negócio e orquestração.
  - **Infrastructure** → persistência, mensageria e integrações externas.
  - **Domain** → entidades e lógica de domínio.

---

## Padrões e Designs
- **Repository Pattern** → abstração do acesso a dados.
- **Dependency Injection (DI)** → desacoplamento e facilidade de testes.
- **Middleware personalizado** → tratamento global de exceções e correlação de requisições.
- **Event Publisher/Consumer** → produtores e consumidores de eventos no Azure Service Bus.
- **Idempotência** → prevenção de duplicidade no processamento de eventos.
- **Dead Letter Queue (DLQ)** → resiliência e análise de mensagens problemáticas.

---

## Fluxo de Eventos
1. **Criação/remoção de usuários** gera eventos (`UserCreated`, `UserRemoved`).  
2. Os eventos são persistidos no **MongoDB (Event Store)**.  
3. Os eventos são publicados no **Azure Service Bus (users-topic)**.  
4. Outros microsserviços (como **Libraries**) consomem esses eventos:
   - Se um **User** for removido → todas as bibliotecas vinculadas a ele são apagadas.

---

## Observabilidade
- **Logs estruturados** com `CorrelationId` para rastrear requisições e eventos.  
- **Swagger** para documentação e testes de endpoints.  
- **GlobalExceptionMiddleware** para captura e padronização de erros.

---

## Competências demonstradas
- Microsserviços  
- Event Sourcing  
- CQRS  
- Event-Driven Architecture (EDA)  
- Azure Service Bus  
- MongoDB (Event Store)  
- Entity Framework Core  
- .NET 8 / ASP.NET Core  
- Repository Pattern  
- Dependency Injection  
- Middleware personalizado  
- Idempotência  
- Docker  
- Swagger

---

## Objetivo
Este projeto foi desenvolvido como parte de um portfólio pessoal para demonstrar:
- Conhecimento em **arquitetura de microsserviços**.  
- Aplicação prática de **event sourcing** e **mensageria assíncrona**.  
- Uso de **padrões de projeto** e boas práticas de engenharia de software.  

