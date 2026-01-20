-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'InnomateCore')
BEGIN
    CREATE DATABASE InnomateCore;
END
GO

-- Use the correct database
USE InnomateCore;
GO

-- Create __EFMigrationsHistory table for Entity Framework
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId NVARCHAR(150) NOT NULL,
        ProductVersion NVARCHAR(32) NOT NULL,
        AppliedOn DATETIME2 NULL
    );
END
GO

-- Insert a dummy migration record to indicate database is ready
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '00000000000000_Create_dummy_migration')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion, AppliedOn)
    VALUES ('00000000000000_Create_dummy_migration', '8.0.0', GETDATE());
END
GO

PRINT 'Database initialization completed successfully';
PRINT 'Database is ready for Entity Framework migrations';
PRINT 'Note: Backend will connect using Trusted_Connection to your local SQL Server';
