CREATE OR ALTER PROCEDURE [dbo].[101_GetSchools]
    @take INT,
    @skip INT,
    @name NVARCHAR(255) = NULL,
    @alls BIT = 0
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
    WHERE (@alls = 1 OR s.IsActive = 1)
      AND (@name IS NULL OR s.Name LIKE '%' + @name + '%')
    ORDER BY s.Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM School s
    WHERE (@alls = 1 OR s.IsActive = 1)
      AND (@name IS NULL OR s.Name LIKE '%' + @name + '%');
END; 