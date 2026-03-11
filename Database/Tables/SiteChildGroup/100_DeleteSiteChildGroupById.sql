-- =============================================
-- Stored Procedure: 100_DeleteSiteChildGroupById
-- Descripción: Elimina un grupo de niños por Id.
--              Primero elimina SiteOperatingDayService que referencia al grupo (FK);
--              SiteChildGroupService se elimina por CASCADE al borrar SiteChildGroup.
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteChildGroupById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SiteOperatingDayService
    WHERE ChildGroupId = @id;

    DELETE FROM SiteChildGroup
    WHERE Id = @id;
END;
