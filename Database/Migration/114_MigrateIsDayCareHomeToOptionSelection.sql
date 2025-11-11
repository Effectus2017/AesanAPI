-- ============================================
-- Script de Actualización/Migración: IsDayCareHome de bit a IsDayCareHomeId (int)
-- Versión: 1.0
-- Fecha: 2025-03-14
-- Descripción: Migra el campo IsDayCareHome de bit a IsDayCareHomeId (int) para soportar opciones adicionales (Sí, No, Ambos)
-- ============================================

-- IMPORTANTE: Ejecutar este script en el siguiente orden:
-- 1. Primero asegurarse de que las opciones existen en OptionSelection (ver OptionSelection-Table.sql)
-- 2. Luego ejecutar este script completo

PRINT '============================================';
PRINT 'Iniciando migración de IsDayCareHome a IsDayCareHomeId';
PRINT '============================================';
GO

-- ============================================
-- PASO 1: Verificar que las opciones existen en OptionSelection
-- ============================================
IF NOT EXISTS (SELECT 1
FROM OptionSelection
WHERE OptionKey = 'isDayCareHome')
BEGIN
    PRINT 'ERROR: Las opciones para isDayCareHome no existen en OptionSelection.';
    PRINT 'Por favor, ejecutar primero el INSERT en OptionSelection-Table.sql';
    PRINT 'INSERT INTO OptionSelection (Name, NameEN, OptionKey, BooleanValue, IsActive, DisplayOrder)';
    PRINT 'VALUES';
    PRINT '    (''No'', ''No'', ''isDayCareHome'', 0, 1, 950),';
    PRINT '    (''Sí'', ''Yes'', ''isDayCareHome'', 1, 1, 960),';
    PRINT '    (''Ambos'', ''Both'', ''isDayCareHome'', NULL, 1, 970);';
    RETURN;
END
ELSE
BEGIN
    PRINT '✓ Opciones de isDayCareHome verificadas en OptionSelection';
END
GO

-- ============================================
-- PASO 2: Agregar nueva columna IsDayCareHomeId como int nullable
-- ============================================
IF COL_LENGTH('AgencyInscription', 'IsDayCareHomeId') IS NULL
BEGIN
    ALTER TABLE AgencyInscription
    ADD IsDayCareHomeId INT NULL;
    PRINT '✓ Columna IsDayCareHomeId agregada exitosamente.';
END
ELSE
BEGIN
    PRINT '⚠ La columna IsDayCareHomeId ya existe. Continuando con la migración...';
END
GO

-- ============================================
-- PASO 3: Migrar datos existentes de IsDayCareHome (bit) a IsDayCareHomeId (int)
-- ============================================
PRINT 'Migrando datos existentes...';
GO

-- Migrar los datos directamente usando subconsultas
UPDATE ai
SET IsDayCareHomeId = CASE 
    WHEN ai.IsDayCareHome = 1 THEN (SELECT Id
FROM OptionSelection
WHERE OptionKey = 'isDayCareHome' AND BooleanValue = 1 AND Name = 'Sí')
    WHEN ai.IsDayCareHome = 0 THEN (SELECT Id
FROM OptionSelection
WHERE OptionKey = 'isDayCareHome' AND BooleanValue = 0 AND Name = 'No')
    ELSE NULL
END
FROM AgencyInscription ai
WHERE ai.IsDayCareHomeId IS NULL;

DECLARE @RowsAffected INT = @@ROWCOUNT;
PRINT '✓ ' + CAST(@RowsAffected AS VARCHAR) + ' registros migrados exitosamente.';
GO

-- ============================================
-- PASO 4: Verificar la migración
-- ============================================
PRINT 'Verificando migración...';
GO

-- Resumen de la migración
SELECT
    COUNT(*) as TotalRegistros,
    SUM(CASE WHEN IsDayCareHome = 1 THEN 1 ELSE 0 END) as RegistrosConSí_Original,
    SUM(CASE WHEN IsDayCareHome = 0 THEN 1 ELSE 0 END) as RegistrosConNo_Original,
    SUM(CASE WHEN IsDayCareHome IS NULL THEN 1 ELSE 0 END) as RegistrosNULL_Original,
    SUM(CASE WHEN IsDayCareHomeId IS NOT NULL THEN 1 ELSE 0 END) as RegistrosMigrados
FROM AgencyInscription;
GO

-- Verificar que los valores migrados sean correctos
PRINT 'Muestra de datos migrados (primeros 10 registros):';
SELECT TOP 10
    ai.Id,
    ai.IsDayCareHome as ValorOriginal,
    ai.IsDayCareHomeId as ValorMigrado,
    os.Name as NombreOpcion,
    os.NameEN as NombreOpcionEN,
    CASE 
        WHEN ai.IsDayCareHome = 1 AND os.Name = 'Sí' THEN '✓ Correcto'
        WHEN ai.IsDayCareHome = 0 AND os.Name = 'No' THEN '✓ Correcto'
        WHEN ai.IsDayCareHome IS NULL AND ai.IsDayCareHomeId IS NULL THEN '✓ Correcto'
        ELSE '⚠ Verificar'
    END as Estado
FROM AgencyInscription ai
    LEFT JOIN OptionSelection os ON ai.IsDayCareHomeId = os.Id
ORDER BY ai.Id;
GO

-- ============================================
-- PASO 5: Agregar foreign key constraint
-- ============================================
IF NOT EXISTS (
    SELECT *
FROM sys.foreign_keys
WHERE name = 'FK_AgencyInscription_IsDayCareHomeId'
)
BEGIN
    ALTER TABLE AgencyInscription
    ADD CONSTRAINT FK_AgencyInscription_IsDayCareHomeId 
    FOREIGN KEY (IsDayCareHomeId) REFERENCES OptionSelection(Id);
    PRINT '✓ Foreign key FK_AgencyInscription_IsDayCareHomeId agregada exitosamente.';
END
ELSE
BEGIN
    PRINT '⚠ La foreign key FK_AgencyInscription_IsDayCareHomeId ya existe.';
END
GO

-- ============================================
-- PASO 6: Validación final
-- ============================================
PRINT '============================================';
PRINT 'Validación final:';
PRINT '============================================';
GO

-- Contar registros que no se migraron correctamente
SELECT
    COUNT(*) as RegistrosSinMigrar,
    CASE 
        WHEN COUNT(*) = 0 THEN '✓ Migración exitosa: Todos los registros fueron migrados correctamente'
        ELSE '⚠ ADVERTENCIA: ' + CAST(COUNT(*) AS VARCHAR) + ' registros no fueron migrados correctamente'
    END as ResultadoMigracion
FROM AgencyInscription
WHERE IsDayCareHome IS NOT NULL
    AND IsDayCareHomeId IS NULL;
GO

PRINT '============================================';
PRINT 'Migración completada.';
PRINT 'NOTA: La columna IsDayCareHome se mantendrá temporalmente hasta validar que todo funciona correctamente.';
PRINT 'Una vez validado, ejecutar el script de limpieza para eliminar la columna antigua.';
PRINT '============================================';
GO

-- ============================================
-- PASO 7: (OPCIONAL) Eliminar columna antigua después de validar
-- ============================================
-- DESCOMENTAR SOLO DESPUÉS DE VALIDAR QUE TODO FUNCIONA CORRECTAMENTE EN PRODUCCIÓN
-- 
-- PRINT 'Eliminando columna antigua IsDayCareHome...';
-- GO
-- 
-- -- Primero eliminar la foreign key si existe (aunque no debería tener una)
-- -- Luego eliminar la columna
-- ALTER TABLE AgencyInscription
-- DROP COLUMN IsDayCareHome;
-- GO
-- 
-- PRINT '✓ Columna IsDayCareHome eliminada exitosamente.';
-- GO
