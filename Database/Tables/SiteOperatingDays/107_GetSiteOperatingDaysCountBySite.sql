-- =============================================
-- Stored Procedure: 107_GetSiteOperatingDaysCountBySite
-- Descripción: Devuelve la cantidad total de días de funcionamiento activos de un sitio
--              (sin filtrar por rango de fechas). Excluye días feriados del conteo.
--              Usado desde GetSiteById cuando el sitio no tiene OperatingFromDate/OperatingToDate.
-- Fecha: 2026-02-16
-- Versión: 1.1
-- Cambios v1.1: Excluir días feriados del conteo (IsHoliday = 0 OR IsHoliday IS NULL)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[107_GetSiteOperatingDaysCountBySite]
    @siteid INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT count = COUNT(*)
    FROM SiteOperatingDays
    WHERE SiteId = @siteid
      AND IsActive = 1
      AND (IsHoliday = 0 OR IsHoliday IS NULL);
END;
GO
