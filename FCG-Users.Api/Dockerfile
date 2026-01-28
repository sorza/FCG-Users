ARG BUILD_VERSION=1.0.0
ARG BUILD_DATE
ARG VCS_REF

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["FCG-Users.Api/FCG-Users.Api.csproj", "FCG-Users.Api/"]
COPY ["FCG-Users.Application/FCG-Users.Application.csproj", "FCG-Users.Application/"]
COPY ["FCG-Users.Infrastructure/FCG-Users.Infrastructure.csproj", "FCG-Users.Infrastructure/"]
COPY ["FCG-Users.Domain/FCG-Users.Domain.csproj", "FCG-Users.Domain/"]

RUN dotnet restore "FCG-Users.Api/FCG-Users.Api.csproj"

COPY . .
WORKDIR /src/FCG-Users.Api

RUN dotnet publish "FCG-Users.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:InvariantGlobalization=true \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

LABEL maintainer="FCG Team" \
      version="${BUILD_VERSION}" \
      description="FCG Users API - Microservice for user management" \
      org.opencontainers.image.created="${BUILD_DATE}" \
      org.opencontainers.image.revision="${VCS_REF}" \
      org.opencontainers.image.source="https://github.com/alexandresorza/fcg-microservices"

WORKDIR /app

RUN addgroup -g 1000 appuser && \
    adduser -u 1000 -G appuser -s /bin/sh -D appuser && \
    chown -R appuser:appuser /app

COPY --from=build --chown=appuser:appuser /app/publish .

USER appuser

ENV ASPNETCORE_URLS=http://+:80 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "FCG-Users.Api.dll"]
