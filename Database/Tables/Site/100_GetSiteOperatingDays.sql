-- Stored Procedure para obtener días de funcionamiento de un sitio
-- Retorna todos los días de funcionamiento para un sitio específico
-- Incluye información del sitio y los días de funcionamiento
CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteOperatingDays]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar parámetros
        IF @siteId IS NULL OR @siteId <= 0
        BEGIN
        RAISERROR('SiteId es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        -- Verificar que el sitio existe
        IF NOT EXISTS (SELECT 1
    FROM Site
    WHERE Id = @siteId)
        BEGIN
        RAISERROR('El sitio especificado no existe', 16, 1);
        RETURN;
    END

        -- Obtener información del sitio
        SELECT
        s.Id as SiteId,
        s.Name as SiteName
    FROM Site s
    WHERE s.Id = @siteId;

        -- Obtener días de funcionamiento
        SELECT
        sod.Id,
        sod.SiteId,
        sod.OperatingDate,
        sod.StartTime,
        sod.EndTime,
        sod.IsWeekendOverride,
        sod.IsExcluded,
        sod.Comment,
        sod.CreatedAt,
        sod.UpdatedAt
    FROM SiteOperatingDays sod
    WHERE sod.SiteId = @siteId
    ORDER BY sod.OperatingDate;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
