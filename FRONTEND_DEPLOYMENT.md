# Frontend Production Deployment Guide

## Overview
This guide will help you deploy your frontend to Vercel with the correct production API configuration.

## Production API URL
Your backend is deployed at:
```
https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net/api
```

## Environment Configuration

### Local Development
- File: `.env.development`
- API URL: `http://localhost:8080/api` (Docker)

### Production (Vercel)
- File: `.env.production`
- API URL: `https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net/api`

---

## Deployment Steps

### Option 1: Deploy via Vercel Dashboard (Recommended)

1. **Go to Vercel Dashboard**
   - Visit: https://vercel.com/dashboard
   - Select your project (or create new if first time)

2. **Configure Environment Variables**
   - Go to: **Settings** → **Environment Variables**
   - Add the following variable:
     - **Name**: `VITE_API_URL`
     - **Value**: `https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net/api`
     - **Environment**: Select **Production** (and optionally Preview)
   - Click **Save**

3. **Deploy**
   - Go to **Deployments** tab
   - Click **Redeploy** on the latest deployment
   - OR: Simply push your code to GitHub (if connected)

### Option 2: Deploy via Git Push

1. **Commit the environment files**
   ```bash
   git add frontend/.env.production frontend/.env.development
   git commit -m "Add production API configuration"
   git push origin main
   ```

2. **Vercel will auto-deploy** (if GitHub integration is set up)
   - Vercel automatically detects `.env.production` for production builds
   - The build will use the Azure API URL

---

## Verification Steps

### 1. Check Build Logs
- In Vercel dashboard, click on your deployment
- Check the build logs for: `VITE_API_URL` being set correctly

### 2. Test the Deployed App
- Open your Vercel URL (e.g., `https://innomate-app.vercel.app`)
- Open browser DevTools → Network tab
- Try to login or make any API call
- Verify requests go to: `https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net/api/...`

### 3. Check CORS Configuration
Your Azure backend needs to allow your Vercel domain. Ensure this is set in Azure:

**Azure Web App → Environment Variables → App Settings:**
- Name: `AllowedOrigins`
- Value: `https://your-app.vercel.app` (replace with your actual Vercel URL)

---

## Troubleshooting

### Issue: "Network Error" or "CORS Error"
**Solution**: Update Azure CORS settings
1. Go to Azure Portal → Your Web App
2. Settings → Environment Variables → App Settings
3. Update `AllowedOrigins` to include your Vercel URL
4. Example: `https://innomate-app.vercel.app,https://innomate-app-preview.vercel.app`

### Issue: API calls still go to localhost
**Solution**: Clear Vercel cache and redeploy
1. In Vercel dashboard, go to Settings → General
2. Scroll to "Build & Development Settings"
3. Click "Clear Cache"
4. Redeploy

### Issue: Environment variable not picked up
**Solution**: Ensure variable name is exact
- Must be: `VITE_API_URL` (case-sensitive)
- Vite only picks up variables starting with `VITE_`

---

## Quick Commands

### Local Development (with Docker)
```bash
# Start all services
docker-compose up -d

# Frontend will use: http://localhost:8080/api
npm run dev
```

### Build for Production Locally (Test)
```bash
cd frontend
npm run build

# This will use .env.production
# Check dist/assets/*.js to verify API URL is baked in
```

### Deploy to Vercel via CLI
```bash
# Install Vercel CLI (if not installed)
npm i -g vercel

# Deploy
cd frontend
vercel --prod

# Set environment variable via CLI
vercel env add VITE_API_URL production
# Then paste: https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net/api
```

---

## Important Notes

1. **Environment files are NOT committed to Git by default**
   - `.env.production` is now in your repo for team consistency
   - Vercel will use it automatically during build

2. **Vite bakes environment variables at BUILD time**
   - Changing `VITE_API_URL` in Vercel requires a **redeploy**
   - It's not a runtime variable

3. **Multiple Environments**
   - `.env.development` → Used during `npm run dev`
   - `.env.production` → Used during `npm run build`
   - Vercel Environment Variables → Override `.env.production` if set

---

## Current Configuration Summary

| Environment | API URL |
|------------|---------|
| **Local Dev** | `http://localhost:8080/api` |
| **Production (Vercel)** | `https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net/api` |

**Backend (Azure)**: `https://innomateapi-hqgagzcggda6fjd6.eastasia-01.azurewebsites.net`
**Frontend (Vercel)**: *Your Vercel URL* (e.g., `https://innomate-app.vercel.app`)

---

## Next Steps

1. ✅ Environment files created
2. ⏳ Push to GitHub
3. ⏳ Configure Vercel environment variable (if not using .env.production)
4. ⏳ Update Azure CORS with your Vercel URL
5. ⏳ Deploy and test

**Need help?** Check the Vercel deployment logs or Azure Application Insights for errors.
