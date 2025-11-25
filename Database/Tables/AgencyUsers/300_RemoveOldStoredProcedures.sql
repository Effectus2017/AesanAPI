-- =============================================
-- Script: Eliminar SPs antiguos
-- Fecha: [Fecha de implementación]
-- Descripción: Elimina SPs antiguos después de verificar que no se usan
-- ⚠️ ADVERTENCIA: Solo ejecutar después de confirmar que ningún código los usa
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY
    -- Lista de SPs antiguos a eliminar
    DECLARE @spsToRemove TABLE (SPName NVARCHAR(255));
    
    INSERT INTO @spsToRemove VALUES
        ('101_AssignAgencyToUser'),
        ('103_GetUserAssignedAgency'),
        ('110_UpdateUser'),
        ('101_UpdateUserMainAgency'),
        ('100_GetUserAssignedAgencies'),
        ('113_GetAgencyById'),
        ('112_GetAgencyByIdAndUserId'),
        ('117_GetAgencies'),
        ('100_GetAesanDashboardMetrics'),
        ('101_UnassignAgencyToUser');
    
    -- Eliminar cada SP
    DECLARE @spName NVARCHAR(255);
    DECLARE @sql NVARCHAR(MAX);
    
    DECLARE sp_cursor CURSOR FOR
    SELECT SPName FROM @spsToRemove;
    
    OPEN sp_cursor;
    FETCH NEXT FROM sp_cursor INTO @spName;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = @spName)
        BEGIN
            SET @sql = 'DROP PROCEDURE [dbo].[' + @spName + ']';
            EXEC sp_executesql @sql;
            PRINT '✅ SP eliminado: ' + @spName;
        END
        ELSE
        BEGIN
            PRINT '⚠️ SP no existe (ya eliminado): ' + @spName;
        END
        
        FETCH NEXT FROM sp_cursor INTO @spName;
    END
    
    CLOSE sp_cursor;
    DEALLOCATE sp_cursor;
    
    COMMIT TRANSACTION;
    PRINT '✅ SPs antiguos eliminados exitosamente';
    PRINT '⚠️ IMPORTANTE: Verificar que la aplicación funciona correctamente después de este cambio';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    PRINT '❌ Error al eliminar SPs: ' + @ErrorMessage;
    THROW;
END CATCH
GO

