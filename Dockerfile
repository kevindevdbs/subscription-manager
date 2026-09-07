FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Restore em camada separada: só invalida quando muda dependência.
COPY src/SubscriptionManager.Domain/*.csproj src/SubscriptionManager.Domain/
COPY src/SubscriptionManager.Application/*.csproj src/SubscriptionManager.Application/
COPY src/SubscriptionManager.Infrastructure/*.csproj src/SubscriptionManager.Infrastructure/
COPY src/SubscriptionManager.Api/*.csproj src/SubscriptionManager.Api/
RUN dotnet restore src/SubscriptionManager.Api/SubscriptionManager.Api.csproj

COPY src/ src/
RUN dotnet publish src/SubscriptionManager.Api/SubscriptionManager.Api.csproj \
    -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# A imagem roda como root por padrão.
USER $APP_UID

COPY --from=build /app ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "SubscriptionManager.Api.dll"]
