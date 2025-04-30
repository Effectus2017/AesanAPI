
-- Lista de programas disponibles para la inscripción e intentos de participación para las agencias

CREATE TABLE Program
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL, -- nombre del programa
    Description NVARCHAR(MAX) NOT NULL, -- descripción del programa
    DescriptionEN NVARCHAR(MAX) NOT NULL, -- descripción del programa en inglés
    IsActive BIT NOT NULL DEFAULT 1, -- si el programa sigue activo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- fecha de creación del programa
    UpdatedAt DATETIME NULL -- fecha de modificación del programa
);

-- Modificación de la tabla Program para agregar la columna DescriptionEN
ALTER TABLE Program
ADD DescriptionEN NVARCHAR(MAX) NOT NULL;