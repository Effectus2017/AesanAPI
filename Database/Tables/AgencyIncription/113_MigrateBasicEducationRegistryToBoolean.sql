-- Migración: Cambiar BasicEducationRegistryId (INT) a BasicEducationRegistry (BIT)
-- Versión: 1.1.3
-- Fecha: 2025-01-15
-- Descripción: Convierte el campo BasicEducationRegistryId de tipo INT con FK a OptionSelection
--              a un campo BIT siguiendo el patrón de otros campos booleanos como NonProfit, StateFundsDenied, etc.

-- ===========================================
-- MIGRACIÓN DE DATOS
-- ===========================================

-- 1. Agregar nueva columna BasicEducationRegistry como BIT
ALTER TABLE AgencyInscription 
ADD BasicEducationRegistry bit NULL DEFAULT (0);
GO

-- 2. Migrar datos existentes de BasicEducationRegistryId a BasicEducationRegistry
--    Lógica de migración:
--    - Si BasicEducationRegistryId = 4 (Otorgado) → BasicEducationRegistry = 1 (Sí)
--    - Si BasicEducationRegistryId = 3 (En Proceso) → BasicEducationRegistry = 1 (Sí)  
--    - Si BasicEducationRegistryId = 5 (No) → BasicEducationRegistry = 0 (No)
--    - Si BasicEducationRegistryId es NULL → BasicEducationRegistry = 0 (No)
UPDATE AgencyInscription 
SET BasicEducationRegistry = CASE 
    WHEN BasicEducationRegistryId = 4 THEN 1  -- Otorgado = Sí
    WHEN BasicEducationRegistryId = 3 THEN 1  -- En Proceso = Sí (consideramos que tiene el proceso iniciado)
    WHEN BasicEducationRegistryId = 5 THEN 0  -- No = No
    ELSE 0  -- NULL o cualquier otro valor = No
END;
GO

-- 3. Verificar migración de datos
--    Mostrar conteo de registros migrados
SELECT
    'Registros migrados' as Descripcion,
    COUNT(*) as TotalRegistros,
    SUM(CASE WHEN BasicEducationRegistry = 1 THEN 1 ELSE 0 END) as ConCertificacion,
    SUM(CASE WHEN BasicEducationRegistry = 0 THEN 1 ELSE 0 END) as SinCertificacion
FROM AgencyInscription;
GO

-- ===========================================
-- ELIMINACIÓN DE ESTRUCTURA ANTIGUA
-- ===========================================

-- 4. Eliminar foreign key constraint
ALTER TABLE AgencyInscription 
DROP CONSTRAINT FK_AgencyInscription_BasicEducationRegistry;
GO

-- 5. Eliminar default constraint de BasicEducationRegistryId (si existe)
-- Primero verificamos si existe el constraint
IF EXISTS (SELECT *
FROM sys.default_constraints
WHERE name = 'DF__AgencyIns__Basic__7849DB76')
BEGIN
    ALTER TABLE AgencyInscription 
    DROP CONSTRAINT DF__AgencyIns__Basic__7849DB76;
    PRINT 'Default constraint DF__AgencyIns__Basic__7849DB76 eliminado';
END
ELSE
BEGIN
    -- Si el constraint tiene otro nombre, lo buscamos dinámicamente
    DECLARE @ConstraintName NVARCHAR(128);
    SELECT @ConstraintName = name
    FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('AgencyInscription')
        AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('AgencyInscription'), 'BasicEducationRegistryId', 'ColumnId');

    IF @ConstraintName IS NOT NULL
    BEGIN
        DECLARE @SQL NVARCHAR(MAX) = 'ALTER TABLE AgencyInscription DROP CONSTRAINT ' + @ConstraintName;
        EXEC sp_executesql @SQL;
        PRINT 'Default constraint ' + @ConstraintName + ' eliminado';
    END
    ELSE
    BEGIN
        PRINT 'No se encontró default constraint para BasicEducationRegistryId';
    END
END
GO

-- 6. Eliminar columna antigua BasicEducationRegistryId
ALTER TABLE AgencyInscription 
DROP COLUMN BasicEducationRegistryId;
GO

-- ===========================================
-- VERIFICACIÓN FINAL
-- ===========================================

-- 7. Verificar que la nueva columna existe y tiene los datos correctos
SELECT
    'Verificación final' as Descripcion,
    COUNT(*) as TotalRegistros,
    SUM(CASE WHEN BasicEducationRegistry = 1 THEN 1 ELSE 0 END) as ConCertificacion,
    SUM(CASE WHEN BasicEducationRegistry = 0 THEN 1 ELSE 0 END) as SinCertificacion,
    SUM(CASE WHEN BasicEducationRegistry IS NULL THEN 1 ELSE 0 END) as ValoresNulos
FROM AgencyInscription;
GO

-- 8. Mostrar estructura de la tabla para verificar cambios
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AgencyInscription'
    AND COLUMN_NAME IN ('BasicEducationRegistry', 'BasicEducationRegistryId')
ORDER BY COLUMN_NAME;
GO

-- ===========================================
-- COMENTARIOS DE DOCUMENTACIÓN
-- ===========================================

-- La migración convierte el campo de un sistema de 3 estados (En Proceso, Otorgado, No)
-- a un sistema binario (Sí, No) donde:
-- - "Sí" incluye tanto "En Proceso" como "Otorgado" (agencia tiene o está en proceso de obtener la certificación)
-- - "No" corresponde al estado anterior "No"

-- Este cambio simplifica la lógica de negocio y sigue el patrón establecido
-- por otros campos booleanos en la tabla AgencyInscription.

PRINT 'Migración completada: BasicEducationRegistryId convertido a BasicEducationRegistry (BIT)';
GO
