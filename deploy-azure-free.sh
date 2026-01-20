#!/bin/bash

# Azure Free Tier Deployment Script for InnomateApp
# This script uses Azure free tier services to deploy your application

set -e

# Configuration
RESOURCE_GROUP="innomate-rg-free"
LOCATION="eastus"
APP_SERVICE_PLAN="innomate-asp-free"
WEB_APP_NAME="innomate-app-$(date +%s)"
SQL_SERVER_NAME="innomate-sql-$(date +%s)"
SQL_DATABASE_NAME="InnomateCore"
CONTAINER_REGISTRY="innomatecr$(date +%s)"

echo "🚀 Starting Azure Free Tier Deployment for InnomateApp..."

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI is not installed. Please install it first."
    exit 1
fi

# Login to Azure
echo "🔐 Logging into Azure..."
az login

# Set subscription
echo "📋 Setting subscription..."
az account set --subscription "Free Trial"

# Create Resource Group
echo "📦 Creating resource group..."
az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION

# Create Azure Container Registry (Free tier)
echo "📦 Creating container registry..."
az acr create \
    --resource-group $RESOURCE_GROUP \
    --name $CONTAINER_REGISTRY \
    --sku Basic \
    --admin-enabled true

# Get ACR credentials
ACR_USERNAME=$(az acr credential show --name $CONTAINER_REGISTRY --query username -o tsv)
ACR_PASSWORD=$(az acr credential show --name $CONTAINER_REGISTRY --query passwords[0].value -o tsv)
ACR_LOGIN_SERVER=$(az acr show --name $CONTAINER_REGISTRY --query loginServer -o tsv)

echo "🔐 ACR Login Server: $ACR_LOGIN_SERVER"

# Create SQL Server and Database (Free tier)
echo "🗄️ Creating SQL Server and Database..."
az sql server create \
    --name $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user "innomateadmin" \
    --admin-password "Innomate@123!" \
    --version "12.0"

az sql db create \
    --name $SQL_DATABASE_NAME \
    --server $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --edition "GeneralPurpose" \
    --family "Gen5" \
    --capacity "2" \
    --max-size "5120MB"

# Configure firewall rule for Azure services
az sql server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name "AllowAzureIPs" \
    --start-ip-address "0.0.0.0" \
    --end-ip-address "0.0.0.0"

# Create App Service Plan (Free tier)
echo "🌐 Creating App Service Plan..."
az appservice plan create \
    --name $APP_SERVICE_PLAN \
    --resource-group $RESOURCE_GROUP \
    --sku "F1" \
    --is-linux

# Create Web App for Containers
echo "🚀 Creating Web App..."
az webapp create \
    --resource-group $RESOURCE_GROUP \
    --plan $APP_SERVICE_PLAN \
    --name $WEB_APP_NAME \
    --deployment-container-image-name "nginx:latest"

# Configure Web App
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP \
    --name $WEB_APP_NAME \
    --settings \
    WEBSITES_PORT=80 \
    DOCKER_CUSTOM_IMAGE_NAME="$ACR_LOGIN_SERVER/innomate-backend:latest" \
    DOCKER_REGISTRY_SERVER_URL="https://$ACR_LOGIN_SERVER" \
    DOCKER_REGISTRY_SERVER_USERNAME="$ACR_USERNAME" \
    DOCKER_REGISTRY_SERVER_PASSWORD="$ACR_PASSWORD"

# Get connection string
SQL_CONNECTION_STRING=$(az sql db show-connection-string \
    --name $SQL_DATABASE_NAME \
    --server $SQL_SERVER_NAME \
    --client ado.net \
    --query "connectionString" -o tsv)

# Replace password in connection string
SQL_CONNECTION_STRING=${SQL_CONNECTION_STRING/<password>/Innomate@123!}

# Set environment variables
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP \
    --name $WEB_APP_NAME \
    --settings \
    "ConnectionStrings__DefaultConnection=$SQL_CONNECTION_STRING" \
    "Jwt__Key=super_secret_jwt_signing_key_for_production_123!" \
    "Jwt__Issuer=InnomateApp" \
    "Jwt__Audience=InnomateAppUsers" \
    "ASPNETCORE_ENVIRONMENT=Production"

# Enable continuous deployment
az webapp deployment container config \
    --resource-group $RESOURCE_GROUP \
    --name $WEB_APP_NAME \
    --enable-cd true

echo "✅ Deployment completed successfully!"
echo ""
echo "📋 Deployment Summary:"
echo "Resource Group: $RESOURCE_GROUP"
echo "Web App URL: https://$WEB_APP_NAME.azurewebsites.net"
echo "SQL Server: $SQL_SERVER_NAME.database.windows.net"
echo "Container Registry: $ACR_LOGIN_SERVER"
echo ""
echo "🔧 Next Steps:"
echo "1. Build and push your Docker images to the container registry:"
echo "   docker build -t $ACR_LOGIN_SERVER/innomate-backend:latest ./backend"
echo "   docker login $ACR_LOGIN_SERVER -u $ACR_USERNAME -p $ACR_PASSWORD"
echo "   docker push $ACR_LOGIN_SERVER/innomate-backend:latest"
echo ""
echo "2. Configure your GitHub repository secrets:"
echo "   - AZURE_CREDENTIALS: Your Azure service principal credentials"
echo "   - AZURE_DB_CONNECTION_STRING: $SQL_CONNECTION_STRING"
echo "   - JWT_SECRET_KEY: Your JWT secret key"
echo ""
echo "3. Update your GitHub Actions workflow with the correct container registry URL"
