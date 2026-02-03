-- =============================================
-- Stored Procedure: 106_GetSiteServiceSlotOperatingDates
-- Descripción: Obtiene las fechas de operación por (ChildGroupId, ServiceTypeId)
--              para enriquecer los slots de servicio con días reales del calendario.
-- Parámetros: @siteid, @fromdate, @todate (rango del sitio).
-- Fecha: 2026-02-03
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[106_GetSiteServiceSlotOperatingDates]
    @siteid INT,
    @fromdate DATE,
    @todate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        childgroupid = sods.ChildGroupId,
        servicetypeid = sods.ServiceTypeId,
        operatingdate = sod.OperatingDate
    FROM SiteOperatingDayService sods
    INNER JOIN SiteOperatingDays sod ON sods.OperatingDayId = sod.Id
    WHERE sod.SiteId = @siteid
      AND sod.OperatingDate >= @fromdate
      AND sod.OperatingDate <= @todate
      AND sod.IsActive = 1
    ORDER BY sod.OperatingDate, sods.ChildGroupId, sods.ServiceTypeId;
END;
