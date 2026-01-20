# Use the official .NET 8.0 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["backend/InnomateApp.API/InnomateApp.API.csproj", "backend/InnomateApp.API/"]
COPY ["backend/InnomateApp.Application/InnomateApp.Application.csproj", "backend/InnomateApp.Application/"]
COPY ["backend/InnomateApp.Domain/InnomateApp.Domain.csproj", "backend/InnomateApp.Domain/"]
COPY ["backend/InnomateApp.Infrastructure/InnomateApp.Infrastructure.csproj", "backend/InnomateApp.Infrastructure/"]
RUN dotnet restore "backend/InnomateApp.API/InnomateApp.API.csproj"

# Copy the rest of the files
COPY . .

# Build the application
WORKDIR "/src/backend/InnomateApp.API"
RUN dotnet build "InnomateApp.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "InnomateApp.API.csproj" -c Release -o /app/publish

# Build the final image
FROM base AS final

# Install security updates and cleanup
RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/api/health || exit 1

ENTRYPOINT ["dotnet", "InnomateApp.API.dll"]
