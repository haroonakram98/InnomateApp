# 🔥 Hot Reload Quick Start

## Option 1: Automated Setup (Easiest!)

**Just double-click this file:**
```
start-dev.bat
```

This will:
1. ✅ Start database in Docker
2. ✅ Open backend terminal with hot reload
3. ✅ Open frontend terminal with hot reload

---

## Option 2: Manual Setup

### Step 1: Start Database
```bash
docker-compose up -d sqlserver
```

### Step 2: Start Backend (New Terminal)
```bash
cd backend/InnomateApp.API
dotnet watch run
```

### Step 3: Start Frontend (New Terminal)
```bash
cd frontend
npm run dev
```

---

## 🌐 Access Your App

- **Frontend**: http://localhost:5173 ⚡ Instant updates
- **Backend**: https://localhost:7219/swagger 🔄 Auto-restart
- **Database**: localhost:1433 🐳 Docker

---

## 🎯 How Hot Reload Works

### Frontend (Vite)
- Edit any file in `frontend/src/`
- Save → Browser updates **instantly** ⚡
- No page refresh needed!

### Backend (.NET)
- Edit any `.cs` file
- Save → API **auto-restarts** in 2-5 seconds 🔄
- Refresh browser or make new API call

---

## 🛑 Stop Development

Press `Ctrl+C` in each terminal to stop.

---

## 📚 Full Documentation

See `HOT_RELOAD_SETUP.md` for:
- Detailed setup instructions
- Troubleshooting
- IDE integration
- Advanced configurations

---

## 💡 Pro Tips

1. **Keep 3 terminals open** for easy monitoring
2. **Use VS Code** for best hot reload experience
3. **Check logs** if something doesn't reload
4. **Frontend errors** show in browser overlay

---

**Happy coding!** 🚀
