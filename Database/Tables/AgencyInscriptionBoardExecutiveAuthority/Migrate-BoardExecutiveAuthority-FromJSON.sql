-- =============================================
-- Script de Migración: BoardExecutiveAuthority de JSON a Tabla de Relación
-- Descripción: Migra los datos de BoardExecutiveAuthority desde JSON (nvarchar) a la tabla AgencyInscriptionBoardExecutiveAuthority
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

SET NOCOUNT ON;

DECLARE @TotalRecords INT = 0;
DECLARE @MigratedRecords INT = 0;
DECLARE @ErrorRecords INT = 0;
DECLARE @AgencyInscriptionId INT;
DECLARE @BoardExecutiveAuthority NVARCHAR(MAX);
DECLARE @JsonValue NVARCHAR(MAX);
DECLARE @OptionSelectionId INT;

BEGIN TRY
    BEGIN TRANSACTION;

    -- Contar registros a migrar
    SELECT @TotalRecords = COUNT(*)
    FROM AgencyInscription
    WHERE BoardExecutiveAuthority IS NOT NULL
        AND LEN(LTRIM(RTRIM(BoardExecutiveAuthority))) > 0
        AND BoardExecutiveAuthority != 'null'
        AND BoardExecutiveAuthority != '[]';

    PRINT '========================================';
    PRINT 'Migración de BoardExecutiveAuthority';
    PRINT '========================================';
    PRINT 'Total de registros a migrar: ' + CAST(@TotalRecords AS NVARCHAR(10));
    PRINT '';

    -- Cursor para procesar cada registro
    DECLARE migration_cursor CURSOR FOR
    SELECT 
        ai.Id AS AgencyInscriptionId,
        ai.BoardExecutiveAuthority
    FROM AgencyInscription ai
    WHERE ai.BoardExecutiveAuthority IS NOT NULL
        AND LEN(LTRIM(RTRIM(ai.BoardExecutiveAuthority))) > 0
        AND ai.BoardExecutiveAuthority != 'null'
        AND ai.BoardExecutiveAuthority != '[]';

    OPEN migration_cursor;
    FETCH NEXT FROM migration_cursor INTO @AgencyInscriptionId, @BoardExecutiveAuthority;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRY
            -- Limpiar el JSON (remover espacios y caracteres especiales)
            SET @JsonValue = LTRIM(RTRIM(@BoardExecutiveAuthority));
            
            -- Remover corchetes si existen
            IF LEFT(@JsonValue, 1) = '['
                SET @JsonValue = SUBSTRING(@JsonValue, 2, LEN(@JsonValue) - 2);
            
            -- Verificar que no esté vacío después de limpiar
            IF LEN(@JsonValue) > 0
            BEGIN
                -- Crear tabla temporal para almacenar los IDs parseados
                DECLARE @TempIds TABLE (OptionSelectionId INT);

                -- Parsear los IDs del JSON (separados por coma)
                INSERT INTO @TempIds (OptionSelectionId)
                SELECT CAST(LTRIM(RTRIM(value)) AS INT)
                FROM STRING_SPLIT(@JsonValue, ',')
                WHERE LTRIM(RTRIM(value)) != ''
                    AND ISNUMERIC(LTRIM(RTRIM(value))) = 1;

                -- Validar que todos los IDs existan en OptionSelection con OptionKey = 'boardExecutiveAuthority'
                IF EXISTS (
                    SELECT 1
                    FROM @TempIds ti
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM OptionSelection os
                        WHERE os.Id = ti.OptionSelectionId
                            AND os.OptionKey = 'boardExecutiveAuthority'
                            AND os.IsActive = 1
                    )
                )
                BEGIN
                    PRINT 'ERROR: AgencyInscriptionId ' + CAST(@AgencyInscriptionId AS NVARCHAR(10)) + 
                          ' contiene OptionSelectionId inválidos. Saltando registro.';
                    SET @ErrorRecords = @ErrorRecords + 1;
                END
                ELSE
                BEGIN
                    -- Eliminar registros existentes para esta inscripción (por si acaso)
                    DELETE FROM AgencyInscriptionBoardExecutiveAuthority
                    WHERE AgencyInscriptionId = @AgencyInscriptionId;

                    -- Insertar los nuevos registros
                    INSERT INTO AgencyInscriptionBoardExecutiveAuthority
                        (AgencyInscriptionId, OptionSelectionId, IsActive, CreatedAt)
                    SELECT @AgencyInscriptionId, OptionSelectionId, 1, GETDATE()
                    FROM @TempIds;

                    SET @MigratedRecords = @MigratedRecords + 1;
                    
                    IF @MigratedRecords % 10 = 0
                        PRINT 'Migrados ' + CAST(@MigratedRecords AS NVARCHAR(10)) + ' registros...';
                END
            END
        END TRY
        BEGIN CATCH
            PRINT 'ERROR al migrar AgencyInscriptionId ' + CAST(@AgencyInscriptionId AS NVARCHAR(10)) + 
                  ': ' + ERROR_MESSAGE();
            SET @ErrorRecords = @ErrorRecords + 1;
        END CATCH

        FETCH NEXT FROM migration_cursor INTO @AgencyInscriptionId, @BoardExecutiveAuthority;
    END

    CLOSE migration_cursor;
    DEALLOCATE migration_cursor;

    COMMIT TRANSACTION;

    PRINT '';
    PRINT '========================================';
    PRINT 'Migración completada';
    PRINT '========================================';
    PRINT 'Total de registros procesados: ' + CAST(@TotalRecords AS NVARCHAR(10));
    PRINT 'Registros migrados exitosamente: ' + CAST(@MigratedRecords AS NVARCHAR(10));
    PRINT 'Registros con errores: ' + CAST(@ErrorRecords AS NVARCHAR(10));
    PRINT '';

    -- Verificar integridad
    DECLARE @TotalInNewTable INT;
    SELECT @TotalInNewTable = COUNT(DISTINCT AgencyInscriptionId)
    FROM AgencyInscriptionBoardExecutiveAuthority
    WHERE IsActive = 1;

    PRINT 'Total de AgencyInscriptionId únicos en nueva tabla: ' + CAST(@TotalInNewTable AS NVARCHAR(10));
    PRINT '';

    IF @MigratedRecords = @TotalRecords
        PRINT '✓ Migración exitosa: Todos los registros fueron migrados correctamente.';
    ELSE
        PRINT '⚠ Advertencia: No todos los registros fueron migrados. Revisar errores.';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT '';
    PRINT '========================================';
    PRINT 'ERROR EN MIGRACIÓN';
    PRINT '========================================';
    PRINT 'Error: ' + ERROR_MESSAGE();
    PRINT 'Línea: ' + CAST(ERROR_LINE() AS NVARCHAR(10));
    PRINT '';

    RAISERROR('Error durante la migración. Transacción revertida.', 16, 1);
END CATCH;
GO
