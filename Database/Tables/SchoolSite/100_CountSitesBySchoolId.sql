-- =============================================
-- Stored Procedure: 100_CountSitesBySchoolId
-- Descripción: Cuenta cuántos sitios activos tiene una escuela.
-- Usado para validar regla: el primer sitio de la escuela debe ser Comedor.
-- Fecha: 2026-02-13
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_CountSitesBySchoolId]
    @schoolId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT cnt = COUNT(*)
    FROM SchoolSite ss
        INNER JOIN Site s ON ss.SiteId = s.Id
    WHERE ss.SchoolId = @schoolId
        AND ss.IsActive = 1
        AND s.IsActive = 1;
END;
