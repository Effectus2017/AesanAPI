CREATE OR ALTER PROCEDURE [dbo].[103_GetSchools]
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

    -- Retorna únicamente los campos necesarios para la tabla de escuelas
    SELECT
        s.Id,
        s.Name,
        s.Address,
        c.Name AS CityName,
        r.Name AS RegionName,
        s.IsMainSchool,
        s2.Name AS MainSchoolName
    FROM School s
        INNER JOIN City c ON s.CityId = c.Id
        INNER JOIN Region r ON s.RegionId = r.Id
        LEFT JOIN SchoolSatellite ss ON s.Id = ss.SatelliteSchoolId
        LEFT JOIN School s2 ON ss.MainSchoolId = s2.Id
    WHERE s.IsActive = 1
        AND (
            @alls = 1
        OR ((@name IS NULL OR s.Name LIKE '%' + @name + '%')
        AND (@cityId IS NULL OR s.CityId = @cityId)
        AND (@regionId IS NULL OR s.RegionId = @regionId)
        AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
            )
        )
    ORDER BY s.IsMainSchool DESC, s.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Retorna el conteo total para paginación
    SELECT COUNT(*)
    FROM School s
    WHERE s.IsActive = 1
        AND (
            @alls = 1
        OR ((@name IS NULL OR s.Name LIKE '%' + @name + '%')
        AND (@cityId IS NULL OR s.CityId = @cityId)
        AND (@regionId IS NULL OR s.RegionId = @regionId)
        AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
            )
        )
END;

-- Ejemplo de uso:
-- EXEC [103_GetSchools] @take = 10, @skip = 0, @name = NULL, @cityId = NULL, @regionId = NULL, @agencyId = NULL, @alls = 0;
