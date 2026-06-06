FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/BridgePay.API/BridgePay.API.csproj", "src/BridgePay.API/"]
COPY ["src/BridgePay.Application/BridgePay.Application.csproj", "src/BridgePay.Application/"]
COPY ["src/BridgePay.Domain/BridgePay.Domain.csproj", "src/BridgePay.Domain/"]
COPY ["src/BridgePay.Infrastructure/BridgePay.Infrastructure.csproj", "src/BridgePay.Infrastructure/"]
RUN dotnet restore "src/BridgePay.API/BridgePay.API.csproj"

COPY . .
WORKDIR "/src/src/BridgePay.API"
RUN dotnet build "BridgePay.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BridgePay.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BridgePay.API.dll"]
