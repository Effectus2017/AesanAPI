-- Insertar valores de Site Location en OptionSelection
-- Site Location: Mobile (para Servicio en Camiones) o Fixed (para todos los demás)
-- Versión: 1.0
-- Fecha: 2025-01-15

INSERT INTO OptionSelection
    (Name, NameEN, OptionKey, IsActive, DisplayOrder)
VALUES
    ('Fijo', 'Fixed', 'siteLocation', 1, 1),
    ('Móvil', 'Mobile', 'siteLocation', 1, 2);
