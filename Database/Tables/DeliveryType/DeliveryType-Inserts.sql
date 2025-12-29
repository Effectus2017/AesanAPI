-- Insertar todos los valores de DeliveryType
-- Recrear todos los valores desde cero según especificación

TRUNCATE TABLE DeliveryType;

-- N/A
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('N/A', 'N/A', 0);

-- Sitio Recoge (nuevo para PDAM)
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Sitio Recoge', 'Pickup Site', 10);

-- Auspiciador Entrega
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Auspiciador Entrega', 'Sponsor Delivery', 20);

-- Centro Comunal
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Centro Comunal', 'Community Center', 30);

-- Domicilio
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Domicilio', 'Home Delivery', 40);

-- Recogido de Padre o Encargado (para PDAM y PACNA)
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Recogido de Padre o Encargado', 'Parents Pick up', 50);

-- Recogido (solo para PSAV)
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Recogido', 'Pick up', 55);

-- Servi-Carro
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Servi-Carro', 'Car Service', 60);

-- Servi-Expreso
INSERT INTO DeliveryType
    (Name, NameEN, DisplayOrder)
VALUES
    ('Servi-Expreso', 'Express Service', 70);

