-- =============================================
-- Script: InsertStaffPositions
-- =============================================
-- Inserta los puestos y títulos en OptionSelection según la clasificación de staff
-- Este script debe ejecutarse después de crear las clasificaciones de staff

-- Puestos para Empleados Administrativos (optionKey = 'administrativePosition')
INSERT INTO OptionSelection
    (OptionKey, Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('administrativePosition', 'Administrador', 'Administrator', 350, 1, GETDATE()),
    ('administrativePosition', 'Auxiliar Administrativo', 'Administrative Assistant', 360, 1, GETDATE()),
    ('administrativePosition', 'Contable', 'Accountant', 370, 1, GETDATE()),
    ('administrativePosition', 'Director', 'Director', 380, 1, GETDATE());

-- Puestos para Empleados Operacionales (optionKey = 'operationalPosition')
INSERT INTO OptionSelection
    (OptionKey, Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('operationalPosition', 'Ayudante de Cocina', 'Kitchen Assistant', 400, 1, GETDATE()),
    ('operationalPosition', 'Cocinero(a)', 'Cook', 410, 1, GETDATE()),
    ('operationalPosition', 'Chef', 'Chef', 420, 1, GETDATE()),
    ('operationalPosition', 'Chofer', 'Driver', 430, 1, GETDATE()),
    ('operationalPosition', 'Encargado(a) de Cocina', 'Kitchen Manager', 440, 1, GETDATE());

-- Títulos para Miembros de Junta (optionKey = 'boardMemberTitle')
INSERT INTO OptionSelection
    (OptionKey, Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('boardMemberTitle', 'Ayudante', 'Assistant', 500, 1, GETDATE()),
    ('boardMemberTitle', 'Presidente', 'President', 510, 1, GETDATE()),
    ('boardMemberTitle', 'Secretario(a)', 'Secretary', 520, 1, GETDATE()),
    ('boardMemberTitle', 'Tesorero', 'Treasurer', 530, 1, GETDATE());

-- Verificar que se insertaron correctamente todas las optionKey
SELECT
    OptionKey,
    Name,
    NameEn,
    SortOrder
FROM OptionSelection
WHERE OptionKey IN ('administrativePosition', 'operationalPosition', 'boardMemberTitle')
ORDER BY OptionKey, SortOrder; 