-- =============================================
-- Stored Procedure: 105_GetSites
-- Descripción: Obtiene todos los sitios con paginación y filtros.
--              SiteCode mostrado como XXX-XX-X (agencia-escuela-ordinal por escuela).
-- Fecha: 2026-03-13
-- Versión: 2.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[105_GetSites]
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

    ;WITH Ranked AS (
        SELECT
            s.Id AS SiteId,
            ss.SchoolId,
            ROW_NUMBER() OVER (PARTITION BY ss.SchoolId ORDER BY s.SiteNumber, s.Id) AS SiteOrdinalInSchool
        FROM Site s
        LEFT JOIN SchoolSite ss ON s.Id = ss.SiteId AND ss.IsActive = 1
        WHERE s.IsActive = 1
    )
    SELECT
        s.Id,
        s.Name,
        s.Address,
        CityName = c.Name,
        RegionName = r.Name,
        SchoolName = sch.Name,
        generalenrollment = ISNULL(s.GeneralEnrollment, (SELECT ISNULL(SUM(scg.NumberOfChildren), 0) FROM SiteChildGroup scg WHERE scg.SiteId = s.Id)),
        s.IsActive,
        SiteCode = CASE
            WHEN a.AgencyCode IS NOT NULL AND sch.SchoolCode IS NOT NULL AND s.SiteCode IS NOT NULL AND rnk.SchoolId IS NOT NULL
            THEN RIGHT(a.AgencyCode, 3) + '-' + sch.SchoolCode + '-' + CAST(rnk.SiteOrdinalInSchool AS VARCHAR(10))
            ELSE s.SiteCode
        END
    FROM Site s
        INNER JOIN City c ON s.CityId = c.Id
        INNER JOIN Region r ON s.RegionId = r.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
        LEFT JOIN GroupType gt ON s.GroupTypeId = gt.Id
        LEFT JOIN SchoolSite ss ON s.Id = ss.SiteId AND ss.IsActive = 1
        LEFT JOIN School sch ON ss.SchoolId = sch.Id AND sch.IsActive = 1
        LEFT JOIN Ranked rnk ON s.Id = rnk.SiteId AND (ss.SchoolId = rnk.SchoolId OR (ss.SchoolId IS NULL AND rnk.SchoolId IS NULL))
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
        );
END;
