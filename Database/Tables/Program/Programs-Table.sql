
-- Lista de programas disponibles para la inscripción e intentos de participación para las agencias

CREATE TABLE Program
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    -- nombre del programa
    Name NVARCHAR(255) NOT NULL,
    -- nombre del programa en inglés
    NameEN NVARCHAR(255) NOT NULL,
    -- descripción del programa
    Description NVARCHAR(MAX) NOT NULL,
    -- descripción del programa en inglés
    DescriptionEN NVARCHAR(MAX) NOT NULL,
    -- si el programa sigue activo
    IsActive BIT NOT NULL DEFAULT 1,
    -- fecha de creación del programa
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    -- fecha de modificación del programa
    UpdatedAt DATETIME NULL
);

INSERT INTO dbo.Program
    (Id, Name, NameEN, Description, DescriptionEN, IsActive, CreatedAt, UpdatedAt)
VALUES
    (1, 'PDAM', 'NSLBP', 'Programa de Desayuno Escolar', 'National School Lunch Program', 1, '2025-05-22 13:58:46.000', NULL),
    (2, 'PSAV', 'SFSP', 'Programa de Servicios de Alimentos de Verano', 'Summer Food Service Program', 1, '2025-05-22 13:58:46.000', NULL),
    (3, 'PACNA', 'CACFP', 'Programa de Alimentos para el Cuidado de Niños y Adulto', 'Child and Adult Care Food Program', 1, '2025-05-22 13:58:46.000', NULL),
    (4, 'PFHF', 'FFVP', 'Programa de Frutas y Hortalizas Frescas', 'Free and Reduced Price School Meals Program', 1, '2025-05-22 13:58:46.000', NULL),
    (5, 'PDFE', 'F2S', 'Programa de la Finca a la Escuela', 'Farm to School Program', 1, '2025-05-22 13:58:46.000', NULL),
    (6, 'AESAN', 'AESAN', 'Agencia de Servicios de Alimentos Nutritivos', 'Agency for Services of Nutritious Food', 1, '2025-05-22 13:58:46.000', NULL),
    (7, 'PAF', 'FFDP', 'Programa de Distribución de Alimentos Federales', 'Federal Foods Distribution Program', 0, '2025-05-22 13:58:46.000', NULL);