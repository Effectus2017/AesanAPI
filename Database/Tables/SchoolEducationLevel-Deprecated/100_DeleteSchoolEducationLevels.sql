-- =============================================
-- DEPRECATED: Este SP ha sido reemplazado por 100_DeleteSiteEducationLevels
-- Fecha de deprecación: 2025-01-15
-- Razón: Migración de School a Site
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