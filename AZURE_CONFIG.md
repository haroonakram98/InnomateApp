# Azure Backend Configuration Checklist

## Required Environment Variables in Azure

Go to: **Azure Portal** → **InnomateApi** → **Settings** → **Environment variables**

### Connection Strings Tab
| Name | Value | Type |
|------|-------|------|
| `DefaultConnection` | `Server=tcp:innomate-server-ua.database.windows.net,1433;Initial Catalog=InnomateDB;User ID=sqladmin;Password=YOUR_PASSWORD;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;` | SQLAzure |

### App Settings Tab
| Name | Value | Description |
|------|-------|-------------|
| `AllowedOrigins` | `https://your-vercel-app.vercel.app` | **IMPORTANT**: Replace with your actual Vercel URL |
| `Jwt__Key` | `your_long_secret_key_minimum_32_characters` | JWT signing key |
| `Jwt__Issuer` | `InnomateApp` | JWT issuer |
| `Jwt__Audience` | `InnomateAppUsers` | JWT audience |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Environment name |

---

## How to Update AllowedOrigins for Vercel

### Step 1: Get Your Vercel URL
1. Go to Vercel Dashboard
2. Find your deployed app
3. Copy the URL (e.g., `https://innomate-app.vercel.app`)

### Step 2: Update Azure
1. Go to Azure Portal → InnomateApi
2. Settings → Environment variables → App settings
3. Find `AllowedOrigins`
4. Update value to: `https://your-vercel-app.vercel.app,http://localhost:5173`
   - This allows both production (Vercel) and local development
5. Click **Save**
6. **Restart** the Web App (Overview → Restart)

### Multiple Domains (if needed)
If you have preview deployments or multiple frontends:
```
https://innomate-app.vercel.app,https://innomate-app-preview.vercel.app,http://localhost:5173
```

---

## Verification

### Test CORS is Working
1. Open your Vercel app in browser
2. Open DevTools → Console
3. Try to login or make any API call
4. If you see CORS error, check:
   - ✅ `AllowedOrigins` includes your Vercel URL
   - ✅ No trailing slash in the URL
   - ✅ HTTPS (not HTTP) for Vercel
   - ✅ Web App was restarted after changing the variable

### Check Current Configuration
You can verify your settings in Azure:
- Go to Web App → Configuration
- All settings should show in the list

---

## Quick Reference

**Backend URL**: `https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net`
**Swagger**: `https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net/swagger`
**API Base**: `https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net/api`

**Database**: `innomate-server-ua.database.windows.net` → `InnomateDB`

---

## Common Issues

### Issue: CORS Error in Browser
**Error**: `Access to XMLHttpRequest at 'https://innomateapi...' from origin 'https://your-app.vercel.app' has been blocked by CORS policy`

**Solution**:
1. Add your Vercel URL to `AllowedOrigins` in Azure
2. Restart the Web App
3. Clear browser cache and try again

### Issue: 401 Unauthorized
**Check**:
- JWT token is being sent in Authorization header
- `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience` are set correctly in Azure

### Issue: 500 Internal Server Error
**Check**:
1. Azure → Log Stream (to see real-time errors)
2. Application Insights (if enabled)
3. Database connection string is correct
4. Database firewall allows Azure services

---

## Deployment Workflow

```
┌─────────────┐      Push Code       ┌──────────────┐
│   GitHub    │ ──────────────────→  │ GitHub       │
│  (main)     │                      │ Actions      │
└─────────────┘                      └──────┬───────┘
                                            │
                                            │ Deploy
                                            ↓
                                     ┌──────────────┐
                                     │   Azure      │
                                     │   Web App    │
                                     └──────┬───────┘
                                            │
                                            │ Connects to
                                            ↓
                                     ┌──────────────┐
                                     │   Azure      │
                                     │   SQL DB     │
                                     └──────────────┘

┌─────────────┐      Push Code       ┌──────────────┐
│   GitHub    │ ──────────────────→  │   Vercel     │
│  (main)     │                      │   Auto       │
└─────────────┘                      │   Deploy     │
                                     └──────┬───────┘
                                            │
                                            │ Calls API
                                            ↓
                                     ┌──────────────┐
                                     │   Azure      │
                                     │   Backend    │
                                     └──────────────┘
```

---

## Next Steps After Configuration

1. ✅ Set `AllowedOrigins` in Azure with your Vercel URL
2. ✅ Restart Azure Web App
3. ✅ Push frontend code to GitHub
4. ✅ Vercel auto-deploys
5. ✅ Test the full flow: Login → Dashboard → API calls

**All set!** Your production environment should now be fully connected.
