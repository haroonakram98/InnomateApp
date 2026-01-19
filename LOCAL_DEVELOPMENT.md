# Local Development Guide

This guide covers how to run your InnomateApp locally for development and testing.

---

## 🎯 Quick Start (Recommended - Docker)

The **easiest way** to run everything locally is using Docker Compose. Everything is already configured!

### Prerequisites
- Docker Desktop installed and running
- Git (to clone/pull the repo)

### Steps

1. **Ensure Docker is Running**
   ```bash
   # Check Docker is running
   docker --version
   docker-compose --version
   ```

2. **Start All Services**
   ```bash
   # From the project root directory
   docker-compose up -d
   ```

3. **Wait for Services to Start** (~30-60 seconds)
   ```bash
   # Check logs to see when everything is ready
   docker-compose logs -f
   
   # Look for these messages:
   # ✅ Database initialization successfully completed!
   # ✅ InnomateApp API started successfully
   ```

4. **Access Your Application**
   - **Frontend**: http://localhost:5173
   - **Backend API**: http://localhost:8080/api
   - **Swagger**: http://localhost:8080/swagger
   - **Database**: localhost:1433 (SQL Server)

5. **Stop Services**
   ```bash
   docker-compose down
   ```

---

## 📋 What's Already Configured

Your project has these files ready for local development:

### `.env` (Root Directory)
```env
# Database
DB_PASSWORD=Innomate@123
DB_CONTAINER_NAME=innomate-db

# API
ASPNETCORE_ENVIRONMENT=Development
API_PORT=8080
API_CONTAINER_NAME=innomate-api

# Frontend
VITE_API_URL=http://localhost:8080/api
UI_PORT=5173
UI_CONTAINER_NAME=innomate-ui
```

### `frontend/.env.development`
```env
VITE_API_URL=http://localhost:8080/api
```

### `docker-compose.yml`
Already configured to:
- Run SQL Server 2017
- Build and run the .NET API
- Build and run the React frontend
- Connect all services together

---

## 🔧 Option 2: Run Without Docker (Native)

If you prefer to run services individually without Docker:

### Prerequisites
- .NET 8 SDK
- Node.js 20+
- SQL Server (local instance or Azure)

### Step 1: Database Setup

**Option A: Use Azure SQL (Recommended)**
```bash
# No local setup needed - use your Azure database
# Update appsettings.Development.json with Azure connection string
```

**Option B: Local SQL Server**
```bash
# Install SQL Server Express (free)
# Or use SQL Server Developer Edition (free)
# Connection string: Server=localhost;Database=SmartOps;Trusted_Connection=True;
```

### Step 2: Run Backend API

```bash
# Navigate to API project
cd backend/InnomateApp.API

# Restore dependencies
dotnet restore

# Run migrations (first time only)
dotnet ef database update --project ../InnomateApp.Infrastructure

# Run the API
dotnet run
```

**Backend will be available at:**
- HTTP: http://localhost:5000
- HTTPS: https://localhost:7219
- Swagger: https://localhost:7219/swagger

### Step 3: Update Frontend Configuration

Create or update `frontend/.env.local`:
```env
# If using HTTPS (default .NET dev server)
VITE_API_URL=https://localhost:7219/api

# OR if using HTTP
VITE_API_URL=http://localhost:5000/api
```

### Step 4: Run Frontend

```bash
# Navigate to frontend
cd frontend

# Install dependencies (first time only)
npm install

# Run dev server
npm run dev
```

**Frontend will be available at:**
- http://localhost:5173

---

## 🗄️ Database Management

### View Database (SQL Server Management Studio)

**Connection Details:**
- **Server**: `localhost,1433` (Docker) or `localhost` (native)
- **Authentication**: SQL Server Authentication
- **Username**: `sa`
- **Password**: `Innomate@123` (from `.env`)
- **Database**: `SmartOps`

### Reset Database (Docker)

```bash
# Stop all containers
docker-compose down

# Remove the database volume (⚠️ This deletes all data!)
docker volume rm innomateapp_sql-data-2017

# Start fresh
docker-compose up -d
```

### Migrations

```bash
# Add a new migration
cd backend/InnomateApp.Infrastructure
dotnet ef migrations add YourMigrationName --startup-project ../InnomateApp.API

# Apply migrations
dotnet ef database update --startup-project ../InnomateApp.API
```

---

## 🐛 Troubleshooting

### Issue: "Port already in use"

**Error**: `Bind for 0.0.0.0:8080 failed: port is already allocated`

**Solution**:
```bash
# Find what's using the port
netstat -ano | findstr :8080

# Kill the process (replace PID with actual process ID)
taskkill /PID <PID> /F

# Or change the port in .env
API_PORT=8081
```

### Issue: SQL Server container exits immediately

**Solution**:
```bash
# Check logs
docker logs innomate-db

# Common fix: Remove old volume and restart
docker-compose down
docker volume rm innomateapp_sql-data-2017
docker-compose up -d
```

### Issue: Frontend can't connect to API

**Check**:
1. API is running: http://localhost:8080/swagger
2. `.env.development` has correct URL: `http://localhost:8080/api`
3. CORS is configured in `Program.cs` (already done)

**Solution**:
```bash
# Restart frontend
cd frontend
npm run dev
```

### Issue: Database migrations not applied

**Solution**:
```bash
# The API automatically runs migrations on startup
# But you can manually run them:
cd backend/InnomateApp.API
dotnet ef database update --project ../InnomateApp.Infrastructure
```

### Issue: "Login failed for user 'sa'"

**Solution**:
```bash
# Wait longer - SQL Server takes 20-30 seconds to fully start
docker logs -f innomate-db

# Look for: "SQL Server is now ready for client connections"
```

---

## 🔄 Common Development Workflows

### Making Backend Changes

```bash
# 1. Make your code changes
# 2. Rebuild the API container
docker-compose up -d --build api

# OR run without Docker
cd backend/InnomateApp.API
dotnet run
```

### Making Frontend Changes

```bash
# 1. Make your code changes
# 2. Rebuild the frontend container
docker-compose up -d --build frontend

# OR run without Docker (hot reload enabled)
cd frontend
npm run dev
# Changes auto-reload!
```

### Adding a New NuGet Package

```bash
cd backend/InnomateApp.API
dotnet add package PackageName

# Rebuild container
docker-compose up -d --build api
```

### Adding a New npm Package

```bash
cd frontend
npm install package-name

# Rebuild container
docker-compose up -d --build frontend
```

---

## 📊 Monitoring & Logs

### View All Logs
```bash
docker-compose logs -f
```

### View Specific Service Logs
```bash
# Database
docker logs -f innomate-db

# API
docker logs -f innomate-api

# Frontend
docker logs -f innomate-ui
```

### Check Service Status
```bash
docker-compose ps
```

---

## 🎨 Development vs Production

| Aspect | Local Development | Production (Azure) |
|--------|------------------|-------------------|
| **Frontend URL** | http://localhost:5173 | https://innomate-app.vercel.app |
| **API URL** | http://localhost:8080/api | https://innomateapi-....azurewebsites.net/api |
| **Database** | Docker SQL Server 2017 | Azure SQL Database |
| **Environment** | `Development` or `Docker` | `Production` |
| **HTTPS** | Optional | Required |
| **Hot Reload** | ✅ Enabled | ❌ Build required |

---

## 🚀 Quick Commands Reference

```bash
# Start everything
docker-compose up -d

# Stop everything
docker-compose down

# Rebuild and start
docker-compose up -d --build

# View logs
docker-compose logs -f

# Restart a specific service
docker-compose restart api

# Remove everything (including volumes)
docker-compose down -v

# Check what's running
docker-compose ps

# Execute command in container
docker exec -it innomate-api bash
```

---

## 🔐 Default Credentials

### Database (SQL Server)
- **Username**: `sa`
- **Password**: `Innomate@123`
- **Database**: `SmartOps`

### Application (Seeded Data)
Check your `SeedData.cs` for default users. Typically:
- **Admin**: Check the seeding logic in `InnomateApp.Infrastructure/Persistence/SeedData.cs`

---

## 📝 Environment Variables Summary

### Root `.env` (for Docker Compose)
```env
DB_PASSWORD=Innomate@123
DB_CONTAINER_NAME=innomate-db
ASPNETCORE_ENVIRONMENT=Development
API_PORT=8080
API_CONTAINER_NAME=innomate-api
VITE_API_URL=http://localhost:8080/api
UI_PORT=5173
UI_CONTAINER_NAME=innomate-ui
```

### `frontend/.env.development` (for npm run dev)
```env
VITE_API_URL=http://localhost:8080/api
```

### `backend/InnomateApp.API/appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartOps;User Id=sa;Password=Innomate@123;TrustServerCertificate=True;"
  }
}
```

---

## ✅ Verification Checklist

After starting services, verify:

- [ ] Database is running: `docker ps` shows `innomate-db`
- [ ] API is running: http://localhost:8080/swagger shows Swagger UI
- [ ] Frontend is running: http://localhost:5173 shows your app
- [ ] Database has tables: Connect via SSMS and check `SmartOps` database
- [ ] API can connect to DB: Check API logs for "Database initialization successfully completed!"
- [ ] Frontend can call API: Open browser DevTools → Network tab, try login

---

## 🎯 Next Steps

1. **Start Development**: `docker-compose up -d`
2. **Make Changes**: Edit code in your IDE
3. **Test Locally**: http://localhost:5173
4. **Commit & Push**: Changes auto-deploy to production via GitHub Actions

**Happy Coding!** 🚀
