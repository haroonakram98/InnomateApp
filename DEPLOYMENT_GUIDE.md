# InnomateApp Deployment & Architecture Documentation

This document provides a comprehensive overview of the **InnomateApp** architecture, specifically focusing on the Docker-based multi-tier deployment strategy, the challenges encountered during implementation, and the final stable solutions.

## 1. Product Structure & Architecture

The application follows a modern **three-tier architecture** containerized using Docker:

1.  **Frontend (UI)**: A Vite-based React application served via **Nginx**.
2.  **Backend (API)**: A .NET 8 Web API following a Clean Architecture pattern.
3.  **Database (DB)**: A SQL Server instance for persistent data storage.

---

## 2. Docker Implementation Details

The entire stack is orchestrated using `docker-compose.yml`.

### Key Components:
- **Networking**: All containers reside on a shared bridge network, allowing the API to reach the Database using the hostname `sqlserver`.
- **Volumes**: Persistent storage for SQL Server is managed via a named volume (`sql-data-2017`).
- **Build Arguments**: The frontend build process uses `ARG VITE_API_URL` to inject the correct backend endpoint at compile-time.

---

## 3. Implementation Challenges & Solutions

During the deployment process, several critical issues were encountered. Below is a detailed log of the troubleshooting steps:

### Issue 1: SQL Server Container Failing to Start
*   **Symptoms**: The SQL Server container would exit immediately with errors like `Restarting (255)` or `unable to find user mssql`.
*   **Failed Attempts**: 
    - Tried using `mssql/server:2019-latest` and `2022-latest`.
    - Tried overriding the user to `user: root` and `user: "0:0"`.
*   **Root Cause**: Permission mismatches between the host filesystem and the newer SQL Server container security models on Windows/WSL2 Docker environments.
*   **Final Solution**: Downgraded to **SQL Server 2017** (`2017-latest`). This version proved significantly more stable for the specific host environment and resolved the permission issues without requiring dangerous `root` overrides.

### Issue 2: API Connection Timeouts (Cold Start)
*   **Symptoms**: The API would start before the SQL Server was fully "ready" to accept connections, causing the application to crash or stay in a broken state.
*   **Solution**: Implemented a **Resilient Database Initializer** in the .NET code. It uses an exponential backoff retry logic (up to 10 attempts) to wait for the database to become healthy before applying migrations and seeding data.

### Issue 3: Frontend Could Not Reach the API
*   **Symptoms**: Requests from the browser were still trying to hit `https://localhost:7219` instead of the Docker port `http://localhost:8080`.
*   **Root Cause**: Hardcoded fallback values in the frontend code were being "baked" into the production build.
*   **Final Solution**: 
    - Modified the `frontend/Dockerfile` to create a `.env` file dynamically during the build stage.
    - Used `docker-compose` build arguments to pass `http://localhost:8080/api` into the container.

### Issue 4: 404 Error on Page Refresh (SPA Routing)
*   **Symptoms**: Refreshing any page other than the home page resulted in an Nginx 404 error.
*   **Root Cause**: Nginx was looking for physical files (e.g., `/dashboard`) instead of letting the React Router handle the request.
*   **Final Solution**: Created a custom `nginx.conf` with a `try_files` directive:
    ```nginx
    location / {
        try_files $uri $uri/ /index.html;
    }
    ```
    This ensures all non-file requests are routed to `index.html`, allowing the frontend to handle navigation.

---

## 4. Final Stable Pattern (The "Winning" Formula)

| Layer | Technology | Key Configuration |
| :--- | :--- | :--- |
| **Orchestration** | Docker Compose | Uses `depends_on` and custom `networks`. |
| **Frontend** | React/Vite + Nginx | Build-time injected API URLs + SPA routing fix. |
| **Backend** | .NET 8 API | Resilient `DatabaseInitializer` + Disabled HTTPS Redirection in Docker. |
| **Database** | SQL Server 2017 | Named volumes for persistence. |

## 5. Summary of URLs
- **Frontend**: [http://localhost:5173](http://localhost:5173)
- **Backend API**: [http://localhost:8080](http://localhost:8080)
- **Swagger UI**: [http://localhost:8080/swagger](http://localhost:8080/swagger)
