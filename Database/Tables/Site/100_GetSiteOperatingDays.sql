-- Stored Procedure para obtener días de funcionamiento de un sitio
-- Retorna días de funcionamiento para un sitio específico
-- Filtra opcionalmente por mes y año para mejorar el rendimiento
-- Incluye información del sitio y los días de funcionamiento
CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteOperatingDays]
    @siteId INT,
    @month INT = NULL,
    @year INT = NULL
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

        -- Validar mes si se proporciona
        IF @month IS NOT NULL AND (@month < 1 OR @month > 12)
        BEGIN
        RAISERROR('El mes debe estar entre 1 y 12', 16, 1);
        RETURN;
    END

        -- Validar año si se proporciona
        IF @year IS NOT NULL AND @year < 2000
        BEGIN
        RAISERROR('El año debe ser mayor a 2000', 16, 1);
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

        -- Obtener días de funcionamiento con filtro opcional por mes/año
        SELECT
        sod.Id,
        sod.SiteId,
        sod.OperatingDate,
        sod.StartTime,
        sod.EndTime,
        sod.IsWeekend,
        sod.IsHoliday,
        sod.Comment,
        sod.CreatedAt,
        sod.UpdatedAt
    FROM SiteOperatingDays sod
    WHERE sod.SiteId = @siteId
        AND (@month IS NULL OR MONTH(sod.OperatingDate) = @month)
        AND (@year IS NULL OR YEAR(sod.OperatingDate) = @year)
    ORDER BY sod.OperatingDate;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
