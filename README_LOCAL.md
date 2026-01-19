# Running Locally - Summary

## ✅ Good News: Everything is Already Configured!

**You don't need to make ANY changes** to run locally. Your project is set up to automatically use the correct configuration based on the environment.

---

## 🎯 To Run Locally (Easiest Method)

### Step 1: Start Docker Desktop
Make sure Docker Desktop is running on your Windows machine.

### Step 2: Run This Command
```bash
docker-compose up -d
```

### Step 3: Wait 30-60 Seconds
The services need time to start, especially the database.

### Step 4: Access Your App
- **Frontend**: http://localhost:5173
- **Backend**: http://localhost:8080/swagger
- **Database**: localhost:1433

---

## 🔧 How It Works Automatically

### Environment Detection

Your app automatically uses the right configuration:

**When Running Locally (Docker):**
- Frontend reads: `frontend/.env.development`
  - API URL: `http://localhost:8080/api`
- Backend uses: `appsettings.Docker.json`
  - Database: Docker SQL Server container
  - CORS: Allows `http://localhost:5173`

**When Running in Production:**
- Frontend reads: `frontend/.env.production`
  - API URL: `https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net/api`
- Backend uses: Azure environment variables
  - Database: Azure SQL Database
  - CORS: Allows your Vercel URL

### No Manual Switching Required!

The configuration files are already set up:

```
InnomateApp/
├── .env                              # Docker Compose config (local)
├── frontend/
│   ├── .env.development             # Local API URL ✅
│   └── .env.production              # Azure API URL ✅
└── backend/
    └── InnomateApp.API/
        ├── appsettings.json         # Base config
        ├── appsettings.Development.json  # Local SQL Server
        └── appsettings.Docker.json  # Docker SQL Server ✅
```

---

## 📊 Configuration Comparison

| Setting | Local (Docker) | Production (Azure) |
|---------|---------------|-------------------|
| **Frontend URL** | http://localhost:5173 | https://innomate-app.vercel.app |
| **API URL** | http://localhost:8080/api | https://innomateapi-....azurewebsites.net/api |
| **Database** | SQL Server 2017 (Docker) | Azure SQL Database |
| **CORS Origins** | http://localhost:5173 | https://innomate-app.vercel.app |
| **Environment** | `Docker` | `Production` |

---

## 🚀 Quick Commands

```bash
# Start all services (database, API, frontend)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Rebuild after code changes
docker-compose up -d --build

# Reset database (⚠️ deletes all data)
docker-compose down -v
docker-compose up -d
```

---

## 🔍 Verify Everything is Working

### 1. Check Services are Running
```bash
docker-compose ps
```

You should see:
- `innomate-db` (database)
- `innomate-api` (backend)
- `innomate-ui` (frontend)

### 2. Check API is Responding
Open: http://localhost:8080/swagger

You should see the Swagger UI with all your API endpoints.

### 3. Check Frontend is Running
Open: http://localhost:5173

You should see your login page.

### 4. Check Database Connection
Look at the API logs:
```bash
docker logs innomate-api
```

Look for: `✅ Database initialization successfully completed!`

---

## 🐛 Common Issues & Solutions

### Issue: "Port 8080 is already in use"

**Solution 1**: Stop whatever is using port 8080
```bash
# Find the process
netstat -ano | findstr :8080

# Kill it (replace PID with actual number)
taskkill /PID <PID> /F
```

**Solution 2**: Change the port
Edit `.env`:
```env
API_PORT=8081
```

Then update `frontend/.env.development`:
```env
VITE_API_URL=http://localhost:8081/api
```

### Issue: "SQL Server container keeps restarting"

**Solution**: Reset the database volume
```bash
docker-compose down
docker volume rm innomateapp_sql-data-2017
docker-compose up -d
```

### Issue: "Frontend shows 'Network Error'"

**Check**:
1. API is running: http://localhost:8080/swagger
2. Frontend is using correct URL in `.env.development`
3. CORS is configured (already done in `Program.cs`)

**Solution**: Restart frontend
```bash
docker-compose restart frontend
```

---

## 💡 Development Tips

### 1. Making Backend Changes
```bash
# Edit your code
# Then rebuild the API container
docker-compose up -d --build api
```

### 2. Making Frontend Changes
**Option A**: Use Docker (slower)
```bash
docker-compose up -d --build frontend
```

**Option B**: Run frontend natively (faster, hot reload)
```bash
cd frontend
npm install  # First time only
npm run dev  # Changes auto-reload!
```

### 3. Database Management
**Connect with SQL Server Management Studio (SSMS):**
- Server: `localhost,1433`
- Authentication: SQL Server Authentication
- Username: `sa`
- Password: `Innomate@123`
- Database: `SmartOps`

---

## 📚 Documentation Files

- **`QUICKSTART.md`** - Quick reference (this file)
- **`LOCAL_DEVELOPMENT.md`** - Detailed guide with troubleshooting
- **`DEPLOYMENT_GUIDE.md`** - Docker architecture details
- **`FRONTEND_DEPLOYMENT.md`** - Vercel deployment guide
- **`AZURE_CONFIG.md`** - Azure configuration checklist

---

## ✅ Summary

**To run locally, you just need:**

1. Docker Desktop running
2. Run: `docker-compose up -d`
3. Wait 30-60 seconds
4. Open: http://localhost:5173

**No configuration changes needed!** Everything switches automatically based on the environment.

**To deploy to production:**

1. Push to GitHub: `git push origin main`
2. GitHub Actions deploys to Azure (backend)
3. Vercel deploys frontend automatically

**That's it!** 🎉

---

## 🆘 Need Help?

Check the detailed guides:
- Local issues: See `LOCAL_DEVELOPMENT.md`
- Production issues: See `AZURE_CONFIG.md`
- Deployment issues: See `FRONTEND_DEPLOYMENT.md`

Or check the logs:
```bash
docker-compose logs -f
```
