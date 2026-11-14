-- =====================================================
-- MODELO DE DATOS: REGISTRO DE ROPA PARA BOUTIQUE
-- Motor: SQL Server 2022 Express
-- Se puede ejecutar varias veces: borra y vuelve a crear las tablas
-- =====================================================

IF DB_ID('BoutiqueDB') IS NULL
    CREATE DATABASE BoutiqueDB;
GO

USE BoutiqueDB;
GO

-- Se borran primero las tablas que dependen de otras
DROP TABLE IF EXISTS Ventas;
DROP TABLE IF EXISTS Prendas;
DROP TABLE IF EXISTS Proveedores;
DROP TABLE IF EXISTS Tallas;
DROP TABLE IF EXISTS Categorias;
GO

-- =====================================================
-- TABLAS
-- =====================================================

CREATE TABLE Categorias (
    Id     INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Tallas (
    Id     INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE Proveedores (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Nombre   NVARCHAR(100) NOT NULL UNIQUE,
    Telefono NVARCHAR(30)  NOT NULL,
    Email    NVARCHAR(150) NOT NULL
);

CREATE TABLE Prendas (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL,
    Precio      DECIMAL(10,2) NOT NULL CHECK (Precio > 0),
    Existencias INT           NOT NULL CHECK (Existencias >= 0),
    Descuento   DECIMAL(5,2)  NOT NULL DEFAULT 0 CHECK (Descuento BETWEEN 0 AND 100),
    CategoriaId INT NOT NULL REFERENCES Categorias(Id),
    TallaId     INT NOT NULL REFERENCES Tallas(Id),
    ProveedorId INT NOT NULL REFERENCES Proveedores(Id),
    CONSTRAINT UQ_Prenda UNIQUE (Nombre, CategoriaId, TallaId)
);

-- PrendaId no es llave foránea a propósito: el historial de ventas
-- debe conservarse aunque después se elimine la prenda
CREATE TABLE Ventas (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    PrendaId       INT           NOT NULL,
    NombrePrenda   NVARCHAR(100) NOT NULL,
    Cantidad       INT           NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Fecha          DATETIME2     NOT NULL DEFAULT SYSDATETIME()
);
GO

-- =====================================================
-- DATOS INICIALES
-- =====================================================

INSERT INTO Categorias (Nombre) VALUES
    ('Camisas'), ('Pantalones'), ('Vestidos'), ('Faldas'), ('Zapatos');

INSERT INTO Tallas (Nombre) VALUES
    ('S'), ('M'), ('L'), ('XL'), ('36'), ('38'), ('40');

INSERT INTO Proveedores (Nombre, Telefono, Email) VALUES
    ('Moda Express', '809-555-0101', 'ventas@modaexpress.com');

INSERT INTO Prendas (Nombre, Precio, Existencias, Descuento, CategoriaId, TallaId, ProveedorId) VALUES
    ('Camisa de lino',     1500.00, 10,  0, 1, 2, 1),
    ('Sandalias de cuero', 2500.00,  5, 10, 5, 6, 1);
GO

-- =====================================================
-- COMPROBACIÓN: muestra las prendas con sus nombres
-- =====================================================

SELECT p.Id, p.Nombre, c.Nombre AS Categoria, t.Nombre AS Talla,
       pr.Nombre AS Proveedor, p.Precio, p.Descuento, p.Existencias
FROM Prendas p
JOIN Categorias  c  ON c.Id  = p.CategoriaId
JOIN Tallas      t  ON t.Id  = p.TallaId
JOIN Proveedores pr ON pr.Id = p.ProveedorId;
GO
