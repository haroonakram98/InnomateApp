# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj files first (better caching)
COPY InnomateApp.API/*.csproj InnomateApp.API/
COPY InnomateApp.Application/*.csproj InnomateApp.Application/
COPY InnomateApp.Domain/*.csproj InnomateApp.Domain/
COPY InnomateApp.Infrastructure/*.csproj InnomateApp.Infrastructure/

RUN dotnet restore InnomateApp.API/InnomateApp.API.csproj

# copy everything
COPY . .
WORKDIR /src/InnomateApp.API

RUN dotnet publish -c Release -o /app/publish

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Docker
EXPOSE 8080

ENTRYPOINT ["dotnet", "InnomateApp.API.dll"]