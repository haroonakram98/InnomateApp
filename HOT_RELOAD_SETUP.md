# Hot Reload Development Setup

This guide shows you how to enable hot reload for both frontend and backend, so changes reflect immediately without rebuilding Docker containers.

---

## 🎯 Recommended Setup: Hybrid Approach

**Best for development:**
- ✅ **Database**: Run in Docker (easiest)
- ✅ **Backend**: Run natively with hot reload
- ✅ **Frontend**: Run natively with hot reload (Vite's fast refresh)

This gives you the fastest development experience!

---

## 🚀 Quick Start: Hot Reload Setup

### Step 1: Start Only the Database in Docker

```bash
# Stop all containers first
docker-compose down

# Start only the database
docker-compose up -d sqlserver
```

### Step 2: Run Backend with Hot Reload

Open a new terminal:

```bash
# Navigate to API project
cd backend/InnomateApp.API

# Run with hot reload (watch mode)
dotnet watch run
```

**Hot reload is now active!** Any changes to `.cs` files will automatically restart the API.

### Step 3: Run Frontend with Hot Reload

Open another terminal:

```bash
# Navigate to frontend
cd frontend

# Install dependencies (first time only)
npm install

# Run with hot reload
npm run dev
```

**Vite hot reload is now active!** Changes to React components will update instantly in the browser.

---

## 🌐 Access Your App

| Service | URL | Hot Reload |
|---------|-----|------------|
| **Frontend** | http://localhost:5173 | ✅ Instant |
| **Backend** | http://localhost:5000 or https://localhost:7219 | ✅ Auto-restart |
| **Swagger** | https://localhost:7219/swagger | ✅ Auto-restart |
| **Database** | localhost:1433 | N/A (Docker) |

---

## ⚙️ Configuration for Native Backend

When running the backend natively (not in Docker), it uses different ports and settings.

### Update Frontend to Point to Native Backend

**Option A: Temporary (for this session only)**

Create `frontend/.env.local`:
```env
VITE_API_URL=https://localhost:7219/api
```

**Option B: Modify .env.development**

Edit `frontend/.env.development`:
```env
# For native backend (dotnet run)
VITE_API_URL=https://localhost:7219/api

# For Docker backend
# VITE_API_URL=http://localhost:8080/api
```

Then restart the frontend:
```bash
npm run dev
```

---

## 🔥 Hot Reload Features

### Frontend (Vite)
- ⚡ **Instant updates** for React components
- 🎨 **CSS changes** apply without page reload
- 🔄 **State preservation** during updates
- 📦 **Fast bundling** with Vite

### Backend (.NET)
- 🔄 **Auto-restart** on `.cs` file changes
- 📝 **Preserves database** connections
- ⚡ **Fast compilation** with incremental builds
- 🐛 **Debugger** can attach during watch mode

---

## 🛠️ Advanced: Docker with Volume Mounting (Alternative)

If you prefer to keep everything in Docker but still want hot reload, you can use volume mounting.

### Frontend Hot Reload in Docker

Update `docker-compose.yml` to add volume mounts:

```yaml
frontend:
  build:
    context: ./frontend
    args:
      - VITE_API_URL=${VITE_API_URL}
  container_name: ${UI_CONTAINER_NAME}
  ports:
    - "${UI_PORT}:80"
  volumes:
    - ./frontend/src:/app/src  # Mount source code
    - ./frontend/public:/app/public
  command: npm run dev  # Use dev server instead of production build
  depends_on:
    - api
```

**Note**: This requires modifying the Dockerfile to support dev mode. The native approach (Step 3 above) is simpler.

### Backend Hot Reload in Docker

Update `docker-compose.yml`:

```yaml
api:
  build:
    context: ./backend
    dockerfile: InnomateApp.API/Dockerfile
  container_name: ${API_CONTAINER_NAME}
  ports:
    - "${API_PORT}:8080"
  volumes:
    - ./backend:/src  # Mount source code
  environment:
    - ASPNETCORE_ENVIRONMENT=Docker
    - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=SmartOps;User Id=sa;Password=${DB_PASSWORD};TrustServerCertificate=True;
  command: dotnet watch run --project /src/InnomateApp.API
  depends_on:
    - sqlserver
```

**Note**: This is more complex. The hybrid approach (native backend + frontend) is recommended.

---

## 🎯 Recommended Workflow

### Daily Development

```bash
# Terminal 1: Database
docker-compose up -d sqlserver

# Terminal 2: Backend
cd backend/InnomateApp.API
dotnet watch run

# Terminal 3: Frontend
cd frontend
npm run dev
```

### Making Changes

**Frontend Changes:**
1. Edit any file in `frontend/src/`
2. Save the file
3. Browser updates instantly ⚡

**Backend Changes:**
1. Edit any `.cs` file
2. Save the file
3. API auto-restarts in ~2-5 seconds 🔄
4. Refresh browser or make new API call

**Database Changes:**
1. Add migration: `dotnet ef migrations add MigrationName --project ../InnomateApp.Infrastructure`
2. API will auto-restart and apply migration
3. Or manually: `dotnet ef database update --project ../InnomateApp.Infrastructure`

---

## 🐛 Debugging with Hot Reload

### Frontend Debugging
- Use **Browser DevTools** (F12)
- React DevTools extension
- Vite shows errors in browser overlay

### Backend Debugging

**Visual Studio:**
1. Open `InnomateApp.sln`
2. Set `InnomateApp.API` as startup project
3. Press F5 (Debug) or Ctrl+F5 (Run without debug)
4. Hot reload works in debug mode!

**VS Code:**
1. Open the project folder
2. Go to Run & Debug (Ctrl+Shift+D)
3. Select ".NET Core Launch (web)"
4. Press F5
5. Set breakpoints and debug with hot reload

**Command Line:**
```bash
# Run with debugger attached
dotnet watch run --launch-profile "InnomateApp.API"
```

---

## 📊 Performance Comparison

| Method | Frontend Reload | Backend Reload | Setup Complexity |
|--------|----------------|----------------|------------------|
| **Full Docker** | ~30s (rebuild) | ~45s (rebuild) | ⭐ Simple |
| **Hybrid (Recommended)** | ⚡ Instant | 🔄 2-5s | ⭐⭐ Easy |
| **Docker + Volumes** | ⚡ Instant | 🔄 5-10s | ⭐⭐⭐ Complex |

---

## 🔧 Troubleshooting

### Issue: Backend hot reload not working

**Check:**
```bash
# Make sure you're using 'watch' command
dotnet watch run

# Not just:
# dotnet run  ❌
```

### Issue: Frontend changes not reflecting

**Solution:**
```bash
# Stop the dev server (Ctrl+C)
# Clear cache and restart
npm run dev
```

### Issue: "Port already in use"

**Solution:**
```bash
# Backend (check port 5000 or 7219)
netstat -ano | findstr :7219
taskkill /PID <PID> /F

# Frontend (check port 5173)
netstat -ano | findstr :5173
taskkill /PID <PID> /F
```

### Issue: Database connection fails

**Check database is running:**
```bash
docker ps | findstr sqlserver

# If not running:
docker-compose up -d sqlserver
```

**Check connection string in `appsettings.Development.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartOps;User Id=sa;Password=Innomate@123;TrustServerCertificate=True;MultipleActiveResultSets=True;"
  }
}
```

---

## 💡 Pro Tips

### 1. Use Multiple Terminals
Keep 3 terminals open:
- Terminal 1: Database logs (`docker logs -f innomate-db`)
- Terminal 2: Backend (`dotnet watch run`)
- Terminal 3: Frontend (`npm run dev`)

### 2. Browser Auto-Refresh Extension
Install "Live Reload" or similar extension for even faster frontend updates.

### 3. Use .env.local for Temporary Changes
Create `frontend/.env.local` for temporary API URL changes. This file is gitignored.

### 4. Backend Logs
The `dotnet watch` command shows detailed logs. Look for:
- ✅ "Database initialization successfully completed!"
- ✅ "InnomateApp API started successfully"
- 🔄 "Restarting..." (when files change)

### 5. Frontend Error Overlay
Vite shows compilation errors directly in the browser. Fix them and the page auto-updates!

---

## 🎨 IDE Integration

### Visual Studio
- Hot reload is built-in (Edit and Continue)
- Just press F5 and start editing
- Changes apply while debugging

### VS Code
- Install "C# Dev Kit" extension
- Use built-in debugger with hot reload
- Install "Vite" extension for better frontend support

### Rider
- Hot reload works out of the box
- Use "Run with dotnet watch" configuration

---

## 📝 Summary

**Fastest Development Setup:**

```bash
# 1. Start database
docker-compose up -d sqlserver

# 2. Start backend with hot reload
cd backend/InnomateApp.API
dotnet watch run

# 3. Start frontend with hot reload
cd frontend
npm run dev
```

**Access:**
- Frontend: http://localhost:5173 (instant updates ⚡)
- Backend: https://localhost:7219/swagger (auto-restart 🔄)
- Database: localhost:1433 (Docker 🐳)

**Benefits:**
- ⚡ Instant frontend updates
- 🔄 Fast backend restarts (2-5s)
- 🐛 Easy debugging
- 💾 Database persists in Docker
- 🚀 Best development experience

---

## 🔄 Switching Back to Full Docker

When you're done developing and want to test the full Docker setup:

```bash
# Stop native processes (Ctrl+C in each terminal)

# Start full Docker stack
docker-compose down
docker-compose up -d --build
```

---

**Happy coding with hot reload!** 🔥
