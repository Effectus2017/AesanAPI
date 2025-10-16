-- =============================================
-- Stored Procedure: 100_GetSchoolSiteBySiteId
-- Descripción: Obtiene la School asignada a un Site específico
-- Fecha: 2025-10-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolSiteBySiteId]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ss.[Id],
        ss.[SchoolId],
        ss.[SiteId],
        ss.[AssignmentDate],
        ss.[Comment],
        ss.[IsActive],
        ss.[CreatedAt],
        ss.[UpdatedAt],
        s.[Name] AS SchoolName,
        s.[SchoolCode],
        s.[SchoolNumber],
        s.[IsActive] AS SchoolIsActive,
        a.[Name] AS AgencyName,
        a.[AgencyCode] AS AgencyCode
    FROM [SchoolSite] ss
        INNER JOIN [School] s ON ss.[SchoolId] = s.[Id]
        INNER JOIN [Agency] a ON s.[AgencyId] = a.[Id]
    WHERE ss.[SiteId] = @siteId
        AND ss.[IsActive] = 1;
END;
