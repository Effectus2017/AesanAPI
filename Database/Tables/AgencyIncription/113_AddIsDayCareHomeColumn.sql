-- Script de migración para agregar la columna IsDayCareHome
-- Migration script to add IsDayCareHome column
-- 1.1.3

-- Agregar la columna IsDayCareHome a la tabla AgencyInscription existente
-- Add IsDayCareHome column to existing AgencyInscription table
IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AgencyInscription' AND COLUMN_NAME = 'IsDayCareHome')
BEGIN
    ALTER TABLE AgencyInscription
    ADD IsDayCareHome bit NULL DEFAULT (0);

    PRINT 'Columna IsDayCareHome agregada exitosamente a la tabla AgencyInscription';
END
ELSE
BEGIN
    PRINT 'La columna IsDayCareHome ya existe en la tabla AgencyInscription';
END
GO

-- Actualizar todos los registros existentes para establecer el valor por defecto
-- Update all existing records to set default value
UPDATE AgencyInscription
SET IsDayCareHome = 0
WHERE IsDayCareHome IS NULL;
GO

PRINT 'Migración completada: Campo IsDayCareHome agregado a AgencyInscription';
GO
