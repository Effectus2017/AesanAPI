-- Migración: Cambiar SdrNumber de INT a BIGINT para soportar números de hasta 10 dígitos
-- Fecha: 2024
-- Descripción: El campo SdrNumber necesita soportar números de hasta 10 dígitos (ej: 4123412341)
--              que exceden el límite de INT (2,147,483,647). Se cambia a BIGINT que permite
--              valores hasta 9,223,372,036,854,775,807.

-- IMPORTANTE: Ejecutar este script en la base de datos antes de desplegar los cambios en el código

BEGIN TRANSACTION;

BEGIN TRY
    -- Cambiar el tipo de dato de la columna SdrNumber
    ALTER TABLE Agency
    ALTER COLUMN SdrNumber BIGINT NOT NULL;

    PRINT 'Migración completada exitosamente: SdrNumber cambiado de INT a BIGINT';
    
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    PRINT 'Error durante la migración:';
    PRINT ERROR_MESSAGE();
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
