CREATE OR ALTER PROCEDURE [dbo].[101_GetSchoolById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.Id,
        s.Name,
        s.StartDate,
        s.Address,
        s.PostalAddress,
        s.ZipCode,
        s.CityId,
        c.Name AS CityName,
        s.RegionId,
        r.Name AS RegionName,
        s.AreaCode,
        s.AdminFullName,
        s.Phone,
        s.PhoneExtension,
        s.Mobile,
        s.BaseYear,
        s.NextRenewalYear,
        s.OrganizationTypeId,
        ot.Name AS OrganizationTypeName,
        s.EducationLevelId,
        el.Name AS EducationLevelName,
        s.OperatingPeriodId,
        op.Name AS OperatingPeriodName,
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
        s.OperatingPolicyId,
        opol.Description AS OperatingPolicyDescription,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM School s
    LEFT JOIN City c ON s.CityId = c.Id
    LEFT JOIN Region r ON s.RegionId = r.Id
    LEFT JOIN OrganizationType ot ON s.OrganizationTypeId = ot.Id
    LEFT JOIN EducationLevel el ON s.EducationLevelId = el.Id
    LEFT JOIN OperatingPeriod op ON s.OperatingPeriodId = op.Id
    LEFT JOIN KitchenType kt ON s.KitchenTypeId = kt.Id
    LEFT JOIN GroupType gt ON s.GroupTypeId = gt.Id
    LEFT JOIN DeliveryType dt ON s.DeliveryTypeId = dt.Id
    LEFT JOIN SponsorType st ON s.SponsorTypeId = st.Id
    LEFT JOIN ApplicantType at ON s.ApplicantTypeId = at.Id
    LEFT JOIN OperatingPolicy opol ON s.OperatingPolicyId = opol.Id
    WHERE s.Id = @id;

    -- Facilidades
    SELECT sf.FacilityId, f.Name AS FacilityName
    FROM SchoolFacility sf
    INNER JOIN Facility f ON sf.FacilityId = f.Id
    WHERE sf.SchoolId = @id AND sf.IsActive = 1;

    -- Satélites
    SELECT ss.Id, ss.SatelliteSchoolId, s2.Name AS SatelliteSchoolName, ss.AssignmentDate, ss.Status, ss.Comment
    FROM SatelliteSchool ss
    INNER JOIN School s2 ON ss.SatelliteSchoolId = s2.Id
    WHERE ss.MainSchoolId = @id AND ss.IsActive = 1;
END; 