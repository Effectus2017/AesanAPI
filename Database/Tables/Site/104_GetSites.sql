-- =============================================
-- Stored Procedure: 104_GetSites
-- Descripción: Obtiene todos los sitios con paginación y filtros
-- Reemplaza: 104_GetSchools
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[104_GetSites]
    @take INT,
    @skip INT,
    @name NVARCHAR(255) = NULL,
    @cityId INT = NULL,
    @regionId INT = NULL,
    @agencyId INT = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Retorna únicamente los campos necesarios para la tabla de sitios
    SELECT
        s.Id, s.Name, s.Address, c.Name AS CityName, r.Name AS RegionName,
        s.IsMainSite, s2.Name AS MainSiteName, s.GeneralEnrollment, s.SiteNumber,
        a.AgencyCode,
        -- Generar código completo del sitio: extraer parte numérica del AgencyCode + SiteNumber
        CASE 
            WHEN a.AgencyCode IS NOT NULL AND s.SiteNumber IS NOT NULL THEN
                SUBSTRING(a.AgencyCode, CHARINDEX('-', a.AgencyCode) + 1, LEN(a.AgencyCode)) + '-' + CAST(s.SiteNumber AS VARCHAR(10))
            ELSE NULL
        END AS SiteCode
    FROM Site s
        INNER JOIN City c ON s.CityId = c.Id
        INNER JOIN Region r ON s.RegionId = r.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
        LEFT JOIN SiteSatellite ss ON s.Id = ss.SatelliteSiteId
        LEFT JOIN Site s2 ON ss.MainSiteId = s2.Id
    WHERE s.IsActive = 1
        AND (
            @alls = 1
        OR ((@name IS NULL OR s.Name LIKE '%' + @name + '%')
        AND (@cityId IS NULL OR s.CityId = @cityId)
        AND (@regionId IS NULL OR s.RegionId = @regionId)
        AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
            )
        )
    ORDER BY s.IsMainSite DESC, s.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Retorna el conteo total para paginación
    SELECT COUNT(*)
    FROM Site s
    WHERE s.IsActive = 1
        AND (
            @alls = 1
        OR ((@name IS NULL OR s.Name LIKE '%' + @name + '%')
        AND (@cityId IS NULL OR s.CityId = @cityId)
        AND (@regionId IS NULL OR s.RegionId = @regionId)
        AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
            )
        );
END;
