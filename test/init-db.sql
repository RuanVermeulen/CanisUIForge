USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TestApiDb')
BEGIN
    CREATE DATABASE TestApiDb;
END
GO

USE TestApiDb;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    CREATE TABLE Customers
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        Price DECIMAL(18,2) NOT NULL,
        StockQuantity INT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CustomerId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        OrderDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
        CONSTRAINT FK_Orders_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
    );
END
GO

INSERT INTO Customers (FirstName, LastName, Email) VALUES
    ('John', 'Doe', 'john.doe@example.com'),
    ('Jane', 'Smith', 'jane.smith@example.com'),
    ('Bob', 'Johnson', 'bob.johnson@example.com');
GO

INSERT INTO Products (Name, Description, Price, StockQuantity) VALUES
    ('Widget A', 'A standard widget', 19.99, 100),
    ('Widget B', 'A premium widget', 49.99, 50),
    ('Gadget X', 'An advanced gadget', 99.99, 25);
GO

INSERT INTO Orders (CustomerId, ProductId, Quantity, Status) VALUES
    (1, 1, 2, 'Completed'),
    (1, 3, 1, 'Pending'),
    (2, 2, 3, 'Processing');
GO
