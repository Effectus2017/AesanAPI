-- =============================================
-- Stored Procedure: 100_GetSiteChildGroupServicesBySiteId
-- Descripción: Obtiene todos los slots de servicios de los grupos de un sitio
-- Fecha: 2026-01-27
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteChildGroupServicesBySiteId]
    @siteid INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        scgs.Id AS id,
        scgs.ChildGroupId AS childgroupid,
        scgs.ServiceTypeId AS servicetypeid,
        scgs.IsOffered AS isoffered,
        scgs.FromTime AS fromtime,
        scgs.ToTime AS totime,
        scgs.CreatedAt AS createdat,
        scgs.UpdatedAt AS updatedat,
        st.Name AS servicetypename,
        st.NameEN AS servicetypenameen
    FROM SiteChildGroupService scgs
    INNER JOIN SiteChildGroup scg ON scgs.ChildGroupId = scg.Id
    INNER JOIN ServiceType st ON scgs.ServiceTypeId = st.Id
    WHERE scg.SiteId = @siteid
    ORDER BY scg.Id, scgs.ServiceTypeId;
END;
