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
-- TABLA: USUARIOS
-- =============================================
CREATE TABLE [dbo].[Usuarios] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(50) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [Role] NVARCHAR(50) NOT NULL,
    [Nombre] NVARCHAR(100) NULL,
    [Email] NVARCHAR(100) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2(7) NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_Usuarios_Role] CHECK ([Role] IN ('Admin', 'User'))
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Usuarios_Username]
    ON [dbo].[Usuarios]([Username] ASC);
GO

-- =============================================
-- TABLA: CLIENTES
-- =============================================
CREATE TABLE [dbo].[Clientes] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(100) NOT NULL,
    [RUC] NVARCHAR(13) NOT NULL,
    [Direccion] NVARCHAR(200) NULL,
    [Telefono] NVARCHAR(20) NULL,
    [Email] NVARCHAR(100) NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2(7) NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_Clientes_RUC] CHECK (LEN([RUC]) = 13),
    CONSTRAINT [FK_Clientes_Usuarios] FOREIGN KEY ([CreatedBy]) REFERENCES [Usuarios]([Id])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Clientes_RUC]
    ON [dbo].[Clientes]([RUC] ASC);
GO

-- =============================================
-- TABLA: PRODUCTOS
-- =============================================
CREATE TABLE [dbo].[Productos] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(100) NOT NULL,
    [CodigoBarra] NVARCHAR(50) NULL,
    [Descripcion] NVARCHAR(500) NULL,
    [Precio] DECIMAL(18,2) NOT NULL,
    [Stock] INT NOT NULL DEFAULT 0,
    [FechaVencimiento] DATE NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2(7) NULL,
    CONSTRAINT [PK_Productos] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_Productos_Precio] CHECK ([Precio] >= 0),
    CONSTRAINT [CK_Productos_Stock] CHECK ([Stock] >= 0),
    CONSTRAINT [FK_Productos_Usuarios] FOREIGN KEY ([CreatedBy]) REFERENCES [Usuarios]([Id])
);
GO

-- =============================================
-- TRIGGERS PARA ACTUALIZAR UpdatedAt AUTOMÁTICAMENTE
-- =============================================
CREATE TRIGGER trg_Usuarios_Update
ON [dbo].[Usuarios]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Usuarios]
    SET UpdatedAt = SYSUTCDATETIME()
    FROM inserted i
    WHERE [dbo].[Usuarios].Id = i.Id;
END;
GO

CREATE TRIGGER trg_Clientes_Update
ON [dbo].[Clientes]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Clientes]
    SET UpdatedAt = SYSUTCDATETIME()
    FROM inserted i
    WHERE [dbo].[Clientes].Id = i.Id;
END;
GO

CREATE TRIGGER trg_Productos_Update
ON [dbo].[Productos]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Productos]
    SET UpdatedAt = SYSUTCDATETIME()
    FROM inserted i
    WHERE [dbo].[Productos].Id = i.Id;
END;
GO

-- =============================================
-- INSERTAR USUARIOS DE EJEMPLO
-- =============================================
SET IDENTITY_INSERT [dbo].[Usuarios] ON;
GO

INSERT INTO [dbo].[Usuarios] ([Id], [Username], [PasswordHash], [Role], [Nombre], [Email])
VALUES
(1, N'user1', N'User123!', N'User', N'Juan Pérez', N'juan.perez@email.com'),
(2, N'user2', N'User123!', N'User', N'María Gómez', N'maria.gomez@email.com'),
(3, N'user3', N'User123!', N'User', N'Carlos López', N'carlos.lopez@email.com'),
(4, N'user4', N'User123!', N'User', N'Ana Torres', N'ana.torres@email.com');
GO

SET IDENTITY_INSERT [dbo].[Usuarios] OFF;
GO

-- =============================================
-- INSERTAR CLIENTES DE EJEMPLO
-- =============================================
INSERT INTO [dbo].[Clientes] ([Nombre], [RUC], [Direccion], [Telefono], [Email], [CreatedBy])
VALUES
('Empresa Alfa', '1790012345001', 'Av. Principal 123', '0999999991', 'contacto@alfa.com', 1),
('Empresa Beta', '1790012345002', 'Calle Secundaria 45', '0999999992', 'contacto@beta.com', 2),
('Empresa Gamma', '1790012345003', 'Av. Central 12', '0999999993', 'contacto@gamma.com', 3);
GO

-- =============================================
-- INSERTAR PRODUCTOS DE EJEMPLO
-- =============================================
INSERT INTO [dbo].[Productos] ([Nombre], [CodigoBarra], [Descripcion], [Precio], [Stock], [FechaVencimiento], [CreatedBy])
VALUES
('Producto A', '1234567890123', 'Descripción del Producto A', 12.50, 100, '2026-12-31', 1),
('Producto B', '1234567890124', 'Descripción del Producto B', 25.00, 50, '2025-11-30', 2),
('Producto C', '1234567890125', 'Descripción del Producto C', 8.75, 200, '2027-01-15', 3);
GO

-- =============================================
-- BACKUP AUTOMÁTICO UNIVERSAL
-- =============================================
DECLARE @BackupPath NVARCHAR(400);
DECLARE @UserDesktop NVARCHAR(400);

-- Obtiene la ruta del escritorio del usuario actual de Windows
EXEC master.dbo.xp_regread
    'HKEY_CURRENT_USER',
    'Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders',
    'Desktop',
    @UserDesktop OUTPUT;

SET @BackupPath = @UserDesktop + '\ProyectoAgiles\Backups\FacturacionSRI_Sprint1.bak';

BEGIN TRY
    BACKUP DATABASE [FacturacionSRI]
    TO DISK = @BackupPath
    WITH NOFORMAT, INIT, NAME = N'FacturacionSRI-Universal', SKIP, NOREWIND, NOUNLOAD, STATS = 10;
    PRINT N'✅ Backup creado correctamente en: ' + @BackupPath;
END TRY
BEGIN CATCH
    PRINT N'⚠️ No se pudo crear el backup. Verifica permisos o crea la carpeta "ProyectoAgiles\Backups" en tu Escritorio.';
END CATCH;
GO

-- =============================================
-- MENSAJE FINAL
-- =============================================
PRINT '✅ Base de datos FacturacionSRI creada con éxito!';
PRINT '✅ Usuario admin creado: admin / Admin123!';
GO
