-- =============================================
-- Stored Procedure: 104_GetSites
-- Descripción: Obtiene todos los sitios con paginación y filtros
-- Reemplaza: 104_GetSchools
-- Fecha: 2025-01-15
-- Versión: 1.2
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[104_GetSites]
    @take INT,
    @skip INT,
    @name NVARCHAR(255) = NULL,
    @cityId INT = NULL,
    @regionId INT = NULL,
    @agencyId INT = NULL,
    @alls BIT = 0,
    @isDayCareHomeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Retorna únicamente los campos necesarios para la tabla de sitios
    SELECT
        s.Id, s.Name, s.Address, c.Name AS CityName, r.Name AS RegionName,
        s.GeneralEnrollment AS generalenrollment, s.SiteNumber, s.IsActive,
        a.AgencyCode, gt.Name AS GroupTypeName,
        -- Formatear SiteCode como XXX-XX-X (últimos 3 dígitos de agencia - código de escuela - código del sitio)
        CASE 
            WHEN a.AgencyCode IS NOT NULL AND sch.SchoolCode IS NOT NULL AND s.SiteCode IS NOT NULL
            THEN RIGHT(a.AgencyCode, 3) + '-' + sch.SchoolCode + '-' + 
                 SUBSTRING(s.SiteCode, CHARINDEX('-', s.SiteCode) + 1, LEN(s.SiteCode))
            ELSE s.SiteCode
        END AS SiteCode,
        -- Información de la escuela relacionada
        sch.Name AS SchoolName,
        sch.Id AS SchoolId,
        sch.SchoolCode AS SchoolCode
    FROM Site s
        INNER JOIN City c ON s.CityId = c.Id
        INNER JOIN Region r ON s.RegionId = r.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
        LEFT JOIN GroupType gt ON s.GroupTypeId = gt.Id
        LEFT JOIN SchoolSite ss ON s.Id = ss.SiteId AND ss.IsActive = 1
        LEFT JOIN School sch ON ss.SchoolId = sch.Id AND sch.IsActive = 1
    WHERE s.IsActive = 1
        AND (
            @alls = 1
        OR (
                (@name IS NULL OR s.Name LIKE '%' + @name + '%')
        AND (@cityId IS NULL OR s.CityId = @cityId)
        AND (@regionId IS NULL OR s.RegionId = @regionId)
        AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
        AND (@isDayCareHomeId IS NULL OR s.IsDayCareHomeId = @isDayCareHomeId)
                )
            )
    ORDER BY s.SiteNumber DESC, s.Name
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Retorna el conteo total para paginación
    SELECT COUNT(*)
    FROM Site s
    WHERE s.IsActive = 1
        AND (
            @alls = 1
        OR (
                (@name IS NULL OR s.Name LIKE '%' + @name + '%')
        AND (@cityId IS NULL OR s.CityId = @cityId)
        AND (@regionId IS NULL OR s.RegionId = @regionId)
        AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
        AND (@isDayCareHomeId IS NULL OR s.IsDayCareHomeId = @isDayCareHomeId)
                )
            )
END;

--EXEC [104_GetSites] @take = 10, @skip = 0, @name = NULL, @cityId = NULL, @regionId = NULL, @agencyId = NULL, @alls = 0, @isDayCareHomeId = 1;