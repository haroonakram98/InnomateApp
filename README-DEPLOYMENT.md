# InnomateApp Deployment Guide

This guide covers Docker containerization, CI/CD pipeline setup, and Azure deployment using free tier services.

## Architecture Overview

### Backend (.NET 8.0)
- **Clean Architecture** with Domain, Application, Infrastructure, and API layers
- **CQRS Pattern** with MediatR for command/query separation
- **Entity Framework Core** with SQL Server
- **JWT Authentication** with role-based authorization
- **Serilog** for structured logging
- **Prometheus** metrics for monitoring

### Frontend (React + TypeScript)
- **Vite** for fast development and building
- **TailwindCSS** for styling
- **React Query** for server state management
- **Zustand** for client state management
- **React Router** for navigation

## Local Development with Docker

### Prerequisites
- Docker Desktop
- .NET 8.0 SDK
- Node.js 18+
- SQL Server (optional, Docker image provided)

### Quick Start

1. **Clone and build the application:**
   ```bash
   git clone <repository-url>
   cd InnomateApp
   
   # Build and run with Docker Compose
   docker-compose up --build
   ```

2. **Access the application:**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:8080
   - API Documentation: http://localhost:8080/swagger
   - Database: localhost:1433 (SQL Server)

3. **Database Connection:**
   - Server: `localhost,1433`
   - Database: `InnomateCore`
   - User: `sa`
   - Password: `Innomate@123`

## Docker Configuration

### Backend Dockerfile
- Multi-stage build for optimized image size
- Non-root user for security
- Production-ready configuration

### Frontend Dockerfile
- Nginx serving static files
- Gzip compression enabled
- Client-side routing support

### Docker Compose
- Complete application stack
- SQL Server container
- Networking and volume configuration
- Environment variables management

## CI/CD Pipeline with GitHub Actions

### Workflow Features
- **Automated Testing**: Backend unit tests and frontend linting
- **Docker Build**: Multi-architecture image builds
- **Container Registry**: GitHub Container Registry integration
- **Azure Deployment**: Automated deployment to Azure Container Instances

### Required GitHub Secrets
- `AZURE_CREDENTIALS`: Azure service principal JSON
- `AZURE_DB_CONNECTION_STRING`: Production database connection string
- `JWT_SECRET_KEY`: JWT signing key for production

### Pipeline Triggers
- Push to `main` branch: Full deployment
- Push to `develop` branch: Build and test only
- Pull requests: Build and test validation

## Azure Free Tier Deployment

### Services Used (All Free Tier)
1. **Azure App Service** (F1 tier)
   - 1 GB storage
   - 60 minutes compute time/day
   - Custom domain support

2. **Azure SQL Database** (Free tier)
   - 5 GB storage
   - 2 DTUs
   - Basic backup retention

3. **Azure Container Registry** (Basic tier)
   - 10 GB storage
   - 10,000 image pulls/day

### Deployment Options

#### Option 1: Automated Deployment (Recommended)
```bash
# Run the deployment script
chmod +x deploy-azure-free.sh
./deploy-azure-free.sh
```

#### Option 2: Manual Deployment
1. Create Azure resources using the portal
2. Build and push Docker images
3. Configure App Service settings
4. Deploy using Azure CLI

### Environment Configuration

#### Production Environment Variables
```bash
ConnectionStrings__DefaultConnection=Server=tcp:[server-name].database.windows.net,1433;Database=InnomateCore;User ID=innomateadmin;Password=Innomate@123!;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;
Jwt__Key=super_secret_jwt_signing_key_for_production_123!
Jwt__Issuer=InnomateApp
Jwt__Audience=InnomateAppUsers
ASPNETCORE_ENVIRONMENT=Production
```

## Monitoring and Logging

### Application Monitoring
- **Serilog**: Structured logging to console and files
- **Prometheus**: Metrics collection
- **Azure Monitor**: Integration with Azure services

### Log Levels
- Production: Warning and above
- Development: Information and above
- Debug: Verbose logging for troubleshooting

## Security Considerations

### Production Security
- Non-root Docker user
- HTTPS enforcement
- Environment variable secrets
- SQL injection prevention (EF Core)
- CORS configuration
- JWT token expiration

### Database Security
- Strong password policy
- Azure firewall rules
- Encrypted connections
- Regular backups

## Performance Optimization

### Backend Optimization
- Entity Framework query optimization
- Response caching
- Compression middleware
- Connection pooling

### Frontend Optimization
- Code splitting
- Image optimization
- Bundle minification
- CDN deployment

## Troubleshooting

### Common Issues

1. **Database Connection Errors**
   - Check connection string format
   - Verify firewall rules
   - Confirm SQL Server is running

2. **Docker Build Failures**
   - Clear Docker cache: `docker system prune`
   - Check .dockerignore file
   - Verify file permissions

3. **Azure Deployment Issues**
   - Validate resource limits
   - Check app service logs
   - Verify environment variables

### Debug Commands
```bash
# Check container logs
docker-compose logs backend
docker-compose logs frontend

# Test database connection
sqlcmd -S localhost,1433 -U sa -P "Innomate@123" -Q "SELECT @@VERSION"

# Azure app service logs
az webapp log tail --resource-group innomate-rg-free --name innomate-app-xxx
```

## Scaling Considerations

### When to Scale Up
- Consistent high CPU/memory usage
- Database performance bottlenecks
- Increased user traffic

### Scaling Options
- **Vertical Scaling**: Upgrade App Service tier
- **Horizontal Scaling**: Add instances
- **Database Scaling**: Upgrade to Standard/Premium tier
- **CDN Integration**: Azure CDN for static assets

## Backup and Disaster Recovery

### Database Backups
- Automated daily backups (Azure SQL)
- Point-in-time restoration
- Geo-redundant storage options

### Application Backup
- Container image versioning
- Configuration backups
- Git repository history

## Cost Management

### Free Tier Limits
- Monitor Azure usage regularly
- Set up budget alerts
- Review resource utilization

### Cost Optimization Tips
- Use appropriate service tiers
- Implement caching strategies
- Optimize database queries
- Monitor and right-size resources

## Next Steps

1. **Set up monitoring dashboards**
2. **Implement automated testing**
3. **Configure alerting rules**
4. **Set up backup policies**
5. **Plan for scaling scenarios**

## Support

For deployment issues:
1. Check Azure service status
2. Review application logs
3. Consult Azure documentation
4. Monitor GitHub Actions workflow runs
