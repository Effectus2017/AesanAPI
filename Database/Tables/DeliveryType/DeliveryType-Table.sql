CREATE TABLE DeliveryType
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    NameEN NVARCHAR(255) NOT NULL DEFAULT '',
    IsActive BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL DEFAULT 0,
    SelectionNotification BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Inserts iniciales
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('N/A', 'N/A', 0, 0);
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Sitio Recoge', 'Pickup Site', 10, 0);
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Servi Expreso', 'Express Service', 20, 0);
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Domicilio', 'Home Delivery', 30, 0);
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Centro Comunal', 'Community Center', 40, 0);
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Auspiciador Entrega', 'Sponsor Delivery', 50, 0);

INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Servi Carro', 'Car Service', 60, 0);