CREATE OR ALTER PROCEDURE [dbo].[102_GetSchoolById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.Id,
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
        s.CenterId,
        ct.Name AS CenterName,
        s.NonProfit,
        s.BaseYear,
        s.RenewalYear,
        s.EducationLevelId,
        el.Name AS EducationLevelName,
        s.OperatingDays,
        s.KitchenTypeId,
        kt.Name AS KitchenTypeName,
        s.GroupTypeId,
        gt.Name AS GroupTypeName,
        s.DeliveryTypeId,
        dt.Name AS DeliveryTypeName,
        s.SponsorTypeId,
        st.Name AS SponsorTypeName,
        s.ApplicantTypeId,
        at.Name AS ApplicantTypeName,
        s.ResidentialTypeId,
        rt.Name AS ResidentialTypeName,
        s.OperatingPolicyId,
        opol.Name AS OperatingPolicyName,
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
        s.CreatedAt,
        s.UpdatedAt
    FROM School s
        LEFT JOIN City c ON s.CityId = c.Id
        LEFT JOIN Region r ON s.RegionId = r.Id
        LEFT JOIN City c2 ON s.PostalCityId = c2.Id
        LEFT JOIN Region r2 ON s.PostalRegionId = r2.Id
        LEFT JOIN OrganizationType ot ON s.OrganizationTypeId = ot.Id
        LEFT JOIN CenterType ct ON s.CenterId = ct.Id
        LEFT JOIN EducationLevel el ON s.EducationLevelId = el.Id
        LEFT JOIN KitchenType kt ON s.KitchenTypeId = kt.Id
        LEFT JOIN GroupType gt ON s.GroupTypeId = gt.Id
        LEFT JOIN DeliveryType dt ON s.DeliveryTypeId = dt.Id
        LEFT JOIN SponsorType st ON s.SponsorTypeId = st.Id
        LEFT JOIN ApplicantType at ON s.ApplicantTypeId = at.Id
        LEFT JOIN ResidentialType rt ON s.ResidentialTypeId = rt.Id
        LEFT JOIN OperatingPolicy opol ON s.OperatingPolicyId = opol.Id
    WHERE s.Id = @Id;

    -- Facilidades
    SELECT sf.FacilityId, f.Name AS FacilityName
    FROM SchoolFacility sf
        INNER JOIN Facility f ON sf.FacilityId = f.Id
    WHERE sf.SchoolId = @Id AND sf.IsActive = 1;

    -- Satélites
    SELECT ss.Id, ss.SatelliteSchoolId, s2.Name AS SatelliteSchoolName, ss.AssignmentDate, ss.Status, ss.Comment
    FROM SatelliteSchool ss
        INNER JOIN School s2 ON ss.SatelliteSchoolId = s2.Id
    WHERE ss.MainSchoolId = @Id AND ss.IsActive = 1;
END; 