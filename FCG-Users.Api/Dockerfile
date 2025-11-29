FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ../FCG-Users.Api/FCG-Users.Api.csproj ./FCG-Users.Api/
COPY ../FCG-Users.Domain/FCG-Users.Domain.csproj ./FCG-Users.Domain/
COPY ../FCG-Users.Application/FCG-Users.Application.csproj ./FCG-Users.Application/
COPY ../FCG-Users.Infrastructure/FCG-Users.Infrastructure.csproj ./FCG-Users.Infrastructure/

RUN dotnet restore ./FCG-Users.Api/FCG-Users.Api.csproj

COPY .. .

RUN dotnet publish ./FCG-Users.Api/FCG-Users.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FCG-Users.Api.dll"]
