-- Insertar estados de la agencia
INSERT INTO AgencyStatus
    (Name, IsActive, CreatedAt)
VALUES
    ('Pendiente a validar', 1, GETDATE()),           -- 1 (default cuando cae en el listado)
    ('Coordinar Visita Pre-Operacional', 1, GETDATE()), -- 2
    ('Orientación de Programa', 1, GETDATE()),       -- 3
    ('Orientación de Contabilidad', 1, GETDATE()),   -- 4
    ('Cumple con los requisitos', 1, GETDATE()),     -- 5
    ('No cumple con los requisitos', 1, GETDATE());  -- 6

-- Insertar programas
INSERT INTO Program
    (Name, Description, IsActive, CreatedAt)
VALUES
    ('PDAM', 'Programa de Desayuno Escolar', 1, GETDATE()),
    ('PSAV', 'Programa de Servicios de Alimentos de Verano', 1, GETDATE()),
    ('PACNA', 'Programa de Alimentos para el Cuidado de Niños y Adulto', 1, GETDATE()),
    ('PFHF', 'Programa de Frutas y Hortalizas Frescas', 1, GETDATE()),
    --('PAF', 'Programa de Distribución de Alimentos Federales', 0, GETDATE()),
    ('PDFE', 'Programa de la Finca a la Escuela', 1, GETDATE()),
    ('AESAN', 'Agencia de Servicios de Alimentos Nutritivos', 1, GETDATE());

-- Insertar programas de usuario
INSERT INTO UserProgram
    (UserId, ProgramId, IsActive, CreatedAt)
VALUES
    ('87654321-4321-4321-4321-210987654321', 1, 1, GETDATE());