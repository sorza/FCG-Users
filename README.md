# 👥 FCG-Users

Microsserviço de Autenticação — Gerenciamento de identidade, JWT e Event Sourcing.

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-green)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![Event Sourcing](https://img.shields.io/badge/Pattern-Event%20Sourcing-red)](https://martinfowler.com/eaaDev/EventSourcing.html)
[![JWT](https://img.shields.io/badge/Auth-JWT-orange)](https://jwt.io/)

## 📝 Descrição

**FCG-Users** gerencia identidade e autenticação:

- ✅ **Registro e login**: Autenticação estateless via JWT
- ✅ **Senhas seguras**: PBKDF2 com salt
- ✅ **Event Sourcing**: Histórico imutável de usuários
- ✅ **Roles**: Admin e Common (autorização)
- ✅ **Pub/Sub**: Publica UserCreated → Functions envia email

---

## 🚀 Pré-requisitos

- .NET 8 SDK
- SQL Server
- MongoDB (Event Store)
- Azure Service Bus
- Docker (opcional)

---

## ⚙️ Configuração Local

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UsersDb;Trusted_Connection=True;"
  },
  "ServiceBus": {
    "ConnectionString": "<connection-string>",
    "Topics": { "Users": "users-topic" }
  },
  "MongoSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "EventStoreDb"
  },
  "Jwt": {
    "Key": "9y4XJg0aTphzFJw3TvksRvqHXd+Q4VB8f7ZvU08N+9Q=",
    "Issuer": "FGC-Users",
    "ExpiresMinutes": 120
  }
}
```

---

## 🚀 Como Executar

### Migrations
```bash
cd FCG-Users.Api
dotnet ef database update
```

### API
```bash
cd FCG-Users.Api
dotnet run
# https://localhost:7001/swagger
```

### Consumer
```bash
cd FCG-Users.Consumer
dotnet run
```

---

## 📊 Endpoints

| Método | Endpoint  | Autenticação | Descrição |
|--------|-----------|--------------|-----------|
| POST   | `/api`    | Anônimo      | Registrar usuário |
| POST   | `/api/auth`| Anônimo      | Login (get JWT) |
| GET    | `/api`    | Admin        | Listar usuários |
| GET    | `/api/{id}`| Admin        | Obter usuário |
| DELETE | `/api/{id}`| Admin        | Deletar usuário |

---

## 🧪 Testar

```bash
# Registrar
curl -X POST https://localhost:7001/api \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@fcg.com",
    "password": "Senha@123"
  }'

# Login
curl -X POST https://localhost:7001/api/auth \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@fcg.com",
    "password": "Senha@123"
  }'

# Listar usuários (Admin only)
curl -X GET https://localhost:7001/api \
  -H "Authorization: Bearer <token>"
```

---

## 🐳 Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FCG-Users.Api/", "."]
RUN dotnet restore && dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "FCG-Users.Api.dll"]
```

---

## ☸️ Kubernetes

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: fcg-users
spec:
  replicas: 2
  selector:
    matchLabels:
      app: fcg-users
  template:
    metadata:
      labels:
        app: fcg-users
    spec:
      containers:
      - name: fcg-users
        image: fcg-users:latest
        ports:
        - containerPort: 8080
        env:
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: db-secret
              key: connection-string
```

**HPA**:
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: fcg-users-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: fcg-users
  minReplicas: 2
  maxReplicas: 8
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

---

## 📚 Referências

- [JWT](https://jwt.io/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html)

---

**FIAP Tech Challenge — Fase 4**
