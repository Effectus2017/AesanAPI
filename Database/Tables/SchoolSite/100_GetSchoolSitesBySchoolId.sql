-- =============================================
-- Stored Procedure: 100_GetSchoolSitesBySchoolId
-- Descripción: Obtiene todos los Sites asignados a una School específica con paginación
-- Fecha: 2025-10-15
-- Versión: 1.1
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolSitesBySchoolId]
    @schoolId INT,
    @take INT = 50,
    @skip INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Retorna los datos con paginación
    SELECT
        ss.[Id],
        ss.[SchoolId],
        ss.[SiteId],
        ss.[AssignmentDate],
        ss.[Comment],
        ss.[IsActive],
        ss.[CreatedAt],
        ss.[UpdatedAt],
        s.[Name] AS SiteName,
        s.[SiteCode],
        s.[SiteNumber],
        s.[Address],
        s.[IsActive]
    FROM [SchoolSite] ss
        INNER JOIN [Site] s ON ss.[SiteId] = s.[Id]
    WHERE ss.[SchoolId] = @schoolId
        AND ss.[IsActive] = 1
    ORDER BY s.[Name] ASC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Retorna el conteo total para paginación
    SELECT COUNT(*)
    FROM [SchoolSite] ss
    WHERE ss.[SchoolId] = @schoolId
        AND ss.[IsActive] = 1;
END;
