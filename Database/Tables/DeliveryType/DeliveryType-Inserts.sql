-- Insertar todos los valores de DeliveryType
-- Recrear todos los valores desde cero según especificación

TRUNCATE TABLE DeliveryType;

-- N/A
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('N/A', 'N/A', 0, 0);

-- Sitio Recoge (nuevo para PDAM)
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Sitio Recoge', 'Pickup Site', 10, 0);

-- Auspiciador Entrega
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Auspiciador Entrega', 'Sponsor Delivery', 20, 0);

-- Centro Comunal
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Centro Comunal', 'Community Center', 30, 0);

-- Domicilio
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Domicilio', 'Home Delivery', 40, 0);

-- Recogido de Padre o Encargado (para PDAM y PACNA)
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Recogido de Padre o Encargado', 'Parents Pick up', 50, 0);

-- Recogido (solo para PSAV)
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Recogido', 'Pick up', 55, 0);

-- Servi-Carro
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Servi-Carro', 'Car Service', 60, 0);

-- Servi-Expreso
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder, SelectionNotification)
VALUES
    ('Servi-Expreso', 'Express Service', 70, 0);

