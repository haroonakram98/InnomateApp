@echo off
REM Hot Reload Development Setup Script
REM This script starts the database in Docker and opens terminals for backend and frontend

echo ========================================
echo  InnomateApp Hot Reload Setup
echo ========================================
echo.

echo [1/3] Database Setup...
set /p START_DB="Start Docker Database? (y/n, default 'y'): "
if /i "%START_DB%" neq "n" (
    echo Starting database in Docker...
    docker-compose up -d sqlserver
    echo Waiting for database to be ready...
    timeout /t 10 /nobreak > nul
) else (
    echo Skipping Docker database. Ensure your local SQL Server is running!
)

echo.
echo [3/3] Opening development terminals...
echo.

REM Open backend terminal
start "Backend (Hot Reload)" cmd /k "cd /d %~dp0backend\InnomateApp.API && echo Starting Backend with Hot Reload... && dotnet watch run --launch-profile https"

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
echo  ⚠️  IMPORTANT:
echo  If you see network/CORS errors, visit the Backend URL
echo  (https://localhost:7219/swagger) and accept the
echo  security certificate!
echo.
echo  Press any key to close this window...
pause > nul
