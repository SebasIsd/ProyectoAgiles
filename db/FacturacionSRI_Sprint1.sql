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
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL,
    -- Asumimos bcrypt/argon2/etc. VARCHAR(255 es suficiente)
    [PasswordHash] VARCHAR(255) NOT NULL,
    [Role] NVARCHAR(20) NOT NULL DEFAULT 'User',
    [Nombre] NVARCHAR(100) NULL,
    [Email] NVARCHAR(100) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedAt] DATETIME2(7) NULL,
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
    CONSTRAINT FK_Clientes_TipoIdentificacion FOREIGN KEY (TipoIdentificacionId) REFERENCES TipoIdentificacion(Id)
    -- FK to Usuarios.CreatedBy se agrega más abajo con nombre controlado
);

-- Único por tipo + número (solo entre activos)
CREATE UNIQUE INDEX IX_Clientes_Identificacion_Unica 
ON Clientes(TipoIdentificacionId, Identificacion) WHERE Activo = 1;
GO

-- =============================================
-- TABLA: Productos (catálogo principal)
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
    CONSTRAINT FK_Productos_Categoria FOREIGN KEY (CategoriaId) REFERENCES CategoriaProducto(Id)
    -- FK to Usuarios.CreatedBy se agrega más abajo
);
GO

-- =============================================
-- TABLA: Lotes de producto (precio y stock)
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
    CONSTRAINT UQ_ProductoLotes_Lote UNIQUE (ProductoId, Lote)
    -- FK to Usuarios.CreatedBy se agrega más abajo
);
GO

-- =============================================
-- Empresa, Secuenciales, FormasPago
-- =============================================
CREATE TABLE Empresa (
    Id INT PRIMARY KEY IDENTITY,
    Ruc NVARCHAR(13) NOT NULL,
    RazonSocial NVARCHAR(300) NOT NULL,
    NombreComercial NVARCHAR(300),
    DireccionMatriz NVARCHAR(300),
    ContribuyenteEspecial NVARCHAR(10),
    ObligadoContabilidad BIT NOT NULL DEFAULT 1,
    LogoPath NVARCHAR(500),
    Activo BIT DEFAULT 1
);

CREATE TABLE Secuenciales (
    Id INT PRIMARY KEY IDENTITY,
    Establecimiento CHAR(3) NOT NULL,      -- 001
    PuntoEmision CHAR(3) NOT NULL,         -- 001
    TipoComprobante CHAR(2) NOT NULL,      -- 01 = Factura
    SecuencialActual INT NOT NULL DEFAULT 0,
    Activo BIT DEFAULT 1,
    UNIQUE(Establecimiento, PuntoEmision, TipoComprobante)
);

CREATE TABLE FormasPago (
    Id INT PRIMARY KEY IDENTITY,
    CodigoSRI NVARCHAR(2) NOT NULL,  -- 01, 15, 16, 17...20
    Descripcion NVARCHAR(100) NOT NULL,
    Activo BIT DEFAULT 1
);
GO

-- =============================================
-- Facturas (cabecera)
-- =============================================
CREATE TABLE Facturas (
    Id INT PRIMARY KEY IDENTITY,
    Numero NVARCHAR(20) NOT NULL,                    -- 001-001-000000001
    ClaveAcceso NVARCHAR(49) NULL,
    FechaEmision DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    ClienteId INT NOT NULL,
    Subtotal DECIMAL(18,4) NOT NULL,
    TotalDescuento DECIMAL(18,4) NOT NULL DEFAULT 0,
    Iva DECIMAL(18,4) NOT NULL,
    Total DECIMAL(18,4) NOT NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Borrador', -- Borrador, Pendiente, Autorizada, Rechazada, Anulada
    Ambiente TINYINT NOT NULL DEFAULT 1,             -- 1=Pruebas, 2=Producción
    XmlFirmado XML NULL,
    NumeroAutorizacion NVARCHAR(49) NULL,
    FechaAutorizacion DATETIME2 NULL,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Facturas_Cliente FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
    -- FK to Usuarios.CreatedBy se agrega más abajo
);
GO

-- =============================================
-- Detalle de factura
-- =============================================
CREATE TABLE FacturaDetalles (
    Id INT PRIMARY KEY IDENTITY,
    FacturaId INT NOT NULL,
    ProductoId INT NOT NULL,
    ProductoLoteId INT NULL,           -- opcional, para trazabilidad
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,4) NOT NULL,
    SubtotalLinea DECIMAL(18,4) NOT NULL,
    PorcentajeIva DECIMAL(5,2) NOT NULL DEFAULT 15,
    ValorIva DECIMAL(18,4) NOT NULL,
    TotalLinea DECIMAL(18,4) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CreatedBy INT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2(7) NULL,
    CONSTRAINT FK_FacturaDetalles_Factura FOREIGN KEY (FacturaId) REFERENCES Facturas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_FacturaDetalles_Producto FOREIGN KEY (ProductoId) REFERENCES Productos(Id),
    CONSTRAINT FK_FacturaDetalles_Lote FOREIGN KEY (ProductoLoteId) REFERENCES ProductoLotes(Id)
);
GO

-- =============================================
-- Formas de pago de la factura
-- =============================================
CREATE TABLE FacturaFormasPago (
    Id INT PRIMARY KEY IDENTITY,
    FacturaId INT NOT NULL,
    FormaPagoId INT NOT NULL,
    Valor DECIMAL(18,4) NOT NULL,
    Plazo INT NULL,              -- días de crédito
    UnidadTiempo NVARCHAR(10) NULL, -- dias, meses
    CONSTRAINT FK_FacturaFormasPago_Factura FOREIGN KEY (FacturaId) REFERENCES Facturas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_FacturaFormasPago_Forma FOREIGN KEY (FormaPagoId) REFERENCES FormasPago(Id)
);
GO

-- =============================================
-- Comprobantes Electronicos (log)
-- =============================================
CREATE TABLE ComprobantesElectronicos (
    Id INT PRIMARY KEY IDENTITY,
    FacturaId INT NOT NULL,
    ClaveAcceso NVARCHAR(49) NOT NULL,
    EstadoSRI NVARCHAR(50) NULL, -- RECIBIDA, EN_PROCESO, AUTORIZADO, NO_AUTORIZADO
    XmlEnviado XML NULL,
    XmlRecibido XML NULL,
    MensajeError NVARCHAR(MAX) NULL,
    FechaEnvio DATETIME2 NULL,
    FechaAutorizacion DATETIME2 NULL,
    CONSTRAINT FK_Comprobantes_Factura FOREIGN KEY (FacturaId) REFERENCES Facturas(Id)
);
GO

-- =============================================
-- Cobros (moved here AFTER Facturas exists)
-- =============================================
CREATE TABLE Cobros (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FacturaId INT NOT NULL,
    Monto DECIMAL(18,4) NOT NULL CHECK (Monto > 0),
    MetodoPago NVARCHAR(50) NOT NULL,
    Fecha DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy INT NOT NULL,
    CONSTRAINT FK_Cobros_Factura FOREIGN KEY (FacturaId) REFERENCES Facturas(Id),
    CONSTRAINT FK_Cobros_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Usuarios(Id)
);
GO

-- =============================================
-- FK para CreatedBy en tablas que lo requieren
-- (creamos FKs con nombres fijos y chequeo existence)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Clientes_CreatedBy_Usuarios_Id')
BEGIN
    ALTER TABLE Clientes
    ADD CONSTRAINT FK_Clientes_CreatedBy_Usuarios_Id FOREIGN KEY (CreatedBy)
    REFERENCES Usuarios (Id)
    ON DELETE RESTRICT;
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Productos_CreatedBy_Usuarios_Id')
BEGIN
    ALTER TABLE Productos
    ADD CONSTRAINT FK_Productos_CreatedBy_Usuarios_Id FOREIGN KEY (CreatedBy)
    REFERENCES Usuarios (Id)
    ON DELETE RESTRICT;
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ProductoLotes_CreatedBy_Usuarios_Id')
BEGIN
    ALTER TABLE ProductoLotes
    ADD CONSTRAINT FK_ProductoLotes_CreatedBy_Usuarios_Id FOREIGN KEY (CreatedBy)
    REFERENCES Usuarios (Id)
    ON DELETE RESTRICT;
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Facturas_CreatedBy_Usuarios_Id')
BEGIN
    ALTER TABLE Facturas
    ADD CONSTRAINT FK_Facturas_CreatedBy_Usuarios_Id FOREIGN KEY (CreatedBy)
    REFERENCES Usuarios (Id)
    ON DELETE RESTRICT;
END
GO

-- =============================================
-- TRIGGERS UpdatedAt (corregidos)
-- =============================================
-- Clientes
IF OBJECT_ID('trg_Clientes_Update', 'TR') IS NOT NULL
    DROP TRIGGER trg_Clientes_Update;
GO
CREATE TRIGGER trg_Clientes_Update ON Clientes
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE c
    SET UpdatedAt = SYSUTCDATETIME()
    FROM Clientes c
    INNER JOIN inserted i ON c.Id = i.Id;
END;
GO

-- Productos (corregido: ON p.Id = i.Id)
IF OBJECT_ID('trg_Productos_Update', 'TR') IS NOT NULL
    DROP TRIGGER trg_Productos_Update;
GO
CREATE TRIGGER trg_Productos_Update ON Productos
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE p
    SET UpdatedAt = SYSUTCDATETIME()
    FROM Productos p
    INNER JOIN inserted i ON p.Id = i.Id;
END;
GO

-- ProductoLotes (opcional: actualizar UpdatedAt si existe)
IF OBJECT_ID('trg_ProductoLotes_Update', 'TR') IS NOT NULL
    DROP TRIGGER trg_ProductoLotes_Update;
GO
CREATE TRIGGER trg_ProductoLotes_Update ON ProductoLotes
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE pl
    SET CreatedAt = ISNULL(pl.CreatedAt, SYSUTCDATETIME()) -- no sobrescribimos CreatedAt
    FROM ProductoLotes pl
    INNER JOIN inserted i ON pl.Id = i.Id;
END;
GO

-- =============================================
-- DATOS DE CATÁLOGOS (ejemplo)
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
-- USUARIO ADMIN POR DEFECTO (hash de ejemplo)
-- =============================================
INSERT INTO Usuarios (Username, PasswordHash, Role, Nombre, Email)
VALUES ('admin', '$2a$11$oieAI24uoMEQpdZZQiRxN.LVgOhPzgyhd85lZKFQP9Imf97b8bUh6', 'Admin', 'Administrador', 'admin@factura.com');
-- Contraseña de ejemplo: Admin123!
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
VALUES ('Televisor 55\" 4K', 'TV001', '1234567890123', 1, 1, 1);
SET @Prod2 = SCOPE_IDENTITY();

-- Lotes
INSERT INTO ProductoLotes (ProductoId, Lote, PrecioCompra, PrecioVenta, Stock, FechaIngreso, FechaVencimiento, CreatedBy)
VALUES
(@Prod1, 'L2025-001', 8.50, 12.00, 150, '2025-01-15', '2026-01-15', 1),
(@Prod1, 'L2025-002', 8.70, 12.50, 80,  '2025-03-20', '2026-03-20', 1),
(@Prod2, 'LTV-2025-A', 450.00, 699.99, 10, '2025-02-10', NULL, 1);
GO

-- =============================================
-- BACKUP AUTOMÁTICO (intento; xp_regread/xp_cmdshell pueden estar DESHABILITADOS)
-- =============================================
DECLARE @Path NVARCHAR(500);
BEGIN TRY
    -- intenta leer Desktop
    EXEC master.dbo.xp_regread 'HKEY_CURRENT_USER', 'Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders', 'Desktop', @Path OUTPUT;
    IF @Path IS NULL OR LEN(@Path) = 0
        SET @Path = 'C:\Backups';
    SET @Path = @Path + '\ProyectoAgiles\Backups\';
    -- crear carpeta (si xp_cmdshell activo)
    EXEC master.dbo.xp_cmdshell 'mkdir "' + @Path + '"', no_output;
    SET @Path = @Path + 'FacturacionSRI_Completa_' + FORMAT(GETDATE(), 'yyyyMMdd_HHmm') + '.bak';
    BACKUP DATABASE FacturacionSRI 
    TO DISK = @Path
    WITH INIT, NAME = 'FacturacionSRI-Completa', STATS = 10;
    PRINT 'Backup guardado en: ' + @Path;
END TRY
BEGIN CATCH
    -- Si falla, escribir mensaje; el backup puede necesitar intervención manual
    PRINT 'No fue posible realizar backup automático con xp_cmdshell/xp_regread. Ejecutar BACKUP manualmente o habilitar permisos.';
    PRINT ERROR_MESSAGE();
END CATCH;
GO

PRINT 'Script ejecutado correctamente (con correcciones).';
GO
