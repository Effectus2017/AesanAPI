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

-- vamos a agregar nuevos puestos para los administrativos, que el DisplayOrder sea consecutivo a los anteriores pero sumando solo uno en cada uno
-- los valores nuevos son Contador, Coordinador(a) del Programa, Nutricionista, Secretaria, Supervisor(a)
INSERT INTO OptionSelection
    (OptionKey, Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('administrativePosition', 'Contador', 'Accountant', 391, 1, GETDATE()),
    ('administrativePosition', 'Coordinador(a) del Programa', 'Program Coordinator', 401, 1, GETDATE()),
    ('administrativePosition', 'Nutricionista', 'Nutritionist', 411, 1, GETDATE()),
    ('administrativePosition', 'Secretaria', 'Secretary', 421, 1, GETDATE()),
    ('administrativePosition', 'Supervisor(a)', 'Supervisor', 431, 1, GETDATE()),
    ('administrativePosition', 'Asistente de Marketing', 'Marketing Assistant', 441, 1, GETDATE()),
    ('administrativePosition', 'Asistente de Finanzas', 'Finance Assistant', 451, 1, GETDATE());

-- Puestos para Empleados Operacionales (optionKey = 'operationalPosition')
INSERT INTO OptionSelection
    (OptionKey, Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('operationalPosition', 'Ayudante de Cocina', 'Kitchen Assistant', 400, 1, GETDATE()),
    ('operationalPosition', 'Cocinero(a)', 'Cook', 410, 1, GETDATE()),
    ('operationalPosition', 'Chef', 'Chef', 420, 1, GETDATE()),
    ('operationalPosition', 'Chofer', 'Driver', 430, 1, GETDATE()),
    ('operationalPosition', 'Encargado(a) de Cocina', 'Kitchen Manager', 440, 1, GETDATE());

-- vamos a agregar nuevos puestos para los Operacional, que el DisplayOrder sea consecutivo a los anteriores pero sumando solo uno en cada uno
-- los valores nuevos son Asistente de Cocina, Empacador(a), Limpieza

INSERT INTO OptionSelection
    (OptionKey, Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('operationalPosition', 'Asistente de Cocina', 'Kitchen Assistant', 451, 1, GETDATE()),
    ('operationalPosition', 'Empacador(a)', 'Packer', 461, 1, GETDATE()),
    ('operationalPosition', 'Limpieza', 'Cleaning', 471, 1, GETDATE());

-- Títulos para Miembros de Junta (optionKey = 'boardMemberTitle')
INSERT INTO OptionSelection
    (OptionKey, Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('boardMemberTitle', 'Ayudante', 'Assistant', 500, 1, GETDATE()),
    ('boardMemberTitle', 'Presidente', 'President', 510, 1, GETDATE()),
    ('boardMemberTitle', 'Secretario(a)', 'Secretary', 520, 1, GETDATE()),
    ('boardMemberTitle', 'Tesorero', 'Treasurer', 530, 1, GETDATE()),
    ('boardMemberTitle', 'Vocal', 'Vocal', 540, 1, GETDATE());

-- Vocal
INSERT INTO OptionSelection
    (OptionKey, Name, NameEn, DisplayOrder, IsActive, CreatedAt)
VALUES
    ('boardMemberTitle', 'Vocal', 'Vocal', 540, 1, GETDATE());

-- Verificar que se insertaron correctamente todas las optionKey
SELECT
    OptionKey,
    Name,
    NameEn,
    SortOrder
FROM OptionSelection
WHERE OptionKey IN ('administrativePosition', 'operationalPosition', 'boardMemberTitle')
ORDER BY OptionKey, SortOrder; 