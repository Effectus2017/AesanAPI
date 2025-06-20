-- =============================================
-- SP: Actualizar niveles educativos para una escuela
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSchoolEducationLevels]
    @schoolId INT,
    @educationLevelIds NVARCHAR(MAX)
-- IDs separados por coma: "1,2,3"
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Eliminar todos los niveles educativos existentes para esta escuela
        DELETE FROM SchoolEducationLevel WHERE SchoolId = @schoolId;
        
        -- Insertar los nuevos niveles educativos
        IF @educationLevelIds IS NOT NULL AND LEN(TRIM(@educationLevelIds)) > 0
        BEGIN
        INSERT INTO SchoolEducationLevel
            (SchoolId, EducationLevelId, IsActive, CreatedAt)
        SELECT
            @schoolId,
            CAST(value AS INT),
            1,
            GETDATE()
        FROM STRING_SPLIT(@educationLevelIds, ',')
        WHERE TRIM(value) != '';
    END
        
        COMMIT TRANSACTION;
        RETURN 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

-- =============================================
-- TESTING EXAMPLE:
-- =============================================
/*
-- Test 1: Verificar eliminación y re-inserción completa
EXEC [100_UpdateSchoolEducationLevels] @schoolId = 3, @educationLevelIds = '1,2,3';
SELECT * FROM SchoolEducationLevel WHERE SchoolId = 3;

-- Test 2: Cambiar los niveles educativos (eliminar uno, agregar otro)
EXEC [100_UpdateSchoolEducationLevels] @schoolId = 3, @educationLevelIds = '1,4,5';
SELECT * FROM SchoolEducationLevel WHERE SchoolId = 3;

-- Test 3: Verificar que no queden registros duplicados
SELECT SchoolId, EducationLevelId, COUNT(*) as Count 
FROM SchoolEducationLevel 
WHERE SchoolId = 3 
GROUP BY SchoolId, EducationLevelId 
HAVING COUNT(*) > 1;
*/