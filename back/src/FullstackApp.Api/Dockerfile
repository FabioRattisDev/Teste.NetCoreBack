# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY back/src/FullstackApp.Api/FullstackApp.Api.csproj back/src/FullstackApp.Api/
COPY back/src/FullstackApp.Application/FullstackApp.Application.csproj back/src/FullstackApp.Application/
COPY back/src/FullstackApp.Infrastructure/FullstackApp.Infrastructure.csproj back/src/FullstackApp.Infrastructure/
COPY back/src/FullstackApp.Domain/FullstackApp.Domain.csproj back/src/FullstackApp.Domain/

RUN dotnet restore "back/src/FullstackApp.Api/FullstackApp.Api.csproj"

# Copy everything and build
COPY back/ back/
WORKDIR /src/back/src/FullstackApp.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FullstackApp.Api.dll"]
