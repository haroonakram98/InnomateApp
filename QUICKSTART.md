# Quick Start - Local Development

## 🚀 Fastest Way (Docker - Recommended)

```bash
# Start everything
docker-compose up -d

# Wait 30-60 seconds, then access:
# Frontend: http://localhost:5173
# API: http://localhost:8080/swagger
# Database: localhost:1433
```

**That's it!** Everything is already configured.

---

## 📋 What You Need

✅ **Already Configured** (No changes needed):
- `.env` file with local settings
- `frontend/.env.development` with local API URL
- `docker-compose.yml` with all services
- Database migrations run automatically

❌ **No Changes Required** to switch between local and production!

---

## 🔄 Common Commands

```bash
# Start
docker-compose up -d

# Stop
docker-compose down

# View logs
docker-compose logs -f

# Rebuild after code changes
docker-compose up -d --build

# Reset database (⚠️ deletes data)
docker-compose down -v
docker-compose up -d
```

---

## 🌐 Local URLs

| Service | URL |
|---------|-----|
| Frontend | http://localhost:5173 |
| Backend API | http://localhost:8080/api |
| Swagger Docs | http://localhost:8080/swagger |
| Database | localhost:1433 (sa / Innomate@123) |

---

## 🐛 Quick Fixes

**Services won't start?**
```bash
docker-compose down
docker-compose up -d
```

**Port already in use?**
```bash
# Edit .env and change:
API_PORT=8081
UI_PORT=5174
```

**Database issues?**
```bash
# Reset database
docker-compose down -v
docker-compose up -d
```

---

## 📖 Full Documentation

See `LOCAL_DEVELOPMENT.md` for:
- Running without Docker
- Database management
- Troubleshooting
- Development workflows

---

## ✨ Pro Tips

1. **Hot Reload**: Frontend changes auto-reload when running `npm run dev`
2. **API Changes**: Rebuild with `docker-compose up -d --build api`
3. **Database**: Persists between restarts (stored in Docker volume)
4. **Logs**: Use `docker logs -f innomate-api` to debug

---

**Need help?** Check `LOCAL_DEVELOPMENT.md` or the troubleshooting section.
