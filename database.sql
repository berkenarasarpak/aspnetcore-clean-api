-- Clean Architecture API Database Setup
-- SQL Server

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CleanApiDb')
BEGIN
    CREATE DATABASE CleanApiDb;
END
GO

USE CleanApiDb;
GO

-- Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(500) NOT NULL,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Role NVARCHAR(50) NOT NULL DEFAULT 'User',
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        LastLoginAt DATETIME2 NULL
    );
    
    CREATE INDEX IX_Users_Email ON Users(Email);
    CREATE INDEX IX_Users_Role ON Users(Role);
END
GO

-- Products Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        Sku NVARCHAR(100) NOT NULL UNIQUE,
        Price DECIMAL(18, 2) NOT NULL,
        StockQuantity INT NOT NULL DEFAULT 0,
        Category NVARCHAR(100) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );
    
    CREATE INDEX IX_Products_Sku ON Products(Sku);
    CREATE INDEX IX_Products_Category ON Products(Category);
END
GO

-- Orders Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        TotalAmount DECIMAL(18, 2) NOT NULL,
        ShippingAddress NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CompletedAt DATETIME2 NULL,
        CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
    
    CREATE INDEX IX_Orders_UserId ON Orders(UserId);
    CREATE INDEX IX_Orders_OrderNumber ON Orders(OrderNumber);
END
GO

-- OrderItems Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        OrderId UNIQUEIDENTIFIER NOT NULL,
        ProductId UNIQUEIDENTIFIER NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18, 2) NOT NULL,
        CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
END
GO

-- Seed Data
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@cleanapi.com')
BEGIN
    INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedAt)
    VALUES (
        NEWID(),
        'admin@cleanapi.com',
        '$2a$11$DQgMlSV2s2G9L.B.G9d4gO82F3.s6P/1Ahfq6nBJ9U.VH7yJh3P1y',
        'Admin',
        'User',
        'Admin',
        1,
        GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'user@cleanapi.com')
BEGIN
    INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedAt)
    VALUES (
        NEWID(),
        'user@cleanapi.com',
        '$2a$11$DQgMlSV2s2G9L.B.G9d4gO82F3.s6P/1Ahfq6nBJ9U.VH7yJh3P1y',
        'Regular',
        'User',
        'User',
        1,
        GETUTCDATE()
    );
END
GO

-- Sample Products
IF NOT EXISTS (SELECT * FROM Products)
BEGIN
    INSERT INTO Products (Id, Name, Description, Sku, Price, StockQuantity, Category, IsActive, CreatedAt)
    VALUES
    (NEWID(), 'Laptop Pro X1', 'High-performance laptop for professionals', 'LAP-001', 1299.99, 50, 'Electronics', 1, GETUTCDATE()),
    (NEWID(), 'Wireless Mouse', 'Ergonomic wireless mouse with long battery life', 'MOU-001', 29.99, 200, 'Electronics', 1, GETUTCDATE()),
    (NEWID(), 'Mechanical Keyboard', 'RGB mechanical keyboard with Cherry MX switches', 'KEY-001', 149.99, 75, 'Electronics', 1, GETUTCDATE()),
    (NEWID(), 'USB-C Hub', '7-in-1 USB-C hub with HDMI and card reader', 'USB-001', 59.99, 100, 'Electronics', 1, GETUTCDATE()),
    (NEWID(), 'Webcam 4K', '4K webcam with auto-focus and noise cancellation', 'CAM-001', 89.99, 30, 'Electronics', 1, GETUTCDATE()),
    (NEWID(), 'Monitor 27"', '27-inch 4K monitor with IPS panel', 'MON-001', 399.99, 25, 'Electronics', 1, GETUTCDATE()),
    (NEWID(), 'Desk Chair', 'Ergonomic office chair with lumbar support', 'CHR-001', 299.99, 15, 'Furniture', 1, GETUTCDATE()),
    (NEWID(), 'Standing Desk', 'Electric standing desk with memory presets', 'DES-001', 549.99, 10, 'Furniture', 1, GETUTCDATE());
END
GO

PRINT 'Database setup completed successfully!';
