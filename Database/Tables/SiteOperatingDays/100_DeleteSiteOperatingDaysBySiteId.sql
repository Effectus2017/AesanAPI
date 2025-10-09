-- =============================================
-- Stored Procedure: 100_DeleteSiteOperatingDaysBySiteId
-- Descripción: Elimina todos los días de funcionamiento de un sitio
-- Reemplaza: 100_DeleteSchoolOperatingDaysBySchoolId
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteOperatingDaysBySiteId]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SiteOperatingDays WHERE SiteId = @siteId;

    -- Retornar el número de filas afectadas
    RETURN @@ROWCOUNT;
END;
