-- =============================================
-- Stored Procedure: 110_GetOperatingDayIdsByGroupServiceAndDateRange
-- Descripción: Devuelve los OperatingDayId donde ya existe una fila en SiteOperatingDayService
--              para el (ChildGroupId, ServiceTypeId) dado y cuyos días pertenecen al sitio en el rango.
--              Usado desde ReplaceServicesForOperatingDaysBySlot cuando operatingDates está vacío
--              para no insertar duplicados.
-- Fecha: 2026-02-03
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[110_GetOperatingDayIdsByGroupServiceAndDateRange]
    @siteid INT,
    @childgroupid INT,
    @servicetypeid INT,
    @fromdate DATE,
    @todate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id = sods.OperatingDayId
    FROM SiteOperatingDayService sods
    INNER JOIN SiteOperatingDays sod ON sod.Id = sods.OperatingDayId
    WHERE sod.SiteId = @siteid
      AND sods.ChildGroupId = @childgroupid
      AND sods.ServiceTypeId = @servicetypeid
      AND sod.OperatingDate >= @fromdate
      AND sod.OperatingDate <= @todate;
END;
GO
