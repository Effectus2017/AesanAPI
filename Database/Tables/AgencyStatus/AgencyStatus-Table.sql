
-- Estados de la agencia

-- Aprobado
-- Rechazado
-- etc

CREATE TABLE AgencyStatus
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL, -- nombre del estado de la agencia
    NameEN NVARCHAR(255) NOT NULL, -- nombre del estado de la agencia en inglés
    IsActive BIT NOT NULL DEFAULT 1, -- si el estado sigue activo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- fecha de creación del estado de la agencia
    UpdatedAt DATETIME NULL, -- fecha de modificación del estado de la agencia
    DisplayOrder INT NOT NULL DEFAULT 0 -- orden de visualización del estado de la agencia
);

-- Modificación de la tabla AgencyStatus para agregar la columna NameEN
ALTER TABLE AgencyStatus
ADD NameEN NVARCHAR(255) NOT NULL;