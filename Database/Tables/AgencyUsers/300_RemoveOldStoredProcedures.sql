-- =============================================
-- Script: Verificar SPs antiguos para mover a Deprecated
-- Fecha: [Fecha de implementación]
-- Descripción: Verifica qué SPs antiguos existen y deben moverse a Deprecated
-- ⚠️ IMPORTANTE: Los SPs NUNCA se eliminan de la base de datos, solo se mueven los archivos SQL a Deprecated
-- =============================================

-- Este script solo verifica qué SPs antiguos existen
-- Los archivos SQL deben moverse manualmente a la carpeta Deprecated

PRINT '=============================================';
PRINT 'Verificación de SPs antiguos';
PRINT '=============================================';
PRINT '';

-- Lista de SPs antiguos que deben moverse a Deprecated
DECLARE @spsToDeprecate TABLE (
    SPName NVARCHAR(255),
    CurrentPath NVARCHAR(500),
    DeprecatedPath NVARCHAR(500)
);

INSERT INTO @spsToDeprecate VALUES
    ('101_AssignAgencyToUser', 'Database/Tables/AgencyUsers/101_AssignAgencyToUser.sql', 'Deprecated/Api/Database/Tables/AgencyUsers/101_AssignAgencyToUser.sql'),
    ('103_GetUserAssignedAgency', 'Database/Tables/User/103_GetUserAssignedAgency.sql', 'Deprecated/Api/Database/Tables/User/103_GetUserAssignedAgency.sql'),
    ('110_UpdateUser', 'Database/Tables/User/110_UpdateUser.sql', 'Deprecated/Api/Database/Tables/User/110_UpdateUser.sql'),
    ('101_UpdateUserMainAgency', 'Database/Tables/AgencyUsers/101_UpdateUserMainAgency.sql', 'Deprecated/Api/Database/Tables/AgencyUsers/101_UpdateUserMainAgency.sql'),
    ('100_GetUserAssignedAgencies', 'Database/Tables/User/100_GetUserAssignedAgencies.sql', 'Deprecated/Api/Database/Tables/User/100_GetUserAssignedAgencies.sql'),
    ('113_GetAgencyById', 'Database/Tables/Agency/113_GetAgencyById.sql', 'Deprecated/Api/Database/Tables/Agency/113_GetAgencyById.sql'),
    ('112_GetAgencyByIdAndUserId', 'Database/Tables/Agency/112_GetAgencyByIdAndUserId.sql', 'Deprecated/Api/Database/Tables/Agency/112_GetAgencyByIdAndUserId.sql'),
    ('117_GetAgencies', 'Database/Tables/Agency/117_GetAgencies.sql', 'Deprecated/Api/Database/Tables/Agency/117_GetAgencies.sql'),
    ('101_UnassignAgencyToUser', 'Database/Tables/AgencyUsers/101_UnassignAgencyToUser.sql', 'Deprecated/Api/Database/Tables/AgencyUsers/101_UnassignAgencyToUser.sql');

-- Verificar existencia de cada SP en la base de datos
DECLARE @spName NVARCHAR(255);
DECLARE @currentPath NVARCHAR(500);
DECLARE @deprecatedPath NVARCHAR(500);
DECLARE @exists BIT;

DECLARE sp_cursor CURSOR FOR
SELECT SPName, CurrentPath, DeprecatedPath FROM @spsToDeprecate;

OPEN sp_cursor;
FETCH NEXT FROM sp_cursor INTO @spName, @currentPath, @deprecatedPath;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @exists = CASE WHEN EXISTS (SELECT 1 FROM sys.procedures WHERE name = @spName) THEN 1 ELSE 0 END;
    
    IF @exists = 1
    BEGIN
        PRINT '✅ SP existe en BD: ' + @spName;
        PRINT '   📁 Mover archivo de: ' + @currentPath;
        PRINT '   📁 A: ' + @deprecatedPath;
        PRINT '   ⚠️  NOTA: El SP permanece en la BD, solo se mueve el archivo SQL';
    END
    ELSE
    BEGIN
        PRINT '⚠️  SP no existe en BD: ' + @spName;
        PRINT '   (Puede que ya haya sido eliminado o nunca existió)';
    END
    
    PRINT '';
    
    FETCH NEXT FROM sp_cursor INTO @spName, @currentPath, @deprecatedPath;
END

CLOSE sp_cursor;
DEALLOCATE sp_cursor;

PRINT '=============================================';
PRINT 'Resumen:';
PRINT '=============================================';
PRINT 'Los SPs antiguos NO se eliminan de la base de datos.';
PRINT 'Solo se mueven los archivos SQL a la carpeta Deprecated para referencia histórica.';
PRINT 'Los SPs pueden permanecer en la BD indefinidamente sin causar problemas.';
PRINT '';
PRINT 'Pasos a seguir:';
PRINT '1. Verificar que ningún código C# usa estos SPs antiguos';
PRINT '2. Mover los archivos SQL manualmente a Deprecated/';
PRINT '3. Los SPs en la BD pueden quedarse (no causan problemas si no se usan)';
GO

