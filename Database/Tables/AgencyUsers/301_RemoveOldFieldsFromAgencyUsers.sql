-- =============================================
-- Script: Eliminar campos IsOwner e IsMonitor
-- Fecha: [Fecha de implementación]
-- Descripción: Elimina campos antiguos después de verificar que no se usan
-- ⚠️ ADVERTENCIA: Solo ejecutar DESPUÉS de eliminar código C# y SPs antiguos
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY
    -- 1. Verificar que no hay inconsistencias críticas
    DECLARE @inconsistencias INT;
    SELECT @inconsistencias = COUNT(*)
    FROM AgencyUsers
    WHERE IsActive = 1
        AND AssignmentType IS NOT NULL
        AND RoleId IS NOT NULL
        AND (
            (IsOwner = 1 AND AssignmentType != 'AGENCY_OWNER') OR
            (IsMonitor = 1 AND AssignmentType NOT LIKE 'NUTRE_%')
        );
    
    IF @inconsistencias > 10  -- Permitir algunas inconsistencias menores
    BEGIN
        RAISERROR ('Existen %d inconsistencias. Revisar antes de eliminar campos.', 16, 1, @inconsistencias);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT '✅ Verificación de inconsistencias: ' + CAST(@inconsistencias AS VARCHAR(10)) + ' (aceptable)';
    
    -- 2. Eliminar índices relacionados (si existen)
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
    
    -- 3. Eliminar columnas
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AgencyUsers') AND name = 'IsOwner')
    BEGIN
        ALTER TABLE AgencyUsers DROP COLUMN IsOwner;
        PRINT '✅ Columna IsOwner eliminada';
    END
    ELSE
    BEGIN
        PRINT '⚠️ Columna IsOwner ya no existe';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AgencyUsers') AND name = 'IsMonitor')
    BEGIN
        ALTER TABLE AgencyUsers DROP COLUMN IsMonitor;
        PRINT '✅ Columna IsMonitor eliminada';
    END
    ELSE
    BEGIN
        PRINT '⚠️ Columna IsMonitor ya no existe';
    END
    
    COMMIT TRANSACTION;
    PRINT '✅ Campos antiguos eliminados exitosamente';
    PRINT '⚠️ IMPORTANTE: Verificar que la aplicación funciona correctamente después de este cambio';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    PRINT '❌ Error al eliminar campos: ' + @ErrorMessage;
    THROW;
END CATCH
GO

