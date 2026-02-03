-- =============================================
-- Stored Procedure: 101_DeleteSiteChildGroupsBySiteId
-- Descripción: Elimina todos los grupos de niños de un sitio.
--              Primero elimina registros en SiteOperatingDayService que referencian
--              esos grupos (FK_SiteOperatingDayService_ChildGroup) para evitar
--              violación de FK al borrar SiteChildGroup.
-- Versión: 1.1 (sobre 100_DeleteSiteChildGroupsBySiteId)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteChildGroupsBySiteId]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Eliminar servicios por día que referencian los ChildGroups del sitio
    DELETE FROM SiteOperatingDayService
    WHERE ChildGroupId IN (SELECT Id FROM SiteChildGroup WHERE SiteId = @siteId);

    -- Eliminar grupos de niños del sitio
    DELETE FROM SiteChildGroup WHERE SiteId = @siteId;

    RETURN @@ROWCOUNT;
END;
