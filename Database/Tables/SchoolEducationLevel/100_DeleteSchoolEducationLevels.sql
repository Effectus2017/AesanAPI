-- =============================================
-- SP: Eliminar niveles educativos de una escuela
-- =============================================
GO
CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSchoolEducationLevels]
    @schoolId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE SchoolEducationLevel 
        SET IsActive = 0, UpdatedAt = GETDATE()
        WHERE SchoolId = @schoolId;
        
        RETURN @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END; 