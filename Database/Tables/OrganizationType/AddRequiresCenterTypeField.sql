-- Script para agregar el campo RequiresCenterType a la tabla OrganizationType
-- Fecha: 2025-01-15
-- Descripción: Agrega campo booleano para indicar si el tipo de organización requiere selección de tipo de centro

-- Agregar el campo RequiresCenterType si no existe
IF COL_LENGTH('OrganizationType', 'RequiresCenterType') IS NULL
BEGIN
    ALTER TABLE OrganizationType ADD RequiresCenterType BIT NOT NULL DEFAULT 0;
    PRINT 'Campo RequiresCenterType agregado exitosamente a la tabla OrganizationType';
END
ELSE
BEGIN
    PRINT 'El campo RequiresCenterType ya existe en la tabla OrganizationType';
END

-- Actualizar los registros existentes
-- Marcar "Institución Residencial" como que requiere tipo de centro
UPDATE OrganizationType 
SET RequiresCenterType = 1 
WHERE NameEN = 'Residential Institution' OR Name = 'Institución Residencial';

-- Verificar la actualización
SELECT 
    Id,
    Name,
    NameEN,
    DisplayOrder,
    RequiresCenterType,
    IsActive
FROM OrganizationType
ORDER BY DisplayOrder;
