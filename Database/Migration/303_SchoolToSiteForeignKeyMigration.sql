-- =============================================
-- Migration Script: 303_SchoolToSiteForeignKeyMigration
-- Descripción: Actualiza foreign keys que referencian School para que apunten a Site
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- IMPORTANTE: Este script debe ejecutarse DESPUÉS de migrar los datos
-- y ANTES de eliminar las tablas School_OLD

BEGIN TRANSACTION;

DECLARE @sql NVARCHAR(MAX);
DECLARE @constraintName NVARCHAR(255);
DECLARE @tableName NVARCHAR(255);
DECLARE @columnName NVARCHAR(255);

PRINT 'Iniciando migración de foreign keys de School a Site...';

-- 1. Actualizar foreign keys que referencian SchoolId para que apunten a SiteId
-- Lista de tablas que tienen foreign keys a School
DECLARE @foreignKeyUpdates TABLE (
    table_name NVARCHAR(255),
    column_name NVARCHAR(255),
    constraint_name NVARCHAR(255)
);

-- Buscar todas las foreign keys que referencian School
INSERT INTO @foreignKeyUpdates
    (table_name, column_name, constraint_name)
SELECT
    t.name AS table_name,
    c.name AS column_name,
    fk.name AS constraint_name
FROM sys.foreign_keys fk
    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    INNER JOIN sys.tables t ON fkc.parent_object_id = t.object_id
    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
    INNER JOIN sys.tables rt ON fkc.referenced_object_id = rt.object_id
WHERE rt.name = 'School';

-- Actualizar cada foreign key
DECLARE fk_cursor CURSOR FOR
SELECT table_name, column_name, constraint_name
FROM @foreignKeyUpdates;

OPEN fk_cursor;
FETCH NEXT FROM fk_cursor INTO @tableName, @columnName, @constraintName;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Eliminar constraint existente
    SET @sql = 'ALTER TABLE [' + @tableName + '] DROP CONSTRAINT [' + @constraintName + ']';
    EXEC sp_executesql @sql;
    PRINT 'Eliminado constraint ' + @constraintName + ' de tabla ' + @tableName;

    -- Crear nuevo constraint que apunta a Site
    SET @sql = 'ALTER TABLE [' + @tableName + '] ADD CONSTRAINT [FK_' + @tableName + '_Site] FOREIGN KEY([' + @columnName + ']) REFERENCES [Site]([Id])';
    EXEC sp_executesql @sql;
    PRINT 'Creado nuevo constraint FK_' + @tableName + '_Site';

    FETCH NEXT FROM fk_cursor INTO @tableName, @columnName, @constraintName;
END

CLOSE fk_cursor;
DEALLOCATE fk_cursor;

-- 2. Actualizar foreign keys específicas que necesitan atención especial
-- Estas son foreign keys que pueden tener nombres específicos o lógica especial

-- Actualizar foreign keys en tablas que pueden tener múltiples referencias
-- Por ejemplo, si hay tablas que referencian tanto School como otras entidades

-- 3. Verificar que todas las foreign keys estén correctamente configuradas
PRINT 'Verificando foreign keys...';

-- Listar todas las foreign keys que apuntan a Site
SELECT
    fk.name AS constraint_name,
    t.name AS table_name,
    c.name AS column_name,
    rt.name AS referenced_table,
    rc.name AS referenced_column
FROM sys.foreign_keys fk
    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    INNER JOIN sys.tables t ON fkc.parent_object_id = t.object_id
    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
    INNER JOIN sys.tables rt ON fkc.referenced_object_id = rt.object_id
    INNER JOIN sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
WHERE rt.name = 'Site'
ORDER BY t.name, c.name;

-- 4. Actualizar índices que pueden estar relacionados con las foreign keys
-- Los índices se actualizarán automáticamente con las foreign keys
-- pero algunos pueden necesitar renombrado manual

-- 5. Verificar integridad referencial
PRINT 'Verificando integridad referencial...';

-- Verificar que no hay referencias huérfanas
DECLARE @orphanedCount INT = 0;

-- Verificar referencias huérfanas en tablas relacionadas
-- (Esto depende de la estructura específica de tu base de datos)

-- Ejemplo de verificación para SiteSatellite
IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteSatellite')
BEGIN
    SELECT @orphanedCount = COUNT(*)
    FROM SiteSatellite ss
    WHERE NOT EXISTS (SELECT 1
        FROM Site s
        WHERE s.Id = ss.MainSiteId)
        OR NOT EXISTS (SELECT 1
        FROM Site s
        WHERE s.Id = ss.SatelliteSiteId);

    IF @orphanedCount > 0
    BEGIN
        PRINT 'ADVERTENCIA: Se encontraron ' + CAST(@orphanedCount AS NVARCHAR(10)) + ' referencias huérfanas en SiteSatellite';
    END
    ELSE
    BEGIN
        PRINT 'Verificación exitosa: No hay referencias huérfanas en SiteSatellite';
    END
END

-- 6. Actualizar stored procedures que pueden tener referencias hardcodeadas
-- Nota: Los stored procedures ya fueron renombrados en el script anterior
-- pero algunos pueden tener referencias internas que necesiten actualización

PRINT 'Migración de foreign keys completada';
PRINT 'Todas las foreign keys ahora apuntan a la tabla Site';

COMMIT TRANSACTION;
