-- =============================================
-- Stored Procedure: 105_GetSiteOperatingDayIdsBySiteAndDateRangePatternOnly
-- Descripción: Devuelve los Id de los días de funcionamiento activos de un sitio en un rango
--              que pertenecen al patrón semanal (IsManuallyAdded = 0). Excluye días agregados
--              manualmente desde el calendario. Usado al insertar un servicio nuevo sin fechas
--              explícitas para no asignarlo a días extra del calendario.
-- Fecha: 2026-02-04
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[105_GetSiteOperatingDayIdsBySiteAndDateRangePatternOnly]
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
      AND IsActive = 1
      AND (IsHoliday = 0 OR IsHoliday IS NULL)
      AND COALESCE(IsManuallyAdded, 0) = 0;
END;
GO
