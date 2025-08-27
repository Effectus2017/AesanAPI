-- =============================================
-- Script: InsertStaffRelationshipTypes
-- =============================================
-- Inserta los tipos de parentesco en OptionSelection
-- Este script debe ejecutarse después de crear la tabla StaffRelationship

-- Tipos de parentesco entre empleados (optionKey = 'staffRelationshipType')
INSERT INTO OptionSelection
    (Name, NameEn, OptionKey, BooleanValue, IsActive, DisplayOrder)
VALUES
    ('Esposo(a)', 'Spouse', 'staffRelationshipType', 0, 1, 800),
    ('Hijo(a)', 'Child', 'staffRelationshipType', 0, 1, 810),
    ('Suegro(a)', 'Father/Mother-in-law', 'staffRelationshipType', 0, 1, 820),
    ('Padre', 'Father', 'staffRelationshipType', 0, 1, 830),
    ('Madre', 'Mother', 'staffRelationshipType', 0, 1, 840),
    ('Hermano(a)', 'Sibling', 'staffRelationshipType', 0, 1, 850),
    ('Cuñado(a)', 'Brother/Sister-in-law', 'staffRelationshipType', 0, 1, 860);

-- Verificar que se insertaron correctamente
SELECT
    OptionKey,
    Name,
    NameEn,
    DisplayOrder
FROM OptionSelection
WHERE OptionKey = 'staffRelationshipType'
ORDER BY DisplayOrder;
