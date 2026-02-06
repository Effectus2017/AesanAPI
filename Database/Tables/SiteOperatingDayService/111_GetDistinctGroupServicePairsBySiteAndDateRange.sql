-- =============================================
-- Stored Procedure: 111_GetDistinctGroupServicePairsBySiteAndDateRange
-- Descripción: Devuelve los pares distintos (ChildGroupId, ServiceTypeId) que existen
--              en SiteOperatingDayService para días del sitio en el rango dado.
--              Usado para identificar servicios a eliminar cuando el usuario los quita.
-- Fecha: 2026-02-04
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[111_GetDistinctGroupServicePairsBySiteAndDateRange]
    @siteid INT,
    @fromdate DATE,
    @todate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        childgroupid = sods.ChildGroupId,
        servicetypeid = sods.ServiceTypeId
    FROM SiteOperatingDayService sods
    INNER JOIN SiteOperatingDays sod ON sods.OperatingDayId = sod.Id
    WHERE sod.SiteId = @siteid
      AND sod.OperatingDate >= @fromdate
      AND sod.OperatingDate <= @todate
      AND sod.IsActive = 1;
END;
GO
