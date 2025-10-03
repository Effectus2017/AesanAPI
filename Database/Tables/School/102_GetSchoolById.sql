CREATE OR ALTER PROCEDURE [dbo].[102_GetSchoolById]
    @id INT
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
        c2.Name AS PostalCityName,
        s.PostalRegionId,
        r2.Name AS PostalRegionName,
        s.PostalZipCode,
        s.SameAsPhysicalAddress,
        s.OrganizationTypeId,
        ot.Name AS OrganizationTypeName,
        ot.NameEN AS OrganizationTypeNameEN,
        s.CenterTypeId,
        ct.Name AS CenterName,
        ct.NameEN AS CenterNameEN,
        s.NonProfit,
        s.BaseYear,
        s.RenewalYear,
        s.OperatingFromDate,
        s.OperatingToDate,
        s.OperatingDaysCalculated,
        s.ServiceTime,
        s.KitchenTypeId,
        kt.Name AS KitchenTypeName,
        kt.NameEN AS KitchenTypeNameEN,
        s.GroupTypeId,
        gt.Name AS GroupTypeName,
        gt.NameEN AS GroupTypeNameEN,
        s.DeliveryTypeId,
        dt.Name AS DeliveryTypeName,
        dt.NameEN AS DeliveryTypeNameEN,
        s.SponsorTypeId,
        st.Name AS SponsorTypeName,
        st.NameEN AS SponsorTypeNameEN,
        s.ApplicantTypeId,
        at.Name AS ApplicantTypeName,
        at.NameEN AS ApplicantTypeNameEN,
        s.ResidentialTypeId,
        rt.Name AS ResidentialTypeName,
        rt.NameEN AS ResidentialTypeNameEN,
        s.OperatingPolicyId,
        opol.Name AS OperatingPolicyName,
        opol.NameEN AS OperatingPolicyNameEN,
        s.AreaTypeId,
        atype.Name AS AreaTypeName,
        atype.NameEN AS AreaTypeNameEN,
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
        s.Dinner,
        s.DinnerFrom,
        s.DinnerTo,
        s.SnackNight,
        s.SnackNightFrom,
        s.SnackNightTo,
        s.CommunityId,
        s.WalkersId,
        s.SiteTypeId,
        s.ExperienceId,
        s.ReviewResultId,
        s.ReviewDate,
        s.ReviewJustification,
        s.IsActive,
        s.InactiveJustification,
        s.InactiveDate,
        s.CreatedAt,
        s.UpdatedAt
    FROM School s
        LEFT JOIN Agency a ON s.AgencyId = a.Id
        LEFT JOIN City c ON s.CityId = c.Id
        LEFT JOIN Region r ON s.RegionId = r.Id
        LEFT JOIN City c2 ON s.PostalCityId = c2.Id
        LEFT JOIN Region r2 ON s.PostalRegionId = r2.Id
        LEFT JOIN OrganizationType ot ON s.OrganizationTypeId = ot.Id
        LEFT JOIN CenterType ct ON s.CenterTypeId = ct.Id
        LEFT JOIN KitchenType kt ON s.KitchenTypeId = kt.Id
        LEFT JOIN GroupType gt ON s.GroupTypeId = gt.Id
        LEFT JOIN DeliveryType dt ON s.DeliveryTypeId = dt.Id
        LEFT JOIN SponsorType st ON s.SponsorTypeId = st.Id
        LEFT JOIN OptionSelection at ON s.ApplicantTypeId = at.Id
        LEFT JOIN OptionSelection rt ON s.ResidentialTypeId = rt.Id
        LEFT JOIN OperatingPolicy opol ON s.OperatingPolicyId = opol.Id
        LEFT JOIN AreaType atype ON s.AreaTypeId = atype.Id
    WHERE s.Id = @id;

    -- Obtener satélites de la escuela
    SELECT
        ss.Id,
        ss.MainSchoolId,
        ss.SatelliteSchoolId,
        s.Name AS SatelliteSchoolName,
        ss.IsActive,
        ss.CreatedAt,
        ss.UpdatedAt
    FROM SchoolSatellite ss
        LEFT JOIN School s ON ss.SatelliteSchoolId = s.Id
    WHERE ss.MainSchoolId = @id AND ss.IsActive = 1;

    -- Obtener niveles educativos de la escuela
    SELECT
        sel.Id,
        sel.SchoolId,
        sel.EducationLevelId,
        el.Name AS EducationLevelName,
        el.NameEN AS EducationLevelNameEN,
        sel.IsActive,
        sel.CreatedAt,
        sel.UpdatedAt
    FROM SchoolEducationLevel sel
        LEFT JOIN EducationLevel el ON sel.EducationLevelId = el.Id
    WHERE sel.SchoolId = @id AND sel.IsActive = 1;
END;


-- EXEC [102_GetSchoolById] @id = 3;