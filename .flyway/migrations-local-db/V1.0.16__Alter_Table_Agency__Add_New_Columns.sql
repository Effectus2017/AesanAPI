-- =============================================
-- Migración automática de procedimiento almacenado
-- Fecha: 2025-03-07 19:52:32
-- Archivo original: /Volumes/Mac/Proyectos/AESAN/Proyecto/Api/Database/StoredProcedures/Agency/103_AlterTableAgency_AddNewColumns.sql
-- =============================================

-- Script para agregar las nuevas columnas a la tabla Agency
-- Fecha: 2024-05-15
-- Descripción: Agrega las columnas ServiceTime, TaxExemptionStatus y TaxExemptionType a la tabla Agency

-- Verificar si las columnas ya existen antes de agregarlas
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Agency' AND COLUMN_NAME = 'ServiceTime')
BEGIN
    ALTER TABLE Agency
    ADD ServiceTime DATETIME NULL;
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Agency' AND COLUMN_NAME = 'TaxExemptionStatus')
BEGIN
    ALTER TABLE Agency
    ADD TaxExemptionStatus INT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Agency' AND COLUMN_NAME = 'TaxExemptionType')
BEGIN
    ALTER TABLE Agency
    ADD TaxExemptionType INT NULL DEFAULT 0;
END

-- Actualizar los registros existentes con valores predeterminados
UPDATE Agency
SET ServiceTime = GETDATE(),
    TaxExemptionStatus = 1,
    TaxExemptionType = 1
WHERE ServiceTime IS NULL;

GO 
GO
