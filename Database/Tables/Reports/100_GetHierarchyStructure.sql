-- =============================================
-- Stored Procedure: 100_GetHierarchyStructure
-- Descripción: Obtiene la estructura jerárquica completa para el árbol de jerarquía de escuelas
--              Devuelve Auspiciador → Año → Escuelas → Sitios
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetHierarchyStructure]
    @year INT,
    @sponsorId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Primer resultado: Obtener Auspiciador(es) Administrador(es)
    SELECT DISTINCT
        a.[Id],
        a.[Name],
        a.[AgencyCode] AS [Code]
    FROM [Agency] a
    WHERE a.[IsActive] = 1
        AND (@sponsorId IS NULL OR a.[Id] = @sponsorId)
    ORDER BY a.[Name];

    -- Segundo resultado: Obtener Escuelas con sus Sitios para el año especificado
    SELECT DISTINCT
        s.[Id],
        s.[Name],
        s.[SchoolCode],
        s.[SchoolNumber],
        s.[AgencyId],
        st.[Id] AS [SiteId],
        st.[Name] AS [SiteName],
        st.[SiteNumber],
        st.[SiteCode],
        st.[IsActive] AS [IsActive]
    FROM [School] s
        INNER JOIN [Agency] a ON s.[AgencyId] = a.[Id]
        LEFT JOIN [SchoolSite] ss ON s.[Id] = ss.[SchoolId] AND ss.[IsActive] = 1
        LEFT JOIN [Site] st ON ss.[SiteId] = st.[Id]
    WHERE s.[IsActive] = 1
        AND a.[IsActive] = 1
        AND (@sponsorId IS NULL OR a.[Id] = @sponsorId)
        AND (
            -- Filtrar por año: considerar sitios que tengan baseYear o renewalYear igual al año
            st.[Id] IS NULL
        OR st.[BaseYear] = @year
        OR st.[RenewalYear] = @year
        OR (st.[BaseYear] IS NULL AND st.[RenewalYear] IS NULL)
        )
    ORDER BY s.[Name], st.[SiteNumber];
END;

-- Ejemplo de uso:
-- EXEC [dbo].[100_GetHierarchyStructure] @year = 2025, @sponsorId = NULL;
-- EXEC [dbo].[100_GetHierarchyStructure] @year = 2025, @sponsorId = 1;

