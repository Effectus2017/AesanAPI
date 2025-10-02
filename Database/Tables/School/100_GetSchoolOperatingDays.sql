-- Stored Procedure para obtener días de funcionamiento de una escuela
-- Retorna todos los días de funcionamiento para una escuela específica
-- Incluye información de la escuela y los días de funcionamiento
CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolOperatingDays]
    @schoolId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar parámetros
        IF @schoolId IS NULL OR @schoolId <= 0
        BEGIN
        RAISERROR('SchoolId es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        -- Verificar que la escuela existe
        IF NOT EXISTS (SELECT 1
    FROM School
    WHERE Id = @SchoolId)
        BEGIN
        RAISERROR('La escuela especificada no existe', 16, 1);
        RETURN;
    END

        -- Obtener información de la escuela
        SELECT
        s.Id as SchoolId,
        s.Name as SchoolName
    FROM School s
    WHERE s.Id = @schoolId;

        -- Obtener días de funcionamiento
        SELECT
        sod.Id,
        sod.SchoolId,
        sod.OperatingDate,
        sod.StartTime,
        sod.EndTime,
        sod.IsWeekendOverride,
        sod.IsExcluded,
        sod.Comment,
        sod.CreatedAt,
        sod.UpdatedAt
    FROM SchoolOperatingDays sod
    WHERE sod.SchoolId = @schoolId
    ORDER BY sod.OperatingDate;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
