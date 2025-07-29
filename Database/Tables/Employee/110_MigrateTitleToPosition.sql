-- =============================================
-- Script de Migración: TitleId → PositionId
-- =============================================
-- Este script migra la columna TitleId a PositionId en la tabla Employee
-- y actualiza las referencias en OptionSelection

-- Paso 1: Agregar la nueva columna PositionId
IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Employee' AND COLUMN_NAME = 'PositionId')
BEGIN
    ALTER TABLE Employee ADD PositionId INT NULL;
    PRINT 'Columna PositionId agregada a la tabla Employee';
END

-- Paso 2: Copiar datos de TitleId a PositionId
UPDATE Employee 
SET PositionId = TitleId 
WHERE PositionId IS NULL AND TitleId IS NOT NULL;
PRINT 'Datos copiados de TitleId a PositionId';

-- Paso 3: Actualizar OptionSelection para cambiar employeeTitle por employeePosition
UPDATE OptionSelection 
SET OptionKey = 'employeePosition' 
WHERE OptionKey = 'employeeTitle';
PRINT 'OptionKey actualizado de employeeTitle a employeePosition';

-- Paso 4: Hacer PositionId NOT NULL después de migrar los datos
IF EXISTS (SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Employee' AND COLUMN_NAME = 'PositionId' AND IS_NULLABLE = 'YES')
BEGIN
    ALTER TABLE Employee ALTER COLUMN PositionId INT NOT NULL;
    PRINT 'Columna PositionId marcada como NOT NULL';
END

-- Paso 5: Agregar valor por defecto a PositionId
IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Employee' AND COLUMN_NAME = 'PositionId' AND COLUMN_DEFAULT IS NOT NULL)
BEGIN
    ALTER TABLE Employee ADD CONSTRAINT DF_Employee_PositionId DEFAULT 0 FOR PositionId;
    PRINT 'Valor por defecto agregado a PositionId';
END

-- Paso 6: Agregar Foreign Key constraint si no existe
IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
WHERE CONSTRAINT_NAME = 'FK_Employee_PositionId_OptionSelection')
BEGIN
    ALTER TABLE Employee ADD CONSTRAINT FK_Employee_PositionId_OptionSelection 
    FOREIGN KEY (PositionId) REFERENCES OptionSelection(Id);
    PRINT 'Foreign Key constraint agregado para PositionId';
END

-- Paso 7: Crear índice si no existe
IF NOT EXISTS (SELECT *
FROM sys.indexes
WHERE name = 'IX_Employee_PositionId')
BEGIN
    CREATE INDEX IX_Employee_PositionId ON Employee(PositionId);
    PRINT 'Índice IX_Employee_PositionId creado';
END

-- Paso 8: Eliminar la columna TitleId (OPCIONAL - descomentar cuando esté seguro)
-- ALTER TABLE Employee DROP CONSTRAINT FK_Employee_TitleId_OptionSelection;
-- DROP INDEX IX_Employee_TitleId ON Employee;
-- ALTER TABLE Employee DROP COLUMN TitleId;
-- PRINT 'Columna TitleId eliminada';

PRINT 'Migración completada exitosamente'; 