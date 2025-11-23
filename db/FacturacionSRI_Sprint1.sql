USE master;
GO

-- =============================================
-- ELIMINAR BASE SI EXISTE
-- =============================================
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'FacturacionSRI')
BEGIN
    ALTER DATABASE FacturacionSRI SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE FacturacionSRI;
END
GO

-- =============================================
-- CREAR BASE DE DATOS
-- =============================================
CREATE DATABASE FacturacionSRI;
GO

USE FacturacionSRI;
GO

-- =============================================
-- TABLA: Usuarios
-- =============================================
CREATE TABLE [dbo].[Usuarios] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(50) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [Role] NVARCHAR(20) NOT NULL DEFAULT 'User',
    [Nombre] NVARCHAR(100) NULL,
    [Email] NVARCHAR(100) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedAt] DATETIME2(7) NULL,
    CONSTRAINT PK_Usuarios PRIMARY KEY (Id),
    CONSTRAINT CK_Usuarios_Role CHECK (Role IN ('Admin', 'User'))
);

CREATE UNIQUE INDEX IX_Usuarios_Username ON Usuarios(Username);
CREATE UNIQUE INDEX IX_Usuarios_Email ON Usuarios(Email) WHERE Email IS NOT NULL;
GO

-- =============================================
-- CATÁLOGOS
-- =============================================
CREATE TABLE [dbo].[TipoIdentificacion] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(20) NOT NULL UNIQUE, -- RUC, CEDULA, PASAPORTE
    Descripcion NVARCHAR(50) NOT NULL
);

CREATE TABLE [dbo].[CategoriaProducto] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE [dbo].[Marca] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Activo BIT NOT NULL DEFAULT 1
);
GO

-- =============================================
-- TABLA: Clientes
-- =============================================
CREATE TABLE [dbo].[Clientes] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    TipoIdentificacionId INT NOT NULL,
    Identificacion NVARCHAR(20) NOT NULL,
    Direccion NVARCHAR(300) NULL,
    Telefono NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2(7) NULL,
    CONSTRAINT FK_Clientes_TipoIdentificacion FOREIGN KEY (TipoIdentificacionId) REFERENCES TipoIdentificacion(Id),
    CONSTRAINT FK_Clientes_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Usuarios(Id)
);

-- Único por tipo + número (solo entre activos)
CREATE UNIQUE INDEX IX_Clientes_Identificacion_Unica 
ON Clientes(TipoIdentificacionId, Identificacion) WHERE Activo = 1;
GO

-- =============================================
-- TABLA: Productos (solo catálogo principal)
-- =============================================
CREATE TABLE [dbo].[Productos] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    Codigo NVARCHAR(50) NULL,
    CodigoBarra NVARCHAR(50) NULL,
    Descripcion NVARCHAR(500) NULL,
    MarcaId INT NULL,
    CategoriaId INT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2(7) NULL,
    CONSTRAINT FK_Productos_Marca FOREIGN KEY (MarcaId) REFERENCES Marca(Id),
    CONSTRAINT FK_Productos_Categoria FOREIGN KEY (CategoriaId) REFERENCES CategoriaProducto(Id),
    CONSTRAINT FK_Productos_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Usuarios(Id)
);
GO

-- =============================================
-- TABLA: Lotes de producto (aquí va el precio y el stock real)
-- =============================================
CREATE TABLE [dbo].[ProductoLotes] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductoId INT NOT NULL,
    Lote NVARCHAR(50) NOT NULL,
    PrecioCompra DECIMAL(18,4) NOT NULL CHECK (PrecioCompra >= 0),
    PrecioVenta DECIMAL(18,4) NOT NULL CHECK (PrecioVenta >= 0),
    Stock INT NOT NULL DEFAULT 0 CHECK (Stock >= 0),
    FechaIngreso DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    FechaVencimiento DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_ProductoLotes_Producto FOREIGN KEY (ProductoId) REFERENCES Productos(Id),
    CONSTRAINT FK_ProductoLotes_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Usuarios(Id),
    CONSTRAINT UQ_ProductoLotes_Lote UNIQUE (ProductoId, Lote)
);
GO
-- =============================================
-- TABLA: Facturas
-- =============================================
CREATE TABLE [dbo].[Facturas] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    Fecha DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    Subtotal DECIMAL(18,4) NOT NULL DEFAULT 0,
    Iva DECIMAL(18,4) NOT NULL DEFAULT 0,
    Total DECIMAL(18,4) NOT NULL DEFAULT 0,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2(7) NULL,

    CONSTRAINT FK_Facturas_Cliente FOREIGN KEY (ClienteId) REFERENCES Clientes(Id),
    CONSTRAINT FK_Facturas_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Usuarios(Id)
);
GO

-- =============================================
-- TABLA: Detalles de Factura
-- =============================================
CREATE TABLE [dbo].[FacturaDetalles] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FacturaId INT NOT NULL,
    ProductoLoteId INT NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(18,4) NOT NULL,
    Subtotal DECIMAL(18,4) NOT NULL,
    Iva DECIMAL(18,4) NOT NULL,
    Total DECIMAL(18,4) NOT NULL,

    CONSTRAINT FK_Detalles_Factura FOREIGN KEY (FacturaId) REFERENCES Facturas(Id),
    CONSTRAINT FK_Detalles_ProductoLote FOREIGN KEY (ProductoLoteId) REFERENCES ProductoLotes(Id)
);
GO


-- =============================================
-- TRIGGERS UpdatedAt
-- =============================================
CREATE TRIGGER trg_Clientes_Update ON Clientes AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE c SET UpdatedAt = SYSUTCDATETIME()
    FROM Clientes c INNER JOIN inserted i ON c.Id = i.Id;
END;
GO

CREATE TRIGGER trg_Productos_Update ON Productos AFTER UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE p SET UpdatedAt = SYSUTCDATETIME()
    FROM Productos p INNER JOIN inserted i ON p.Id = p.Id;
END;
GO

-- =============================================
-- DATOS DE CATÁLOGOS
-- =============================================
INSERT INTO TipoIdentificacion (Codigo, Descripcion) VALUES
('RUC', 'Registro Único de Contribuyentes'),
('CEDULA', 'Cédula de Identidad'),
('PASAPORTE', 'Pasaporte');
GO

INSERT INTO CategoriaProducto (Nombre) VALUES ('Electrónica'), ('Alimentos'), ('Limpieza'), ('Otros');
INSERT INTO Marca (Nombre) VALUES ('Samsung'), ('Sony'), ('Apple'), ('Genérico');
GO

-- =============================================
-- USUARIO ADMIN POR DEFECTO
-- =============================================
INSERT INTO Usuarios (Username, PasswordHash, Role, Nombre, Email)
VALUES ('admin', '$2a$11$oieAI24uoMEQpdZZQiRxN.LVgOhPzgyhd85lZKFQP9Imf97b8bUh6', 'Admin', 'Administrador', 'admin@factura.com');
-- Contraseña: Admin123!
GO

-- =============================================
-- CLIENTES DE EJEMPLO
-- =============================================
INSERT INTO Clientes (Nombre, TipoIdentificacionId, Identificacion, Direccion, Telefono, Email, CreatedBy)
VALUES 
('Juan Pérez', 2, '0901234567', 'Av. Amazonas', '0991234567', 'juan@email.com', 1),
('Empresa XYZ S.A.', 1, '1791234567001', 'Av. 10 de Agosto', '022345678', 'info@xyz.com', 1),
('María Gómez', 3, 'ABC123456', 'Quito', NULL, 'maria@pasaporte.com', 1);
GO

-- =============================================
-- PRODUCTOS Y LOTES DE EJEMPLO
-- =============================================
DECLARE @Prod1 INT, @Prod2 INT;

INSERT INTO Productos (Nombre, Codigo, MarcaId, CategoriaId, CreatedBy)
VALUES ('Arroz 5kg', 'ARZ001', 4, 2, 1);
SET @Prod1 = SCOPE_IDENTITY();

INSERT INTO Productos (Nombre, Codigo, CodigoBarra, MarcaId, CategoriaId, CreatedBy)
VALUES ('Televisor 55" 4K', 'TV001', '1234567890123', 1, 1, 1);
SET @Prod2 = SCOPE_IDENTITY();

-- Lotes
INSERT INTO ProductoLotes (ProductoId, Lote, PrecioCompra, PrecioVenta, Stock, FechaIngreso, FechaVencimiento, CreatedBy)
VALUES 
VALUES 
(@Prod1, 'L2025-001', 8.50, 12.00, 150, '2025-01-15', '2026-01-15', 1),
(@Prod1, 'L2025-002', 8.70, 12.50, 80,  '2025-03-20', '2026-03-20', 1),
(@Prod2, 'LTV-2025-A', 450.00, 699.99, 10, '2025-02-10', NULL, 1);
GO

-- =============================================
-- BACKUP AUTOMÁTICO AL ESCRITORIO
-- =============================================
DECLARE @Path NVARCHAR(500);
EXEC master.dbo.xp_regread 'HKEY_CURRENT_USER', 'Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders', 'Desktop', @Path OUTPUT;

SET @Path = @Path + '\ProyectoAgiles\Backups\';

-- Crear carpeta si no existe
EXEC master.dbo.xp_cmdshell 'mkdir "' + @Path + '"', no_output;

SET @Path = @Path + 'FacturacionSRI_Completa_' + FORMAT(GETDATE(), 'yyyyMMdd_HHmm') + '.bak';

BACKUP DATABASE FacturacionSRI 
TO DISK = @Path
WITH INIT, NAME = 'FacturacionSRI-Completa', STATS = 10;

PRINT 'Base de datos FacturacionSRI creada 100% actualizada!';
PRINT 'Backup guardado en: ' + @Path;
PRINT 'Usuario admin → admin / Admin123!';
GO