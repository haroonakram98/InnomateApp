# Detailed Deployment Technical Specification

This document provides a deep dive into **What** components are used, **Why** they were chosen, and **How** they are configured for the InnomateApp deployment.

---

## 1. The Database Layer (SQL Server)

### **What?**
We are using `mcr.microsoft.com/mssql/server:2017-latest`.

### **Why?**
*   **Stability**: Newer versions (2019/2022) have stricter security profiles that often conflict with Windows/WSL2 host file permissions when using Docker volumes. Version 2017 is the "Goldilocks" version—it has all the features needed for .NET 8 but is far more forgiving of local environment configurations.
*   **Resource Efficiency**: It has a slightly lower memory footprint than 2022, which is better for local development.

### **How?**
*   **User Management**: We rely on the default `sa` user with a strong password. We removed `user: root` overrides to let the container manage its own internal permissions correctly.
*   **Persistence**: We use a named volume `sql-data-2017` mapped to `/var/opt/mssql`. This ensures that even if you stop or delete the container, your data remains safe.

---

## 2. The Backend Layer (.NET API)

### **What?**
A .NET 8 Web API containerized using a **Multi-Stage Build**.

### **Why?**
*   **Multi-Stage Build**: We use `dotnet/sdk` for building the code and `dotnet/aspnet` for running it. This keeps the final image small and secure because it doesn't contain the compiler or source code.
*   **Environment-Specific Configs**: We use `appsettings.Docker.json` to store settings specific to the container environment (like the database connection string).

### **How?**
*   **DNS Resolution**: Instead of using an IP address, the connection string uses `Server=sqlserver`. Docker's internal DNS resolver automatically maps this to the correct container.
*   **Resiliency**: We implemented a `DatabaseInitializer` class.
    *   **What it does**: It pings the database and tries to apply migrations.
    *   **Why**: Containers start in parallel. The API usually starts faster than the SQL engine. Without this, the API would crash on startup.
*   **Connectivity**: We disabled `UseHttpsRedirection` in Docker to avoid "SSL Handshake" errors in a local environment where self-signed certificates are not configured.

---

## 3. The Frontend Layer (React + Nginx)

### **What?**
A Vite-based React SPA (Single Page Application) served by **Nginx 1.28**.

### **Why?**
*   **Performance**: Nginx is extremely fast at serving static files (HTML/JS/CSS).
*   **SPA Handling**: Standard web servers don't understand React routing. If you refresh on `/sales`, the server looks for a folder called sales. Nginx allows us to redirect these requests back to `index.html`.

### **How?**
*   **Build-Time Injection**: 
    1.  Vite replaces `import.meta.env.VITE_API_URL` during the `npm run build` command.
    2.  We use `ARG` in the Dockerfile to "pass" this URL from `docker-compose.yml` into the build process.
    3.  This is critical: **Changing the environment variable in docker-compose requires a --build to take effect.**
*   **Nginx Config**: We override the default config with `nginx.conf`:
    ```nginx
    try_files $uri $uri/ /index.html;
    ```
    This single line makes the "Refresh" button work on all your app pages.

---

## 4. Orchestration (Docker Compose)

### **What?**
The "glue" that holds everything together.

### **Why?**
It allows us to define the entire "infrastructure as code." Anyone with Docker installed can run your entire app with exactly one command.

### **How?**
*   **depends_on**: We use this to tell Docker the order: DB starts -> API starts -> Frontend starts.
*   **Port Mapping**:
    *   `5173:80`: External port 5173 maps to Nginx port 80.
    *   `8080:8080`: API port exposed for the browser to call.
    *   `1433:1433`: SQL port (optional, but good for connecting via Management Studio).

---

## 5. Deployment Commands Cheat Sheet

| Action | Command |
| :--- | :--- |
| **Start everything** | `docker-compose up -d` |
| **Full Rebuild** | `docker-compose up -d --build` |
| **Stop everything** | `docker-compose down` |
| **Check Logs** | `docker logs -f innomate-api` |
| **Reset Database** | `docker volume rm innomateapp_sql-data-2017` |

## docker-compose up -d for local development
## docker-compose --env-file .env.production -f docker-compose.yml -f docker-compose.prod.yml up -d --build
