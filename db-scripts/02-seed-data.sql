-- ============================================
-- Script: Seed dummy data
-- Idempotent: Only inserts if tables are empty
-- ============================================

USE DeviceManagerDB;
GO

IF NOT EXISTS (SELECT 1 FROM Users)
BEGIN
    INSERT INTO Users (Name, Role, Location) VALUES
        ('Alice Johnson',   'Software Developer',  'London Office'),
        ('Bob Smith',       'QA Engineer',          'Berlin Office'),
        ('Carol Williams',  'Project Manager',      'London Office'),
        ('David Brown',     'DevOps Engineer',      'Remote'),
        ('Eva Martinez',    'UX Designer',          'Berlin Office');
    PRINT 'Seeded 5 users.';
END
ELSE
    PRINT 'Users table already has data. Skipping.';
GO

IF NOT EXISTS (SELECT 1 FROM Devices)
BEGIN
    INSERT INTO Devices (Name, Manufacturer, Type, OperatingSystem, OsVersion, Processor, RamAmount, Description, AssignedUserId) VALUES
        ('iPhone 15 Pro',         'Apple',    'phone',  'iOS',     '17.4',  'A17 Pro',            '8GB',  NULL, 1),
        ('Galaxy S24 Ultra',      'Samsung',  'phone',  'Android', '14',    'Snapdragon 8 Gen 3', '12GB', NULL, 2),
        ('iPad Air M2',           'Apple',    'tablet', 'iPadOS',  '17.4',  'Apple M2',           '8GB',  NULL, NULL),
        ('Pixel 8 Pro',           'Google',   'phone',  'Android', '14',    'Tensor G3',          '12GB', NULL, 3),
        ('Galaxy Tab S9',         'Samsung',  'tablet', 'Android', '14',    'Snapdragon 8 Gen 2', '8GB',  NULL, NULL),
        ('iPhone 14',             'Apple',    'phone',  'iOS',     '17.3',  'A15 Bionic',         '6GB',  NULL, 4),
        ('OnePlus 12',            'OnePlus',  'phone',  'Android', '14',    'Snapdragon 8 Gen 3', '16GB', NULL, NULL),
        ('Surface Pro 9',         'Microsoft','tablet', 'Windows', '11',    'Intel Core i7-1255U','16GB', NULL, 5),
        ('Xiaomi 14 Ultra',       'Xiaomi',   'phone',  'Android', '14',    'Snapdragon 8 Gen 3', '16GB', NULL, NULL),
        ('iPad Mini 6',           'Apple',    'tablet', 'iPadOS',  '17.2',  'A15 Bionic',         '4GB',  NULL, NULL);
    PRINT 'Seeded 10 devices.';
END
ELSE
    PRINT 'Devices table already has data. Skipping.';
GO

PRINT 'Seed data complete!';
GO