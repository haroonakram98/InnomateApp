#!/bin/bash

# Wait for SQL Server to start
sleep 10s

# Try to connect and create database
for i in {1..10}; do
    if /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Innomate@123" -Q "SELECT 1" > /dev/null 2>&1; then
        echo "SQL Server is ready, creating database..."
        
        # Create database if it doesn't exist
        /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Innomate@123" -Q "
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'InnomateCore')
BEGIN
    CREATE DATABASE InnomateCore;
END
"
        
        if [ $? -eq 0 ]; then
            echo "Database InnomateCore created successfully"
            break
        fi
    else
        echo "Waiting for SQL Server... attempt $i"
        sleep 5s
    fi
done

echo "Database initialization completed"
