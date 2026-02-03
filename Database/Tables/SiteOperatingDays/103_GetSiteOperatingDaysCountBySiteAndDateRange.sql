-- =============================================
-- Stored Procedure: 103_GetSiteOperatingDaysCountBySiteAndDateRange
-- Descripción: Devuelve la cantidad de días de funcionamiento activos de un sitio en un rango de fechas.
--              Usado desde InsertServicesForOperatingDays para logging cuando no se insertan filas.
-- Fecha: 2026-02-03
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[103_GetSiteOperatingDaysCountBySiteAndDateRange]
    @siteid INT,
    @fromdate DATE,
    @todate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT count = COUNT(*)
    FROM SiteOperatingDays
    WHERE SiteId = @siteid
      AND OperatingDate >= @fromdate
      AND OperatingDate <= @todate
      AND IsActive = 1;
END;
GO
