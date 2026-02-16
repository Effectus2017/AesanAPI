-- =============================================
-- Stored Procedure: 102_GetSiteOperatingDayIdBySiteAndDate
-- Descripción: Obtiene el Id del día de funcionamiento para un sitio y fecha dada (IsActive = 1).
--              Usado desde ReplaceServicesForOperatingDaysBySlot para resolver OperatingDayId por fecha.
-- Fecha: 2026-02-03
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[102_GetSiteOperatingDayIdBySiteAndDate]
    @siteid INT,
    @operatingdate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id = Id
    FROM SiteOperatingDays
    WHERE SiteId = @siteid
      AND OperatingDate = @operatingdate
      AND IsActive = 1
      AND (IsHoliday = 0 OR IsHoliday IS NULL);
END;
GO
