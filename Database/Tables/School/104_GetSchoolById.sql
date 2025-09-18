CREATE OR ALTER PROCEDURE [dbo].[104_GetSchoolById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener información principal de la escuela
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
        s.GeneralEnrollment,
        s.SiteNumber,
        s.IsMainSchool,
        a.AgencyCode,
        -- Generar código completo del sitio: extraer parte numérica del AgencyCode + SiteNumber
        CASE 
            WHEN a.AgencyCode IS NOT NULL AND s.SiteNumber IS NOT NULL THEN
                SUBSTRING(a.AgencyCode, CHARINDEX('-', a.AgencyCode) + 1, LEN(a.AgencyCode)) + '-' + CAST(s.SiteNumber AS VARCHAR(10))
            ELSE NULL
        END AS SiteCode,
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
        ss.AssignmentDate,
        ss.Comment,
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

    -- Obtener servicios de alimentación de la escuela
    SELECT
        ss.Id,
        ss.SchoolId,
        ss.ChildGroupId,
        cg.Name AS ChildGroupName,
        cg.NameEN AS ChildGroupNameEN,
        cg.OptionKey AS ChildGroupOptionKey,
        ss.Breakfast,
        ss.BreakfastFrom,
        ss.BreakfastTo,
        ss.Lunch,
        ss.LunchFrom,
        ss.LunchTo,
        ss.SnackAM,
        ss.SnackAMFrom,
        ss.SnackAMTo,
        ss.Dinner,
        ss.DinnerFrom,
        ss.DinnerTo,
        ss.SnackPM,
        ss.SnackPMFrom,
        ss.SnackPMTo,
        ss.SnackNight,
        ss.SnackNightFrom,
        ss.SnackNightTo,
        ss.CreatedAt,
        ss.UpdatedAt
    FROM SchoolService ss
        LEFT JOIN OptionSelection cg ON ss.ChildGroupId = cg.Id
    WHERE ss.SchoolId = @id;

    -- Obtener información específica de Day Care Home
    SELECT
        sdch.Id,
        sdch.SchoolId,
        sdch.IsAuthorizedToOperate,
        sdch.HasFamilyDepartmentLicense,
        sdch.NumberOfEnrolledChildren,
        sdch.NumberOfProviderChildren,
        sdch.NumberOfParticipantsWithBloodTies,
        sdch.NumberOfParticipantsWithoutBloodTies,
        sdch.MinorsLiveWithProvider,
        sdch.RelationshipTypeId,
        rt.Name AS RelationshipTypeName,
        rt.NameEN AS RelationshipTypeNameEN,
        rt.OptionKey AS RelationshipTypeOptionKey,
        sdch.OffersServiceToImmigrantChildren,
        sdch.HomeTypeId,
        ht.Name AS HomeTypeName,
        ht.NameEN AS HomeTypeNameEN,
        ht.OptionKey AS HomeTypeOptionKey,
        sdch.AdministratorAuthorizedName,
        sdch.AdministratorBirthDate,
        sdch.OffersServiceToDifferentGroups,
        sdch.CreatedAt,
        sdch.UpdatedAt
    FROM SchoolDayCareHome sdch
        LEFT JOIN OptionSelection rt ON sdch.RelationshipTypeId = rt.Id
        LEFT JOIN OptionSelection ht ON sdch.HomeTypeId = ht.Id
    WHERE sdch.SchoolId = @id;

    -- Obtener tipos de participantes de la escuela
    SELECT
        sp.Id,
        sp.SchoolId,
        sp.ParticipantTypeId,
        pt.Name AS ParticipantTypeName,
        pt.NameEN AS ParticipantTypeNameEN,
        pt.OptionKey AS ParticipantTypeOptionKey,
        sp.IsActive,
        sp.CreatedAt,
        sp.UpdatedAt
    FROM SchoolParticipant sp
        LEFT JOIN OptionSelection pt ON sp.ParticipantTypeId = pt.Id
    WHERE sp.SchoolId = @id AND sp.IsActive = 1;
END;

-- EXEC [104_GetSchoolById] @id = 3;
