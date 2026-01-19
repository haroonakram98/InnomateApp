@echo off
REM Hot Reload Development Setup Script
REM This script starts the database in Docker and opens terminals for backend and frontend

echo ========================================
echo  InnomateApp Hot Reload Setup
echo ========================================
echo.

echo [1/3] Starting database in Docker...
docker-compose up -d sqlserver

echo.
echo [2/3] Waiting for database to be ready...
timeout /t 10 /nobreak > nul

echo.
echo [3/3] Opening development terminals...
echo.

REM Open backend terminal
start "Backend (Hot Reload)" cmd /k "cd /d %~dp0backend\InnomateApp.API && echo Starting Backend with Hot Reload... && dotnet watch run"

REM Wait a bit before starting frontend
timeout /t 3 /nobreak > nul

REM Open frontend terminal
start "Frontend (Hot Reload)" cmd /k "cd /d %~dp0frontend && echo Starting Frontend with Hot Reload... && npm run dev"

echo.
echo ========================================
echo  Setup Complete!
echo ========================================
echo.
echo  Database:  localhost:1433 (Docker)
echo  Backend:   https://localhost:7219/swagger
echo  Frontend:  http://localhost:5173
echo.
echo  Two new terminal windows have opened:
echo  - Backend (dotnet watch run)
echo  - Frontend (npm run dev)
echo.
echo  Make changes to your code and see them
echo  update automatically!
echo.
echo  Press any key to close this window...
pause > nul
