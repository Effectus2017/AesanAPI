-- Migración: Cambiar UieNumber de NVARCHAR(12) a BIGINT para soportar números de hasta 12 dígitos
-- Fecha: 2024
-- Descripción: El campo UieNumber necesita ser numérico (BIGINT) para mantener consistencia
--              con SdrNumber y EinNumber. Permite valores hasta 12 dígitos (999,999,999,999).

-- IMPORTANTE: Ejecutar este script en la base de datos antes de desplegar los cambios en el código
-- ADVERTENCIA: Este script validará que no haya valores alfanuméricos antes de convertir

BEGIN TRANSACTION;

BEGIN TRY
    -- Verificar si hay valores alfanuméricos (que contengan letras)
    DECLARE @AlphanumericCount INT;
    SELECT @AlphanumericCount = COUNT(*)
FROM Agency
WHERE UieNumber IS NOT NULL
    AND UieNumber != ''
    AND (
            -- Verificar si contiene letras (a-z, A-Z)
            UieNumber LIKE '%[a-z]%' COLLATE Latin1_General_BIN
    OR UieNumber LIKE '%[A-Z]%' COLLATE Latin1_General_BIN
    OR ISNUMERIC(UieNumber) = 0
        );

    IF @AlphanumericCount > 0
    BEGIN
    DECLARE @ErrorMessage NVARCHAR(500) = 
            'Error: Se encontraron ' + CAST(@AlphanumericCount AS NVARCHAR(10)) + 
            ' registros con valores alfanuméricos en UieNumber. ' +
            'Debe limpiar estos valores antes de ejecutar la migración.';
    PRINT @ErrorMessage;
    THROW 50000, @ErrorMessage, 1;
END

    -- Verificar si hay valores que excedan el rango de BIGINT
    DECLARE @OutOfRangeCount INT;
    SELECT @OutOfRangeCount = COUNT(*)
FROM Agency
WHERE UieNumber IS NOT NULL
    AND UieNumber != ''
    AND (
            TRY_CAST(UieNumber AS BIGINT) IS NULL
    OR CAST(UieNumber AS BIGINT) > 999999999999
    OR CAST(UieNumber AS BIGINT) < 1
        );

    IF @OutOfRangeCount > 0
    BEGIN
    DECLARE @RangeErrorMessage NVARCHAR(500) = 
            'Error: Se encontraron ' + CAST(@OutOfRangeCount AS NVARCHAR(10)) + 
            ' registros con valores fuera del rango válido (1-999999999999). ' +
            'Debe corregir estos valores antes de ejecutar la migración.';
    PRINT @RangeErrorMessage;
    THROW 50001, @RangeErrorMessage, 1;
END

    -- Cambiar el tipo de dato de la columna UieNumber
    ALTER TABLE Agency
    ALTER COLUMN UieNumber BIGINT NOT NULL;

    PRINT 'Migración completada exitosamente: UieNumber cambiado de NVARCHAR(12) a BIGINT';
    
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    PRINT 'Error durante la migración:';
    PRINT ERROR_MESSAGE();
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
