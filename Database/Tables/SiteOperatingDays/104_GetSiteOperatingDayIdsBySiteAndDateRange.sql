-- =============================================
-- Stored Procedure: 104_GetSiteOperatingDayIdsBySiteAndDateRange
-- Descripción: Devuelve los Id de los días de funcionamiento activos de un sitio en un rango de fechas.
--              Usado desde ReplaceServicesForOperatingDaysBySlot cuando operatingDates está vacío (grupo nuevo).
-- Fecha: 2026-02-03
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[104_GetSiteOperatingDayIdsBySiteAndDateRange]
    @siteid INT,
    @fromdate DATE,
    @todate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id = Id
    FROM SiteOperatingDays
    WHERE SiteId = @siteid
      AND OperatingDate >= @fromdate
      AND OperatingDate <= @todate
      AND IsActive = 1;
END;
GO
