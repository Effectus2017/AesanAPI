CREATE OR ALTER PROCEDURE [dbo].[102_GetSchools]
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

    SELECT
        s.Id,
        s.AgencyId,
        a.Name AS AgencyName,
        s.Name,
        s.StartDate,
        s.Address,
        s.CityId,
        c.Name AS CityName,
        s.RegionId,
        r.Name AS RegionName,
        s.ZipCode,
        s.Latitude,
        s.Longitude,
        s.PostalAddress,
        s.PostalCityId,
        ps.Name AS PostalCityName,
        s.PostalRegionId,
        pr.Name AS PostalRegionName,
        s.PostalZipCode,
        s.SameAsPhysicalAddress,
        s.OrganizationTypeId,
        ot.Name AS OrganizationTypeName,
        s.CenterTypeId,
        ct.Name AS CenterTypeName,
        s.NonProfit,
        s.BaseYear,
        s.RenewalYear,
        s.EducationLevelId,
        --el.Name AS EducationLevelName,
        s.OperatingDays,
        s.KitchenTypeId,
        --kt.Name AS KitchenTypeName,
        s.GroupTypeId,
        --gt.Name AS GroupTypeName,
        s.DeliveryTypeId,
        --dt.Name AS DeliveryTypeName,
        s.SponsorTypeId,
        --st.Name AS SponsorTypeName,
        s.ApplicantTypeId,
        --at.Name AS ApplicantTypeName,
        s.ResidentialTypeId,
        --rt.Name AS ResidentialTypeName,
        s.OperatingPolicyId,
        --opol.Name AS OperatingPolicyName,
        s.HasWarehouse,
        s.HasDiningRoom,
        s.AdministratorAuthorizedName,
        s.SitePhone,
        s.Extension,
        s.MobilePhone,
        s.Breakfast,
        s.BreakfastFrom,
        s.BreakfastTo,
        s.Lunch,
        s.LunchFrom,
        s.LunchTo,
        s.Snack,
        s.SnackFrom,
        s.SnackTo,
        s.IsActive,
        s.IsMainSchool,
        s.CreatedAt,
        s.UpdatedAt
    FROM School s
        INNER JOIN City c ON s.CityId = c.Id
        INNER JOIN Region r ON s.RegionId = r.Id
        LEFT JOIN City ps ON s.PostalCityId = ps.Id
        LEFT JOIN Region pr ON s.PostalRegionId = pr.Id
        LEFT JOIN OrganizationType ot ON s.OrganizationTypeId = ot.Id
        LEFT JOIN CenterType ct ON s.CenterTypeId = ct.Id
        LEFT JOIN EducationLevel el ON s.EducationLevelId = el.Id
        LEFT JOIN KitchenType kt ON s.KitchenTypeId = kt.Id
        LEFT JOIN GroupType gt ON s.GroupTypeId = gt.Id
        LEFT JOIN DeliveryType dt ON s.DeliveryTypeId = dt.Id
        LEFT JOIN SponsorType st ON s.SponsorTypeId = st.Id
        LEFT JOIN ApplicantType at ON s.ApplicantTypeId = at.Id
        LEFT JOIN ResidentialType rt ON s.ResidentialTypeId = rt.Id
        LEFT JOIN OperatingPolicy opol ON s.OperatingPolicyId = opol.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
    WHERE s.IsActive = 1
        AND (
            @alls = 1
        OR ((@name IS NULL OR s.Name LIKE '%' + @name + '%')
        AND (@cityId IS NULL OR s.CityId = @cityId)
        AND (@regionId IS NULL OR s.RegionId = @regionId)
        AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
            )
        )
    ORDER BY s.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM School s
    WHERE s.IsActive = 1
        AND (
            @alls = 1
        OR ((@name IS NULL OR s.Name LIKE '%' + @name + '%')
        AND (@cityId IS NULL OR s.CityId = @cityId)
        AND (@regionId IS NULL OR s.RegionId = @regionId)
            )
        )
END;