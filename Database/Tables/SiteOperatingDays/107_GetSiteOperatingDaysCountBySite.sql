-- =============================================
-- Stored Procedure: 107_GetSiteOperatingDaysCountBySite
-- Descripción: Devuelve la cantidad total de días de funcionamiento activos de un sitio
--              (sin filtrar por rango de fechas). Usado desde GetSiteById cuando el sitio
--              no tiene OperatingFromDate/OperatingToDate para mostrar el total alineado con el calendario.
-- Fecha: 2026-02-16
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[107_GetSiteOperatingDaysCountBySite]
    @siteid INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT count = COUNT(*)
    FROM SiteOperatingDays
    WHERE SiteId = @siteid
      AND IsActive = 1;
END;
GO
