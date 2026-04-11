-- ============================================
-- Script: Create Database and Tables
-- Idempotent: Safe to run multiple times
-- ============================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DeviceManagerDB')
BEGIN
    CREATE DATABASE DeviceManagerDB;
    PRINT 'Database DeviceManagerDB created.';
END
ELSE
BEGIN
    PRINT 'Database DeviceManagerDB already exists. Skipping.';
END
GO

-- Wait for DB to be ready
WAITFOR DELAY '00:00:02';
GO

USE DeviceManagerDB;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        Name        NVARCHAR(100) NOT NULL,
        Role        NVARCHAR(100) NOT NULL,
        Location    NVARCHAR(100) NOT NULL
    );
    PRINT 'Table Users created.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Devices')
BEGIN
    CREATE TABLE Devices (
        Id               INT IDENTITY(1,1) PRIMARY KEY,
        Name             NVARCHAR(100) NOT NULL,
        Manufacturer     NVARCHAR(100) NOT NULL,
        Type             NVARCHAR(50)  NOT NULL,
        OperatingSystem  NVARCHAR(50)  NOT NULL,
        OsVersion        NVARCHAR(50)  NOT NULL,
        Processor        NVARCHAR(100) NOT NULL,
        RamAmount        NVARCHAR(50)  NOT NULL,
        Description      NVARCHAR(500) NULL,
        AssignedUserId   INT NULL,
        CONSTRAINT FK_Device_User
            FOREIGN KEY (AssignedUserId)
            REFERENCES Users(Id)
            ON DELETE SET NULL
    );
    PRINT 'Table Devices created.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppUsers')
BEGIN
    CREATE TABLE AppUsers (
        Id            INT IDENTITY(1,1) PRIMARY KEY,
        Email         NVARCHAR(256) NOT NULL,
        PasswordHash  NVARCHAR(500) NOT NULL,
        UserId        INT NULL,
        CONSTRAINT UQ_AppUser_Email UNIQUE (Email),
        CONSTRAINT FK_AppUser_User
            FOREIGN KEY (UserId)
            REFERENCES Users(Id)
            ON DELETE SET NULL
    );
    PRINT 'Table AppUsers created.';
END
GO

PRINT 'Database setup complete!';
GO