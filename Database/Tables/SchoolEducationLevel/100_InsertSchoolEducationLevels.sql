-- =============================================
-- SP: Insertar niveles educativos para una escuela
-- =============================================
GO
CREATE OR ALTER PROCEDURE [dbo].[100_InsertSchoolEducationLevels]
    @schoolId INT,
    @educationLevelIds NVARCHAR(MAX)
-- IDs separados por coma: "1,2,3"
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Primero eliminar los niveles educativos existentes para esta escuela
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
GO