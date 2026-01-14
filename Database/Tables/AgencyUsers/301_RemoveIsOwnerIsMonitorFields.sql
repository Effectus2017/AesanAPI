-- =============================================
-- Script: Eliminar campos IsOwner e IsMonitor de tabla AgencyUsers
-- Fecha: [Fecha de implementación]
-- Descripción: Elimina los campos IsOwner e IsMonitor de la tabla AgencyUsers
--              después de verificar que todos los datos han sido migrados a AgencyAssignmentType
-- ⚠️ ADVERTENCIA: Solo ejecutar después de:
--    1. Verificar que todos los registros tienen AgencyAssignmentType
--    2. Verificar que ningún código C# usa IsOwner/IsMonitor
--    3. Verificar que todos los SPs nuevos funcionan correctamente
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY
    -- Verificar que todos los registros activos tienen AgencyAssignmentType
    DECLARE @recordsWithoutAssignmentType INT;
    SELECT @recordsWithoutAssignmentType = COUNT(*)
    FROM AgencyUsers
    WHERE IsActive = 1 AND (AgencyAssignmentType IS NULL OR AgencyAssignmentType = '');
    
    IF @recordsWithoutAssignmentType > 0
    BEGIN
        RAISERROR('Existen %d registros activos sin AgencyAssignmentType. No se pueden eliminar los campos.', 16, 1, @recordsWithoutAssignmentType);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT '✅ Verificación: Todos los registros activos tienen AgencyAssignmentType';
    
    -- Eliminar índices que usan IsOwner o IsMonitor (si existen)
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AgencyUsers_IsOwner' AND object_id = OBJECT_ID('AgencyUsers'))
    BEGIN
        DROP INDEX IX_AgencyUsers_IsOwner ON AgencyUsers;
        PRINT '✅ Índice IX_AgencyUsers_IsOwner eliminado';
    END
    
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AgencyUsers_IsMonitor' AND object_id = OBJECT_ID('AgencyUsers'))
    BEGIN
        DROP INDEX IX_AgencyUsers_IsMonitor ON AgencyUsers;
        PRINT '✅ Índice IX_AgencyUsers_IsMonitor eliminado';
    END
    
    -- Eliminar constraints por defecto de IsOwner
    DECLARE @constraintName NVARCHAR(256);
    DECLARE @sql NVARCHAR(MAX);
    
    -- Buscar y eliminar default constraints de IsOwner
    DECLARE constraint_cursor CURSOR FOR
    SELECT dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE c.object_id = OBJECT_ID('AgencyUsers') AND c.name = 'IsOwner';
    
    OPEN constraint_cursor;
    FETCH NEXT FROM constraint_cursor INTO @constraintName;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = 'ALTER TABLE AgencyUsers DROP CONSTRAINT ' + QUOTENAME(@constraintName);
        EXEC sp_executesql @sql;
        PRINT '✅ Constraint por defecto eliminado: ' + @constraintName;
        FETCH NEXT FROM constraint_cursor INTO @constraintName;
    END
    
    CLOSE constraint_cursor;
    DEALLOCATE constraint_cursor;
    
    -- Buscar y eliminar default constraints de IsMonitor
    DECLARE constraint_cursor2 CURSOR FOR
    SELECT dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE c.object_id = OBJECT_ID('AgencyUsers') AND c.name = 'IsMonitor';
    
    OPEN constraint_cursor2;
    FETCH NEXT FROM constraint_cursor2 INTO @constraintName;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = 'ALTER TABLE AgencyUsers DROP CONSTRAINT ' + QUOTENAME(@constraintName);
        EXEC sp_executesql @sql;
        PRINT '✅ Constraint por defecto eliminado: ' + @constraintName;
        FETCH NEXT FROM constraint_cursor2 INTO @constraintName;
    END
    
    CLOSE constraint_cursor2;
    DEALLOCATE constraint_cursor2;
    
    -- Eliminar check constraints de IsOwner (si existen)
    DECLARE constraint_cursor3 CURSOR FOR
    SELECT cc.name
    FROM sys.check_constraints cc
    INNER JOIN sys.columns c ON cc.parent_object_id = c.object_id
    WHERE c.object_id = OBJECT_ID('AgencyUsers') 
      AND c.name = 'IsOwner'
      AND cc.definition LIKE '%IsOwner%';
    
    OPEN constraint_cursor3;
    FETCH NEXT FROM constraint_cursor3 INTO @constraintName;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = 'ALTER TABLE AgencyUsers DROP CONSTRAINT ' + QUOTENAME(@constraintName);
        EXEC sp_executesql @sql;
        PRINT '✅ Check constraint eliminado: ' + @constraintName;
        FETCH NEXT FROM constraint_cursor3 INTO @constraintName;
    END
    
    CLOSE constraint_cursor3;
    DEALLOCATE constraint_cursor3;
    
    -- Eliminar check constraints de IsMonitor (si existen)
    DECLARE constraint_cursor4 CURSOR FOR
    SELECT cc.name
    FROM sys.check_constraints cc
    INNER JOIN sys.columns c ON cc.parent_object_id = c.object_id
    WHERE c.object_id = OBJECT_ID('AgencyUsers') 
      AND c.name = 'IsMonitor'
      AND cc.definition LIKE '%IsMonitor%';
    
    OPEN constraint_cursor4;
    FETCH NEXT FROM constraint_cursor4 INTO @constraintName;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = 'ALTER TABLE AgencyUsers DROP CONSTRAINT ' + QUOTENAME(@constraintName);
        EXEC sp_executesql @sql;
        PRINT '✅ Check constraint eliminado: ' + @constraintName;
        FETCH NEXT FROM constraint_cursor4 INTO @constraintName;
    END
    
    CLOSE constraint_cursor4;
    DEALLOCATE constraint_cursor4;
    
    -- Eliminar columna IsOwner
    IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'IsOwner' AND object_id = OBJECT_ID('AgencyUsers'))
    BEGIN
        ALTER TABLE AgencyUsers DROP COLUMN IsOwner;
        PRINT '✅ Columna IsOwner eliminada';
    END
    ELSE
    BEGIN
        PRINT '⚠️  Columna IsOwner no existe (ya eliminada)';
    END
    
    -- Eliminar columna IsMonitor
    IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'IsMonitor' AND object_id = OBJECT_ID('AgencyUsers'))
    BEGIN
        ALTER TABLE AgencyUsers DROP COLUMN IsMonitor;
        PRINT '✅ Columna IsMonitor eliminada';
    END
    ELSE
    BEGIN
        PRINT '⚠️  Columna IsMonitor no existe (ya eliminada)';
    END
    
    COMMIT TRANSACTION;
    PRINT '';
    PRINT '✅ Campos IsOwner e IsMonitor eliminados exitosamente de la tabla AgencyUsers';
    PRINT '⚠️  IMPORTANTE: Verificar que la aplicación funciona correctamente después de este cambio';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorNumber INT = ERROR_NUMBER();
    DECLARE @ErrorLine INT = ERROR_LINE();
    
    PRINT '❌ Error al eliminar campos:';
    PRINT '   Error: ' + @ErrorMessage;
    PRINT '   Número: ' + CAST(@ErrorNumber AS NVARCHAR(10));
    PRINT '   Línea: ' + CAST(@ErrorLine AS NVARCHAR(10));
    THROW;
END CATCH
GO
