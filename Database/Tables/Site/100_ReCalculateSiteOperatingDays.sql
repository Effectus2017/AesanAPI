-- =============================================
-- Stored Procedure: 100_ReCalculateSiteOperatingDays
-- Descripción: Recalcula OperatingDaysCalculated del sitio:
--             lista los días en SiteOperatingDays para el sitio,
--             cuenta solo los que NO son feriados (IsHoliday = 0 o NULL),
--             actualiza Site.OperatingDaysCalculated con esa suma.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_ReCalculateSiteOperatingDays]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @totalDays INT = 0;

    BEGIN TRY
        -- Contar días del sitio que no son feriados (solo activos)
        SELECT @totalDays = COUNT(*)
        FROM SiteOperatingDays
        WHERE SiteId = @siteId
          AND IsActive = 1
          AND (IsHoliday = 0 OR IsHoliday IS NULL);

        IF @totalDays < 0
            SET @totalDays = 0;

        -- Actualizar el campo en la tabla Site
        UPDATE Site
        SET OperatingDaysCalculated = @totalDays
        WHERE Id = @siteId;
    END TRY
    BEGIN CATCH
        UPDATE Site
        SET OperatingDaysCalculated = NULL
        WHERE Id = @siteId;
    END CATCH
END;
GO
