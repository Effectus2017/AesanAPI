-- =============================================
-- Stored Procedure: 100_DeleteSiteChildGroupsBySiteId
-- Descripción: Elimina todos los grupos de niños de un sitio
-- Reemplaza: 100_DeleteSchoolChildGroupsBySchoolId
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteChildGroupsBySiteId]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SiteChildGroup WHERE SiteId = @siteId;

    -- Retornar el número de filas afectadas
    RETURN @@ROWCOUNT;
END;
