CREATE TABLE [dbo].[KitchenType]
(
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [NameEN] NVARCHAR(255) NOT NULL DEFAULT '',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NULL
);

-- Modificación de la tabla KitchenType para agregar la columna NameEN y DisplayOrder si no existen
IF COL_LENGTH('KitchenType', 'NameEN') IS NULL
BEGIN
    ALTER TABLE KitchenType ADD NameEN NVARCHAR(255) NOT NULL DEFAULT '';
END
IF COL_LENGTH('KitchenType', 'DisplayOrder') IS NULL
BEGIN
    ALTER TABLE KitchenType ADD DisplayOrder INT NOT NULL DEFAULT 0;
END


-- N/A
-- (CC) Cocina Central-Solo Satélites
-- (CCC) Cocina Central Combinada- Grupos en Comedor y Satélites
-- (PSS) Preparadas y Servidas en el Sitio- Grupos solo en Comedor
-- Empresa de Gestión de Alimentos (“Food Management Company”)

INSERT INTO KitchenType
    (Name, NameEN, DisplayOrder)
VALUES
    ('N/A', 'N/A', 0);
INSERT INTO KitchenType
    (Name, NameEN, DisplayOrder)
VALUES
    ('(CC) Cocina Central-Solo Satélites', '(CC) Central Kitchen-Only Satellites', 10);
INSERT INTO KitchenType
    (Name, NameEN, DisplayOrder)
VALUES
    ('(CCC) Cocina Central Combinada- Grupos en Comedor y Satélites', '(CCC) Central Combined Kitchen- Groups in Dining Room and Satellites', 20);
INSERT INTO KitchenType
    (Name, NameEN, DisplayOrder)
VALUES
    ('(PSS) Preparadas y Servidas en el Sitio- Grupos solo en Comedor', '(PSS) Prepared and Served on Site- Groups only in Dining Room', 30);
INSERT INTO KitchenType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Empresa de Gestión de Alimentos (“Food Management Company”)', 'Food Management Company', 40);