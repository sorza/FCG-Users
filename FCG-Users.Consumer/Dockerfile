FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FCG-Users.Consumer/FCG-Users.Consumer.csproj", "FCG-Users.Consumer/"]
COPY ["FCG-Users.Application/FCG-Users.Application.csproj", "FCG-Users.Application/"]
COPY ["FCG-Users.Infrastructure/FCG-Users.Infrastructure.csproj", "FCG-Users.Infrastructure/"]
COPY ["FCG-Users.Domain/FCG-Users.Domain.csproj", "FCG-Users.Domain/"]

RUN dotnet restore "FCG-Users.Consumer/FCG-Users.Consumer.csproj"

COPY . .
WORKDIR /src/FCG-Users.Consumer
RUN dotnet publish "FCG-Users.Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FCG-Users.Consumer.dll"]
