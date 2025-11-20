-- =============================================
-- Migración: Cambiar UieNumber de INT/BIGINT a NVARCHAR(12)
-- Fecha: 2025-03-14
-- Descripción: Cambia el tipo de dato de UieNumber a NVARCHAR(12)
--              para soportar valores alfanuméricos de hasta 12 caracteres
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY
    -- Cambiar la columna UieNumber a NVARCHAR(12) en la tabla Agency
    -- Primero convertir a VARCHAR temporalmente si es necesario
    ALTER TABLE Agency
    ALTER COLUMN UieNumber NVARCHAR(12) NOT NULL;

    PRINT 'Migración completada exitosamente: UieNumber cambiado a NVARCHAR(12)';
    
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    PRINT 'Error en la migración: ' + @ErrorMessage;
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO

