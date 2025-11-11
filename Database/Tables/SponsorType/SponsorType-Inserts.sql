-- Insertar todos los valores de SponsorType
-- Eliminar todos los valores actuales y recrear según especificación

TRUNCATE TABLE SponsorType;

-- Gobierno (para PDAM)
INSERT INTO SponsorType
    (Name, NameEN, IsActive, DisplayOrder, SelectionNotification)
VALUES
    ('Gobierno', 'Government', 1, 10, 0);

-- Privado (para PDAM y PACNA)
INSERT INTO SponsorType
    (Name, NameEN, IsActive, DisplayOrder, SelectionNotification)
VALUES
    ('Privado', 'Private', 1, 20, 0);

-- Público Estatal (para PACNA)
INSERT INTO SponsorType
    (Name, NameEN, IsActive, DisplayOrder, SelectionNotification)
VALUES
    ('Público Estatal', 'State Public', 1, 30, 0);

-- Público Federal (para PACNA)
INSERT INTO SponsorType
    (Name, NameEN, IsActive, DisplayOrder, SelectionNotification)
VALUES
    ('Público Federal', 'Federal Public', 1, 40, 0);
