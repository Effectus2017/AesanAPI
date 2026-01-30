-- =============================================
-- Stored Procedure: 100_DeleteSiteChildGroupServicesByChildGroupId
-- Descripción: Elimina todos los slots de servicios de un grupo
-- Fecha: 2026-01-27
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteChildGroupServicesByChildGroupId]
    @childgroupid INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SiteChildGroupService
    WHERE ChildGroupId = @childgroupid;
END;
