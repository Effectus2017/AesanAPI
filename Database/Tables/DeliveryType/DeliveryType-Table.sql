CREATE TABLE [dbo].[DeliveryType]
(
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [NameEN] NVARCHAR(255) NOT NULL DEFAULT '',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NULL
);

-- Inserts iniciales
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('N/A', 'N/A', 0);
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Sitio Recoge', 'Pickup Site', 10);
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Servi Expreso/Carro', 'Express Service/Car', 20);
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Domicilio', 'Home Delivery', 30);
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Centro Comunal', 'Community Center', 40);
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Auspiciador Entrega', 'Sponsor Delivery', 50);