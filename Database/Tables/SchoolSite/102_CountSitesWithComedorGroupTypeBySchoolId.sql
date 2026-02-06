-- =============================================
-- Stored Procedure: 102_CountSitesWithComedorGroupTypeBySchoolId
-- Descripción: Cuenta cuántos sitios con tipo de grupo Comedor tiene una escuela.
-- Usado para validar regla: solo un sitio Comedor por escuela.
-- Fecha: 2026-02-05
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[102_CountSitesWithComedorGroupTypeBySchoolId]
    @schoolId INT,
    @excludeSiteId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @comedorId INT = (
        SELECT Id FROM GroupType
        WHERE Name = N'Comedor' OR NameEN = N'Dining Room'
    );

    SELECT cnt = COUNT(*)
    FROM Site s
        INNER JOIN SchoolSite ss ON s.Id = ss.SiteId
    WHERE ss.SchoolId = @schoolId
        AND ss.IsActive = 1
        AND s.GroupTypeId = @comedorId
        AND (@excludeSiteId IS NULL OR s.Id != @excludeSiteId);
END;
