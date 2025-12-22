-- =============================================
-- Stored Procedure: 105_GetSiteById
-- Descripción: Obtiene un sitio por su ID con toda la información relacionada
-- Reemplaza: 105_GetSchoolById
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[105_GetSiteById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener información principal del sitio
    SELECT
        s.Id, s.AgencyId, a.Name AS AgencyName, s.Name, s.StartDate, s.Address,
        s.CityId, c.Name AS CityName, s.RegionId, r.Name AS RegionName, s.ZipCode,
        s.Latitude, s.Longitude, s.PostalAddress, s.PostalCityId, c2.Name AS PostalCityName,
        s.PostalRegionId, r2.Name AS PostalRegionName, s.PostalZipCode, s.SameAsPhysicalAddress,
        s.OrganizationTypeId, ot.Name AS OrganizationTypeName, ot.NameEN AS OrganizationTypeNameEN, ot.RequiresCenterType AS OrganizationTypeRequiresCenterType,
        s.CenterTypeId, ct.Name AS CenterName, ct.NameEN AS CenterNameEN, s.NonProfit,
        s.BaseYear, s.RenewalYear, s.OperatingFromDate, s.OperatingToDate, s.OperatingDaysCalculated,
        s.OperatingStartTime, s.OperatingEndTime, s.ServiceTime, s.KitchenTypeId, kt.Name AS KitchenTypeName, kt.NameEN AS KitchenTypeNameEN,
        s.GroupTypeId, gt.Name AS GroupTypeName, gt.NameEN AS GroupTypeNameEN, s.DeliveryTypeId,
        dt.Name AS DeliveryTypeName, dt.NameEN AS DeliveryTypeNameEN, s.SponsorTypeId,
        st.Name AS SponsorTypeName, st.NameEN AS SponsorTypeNameEN, s.ApplicantTypeId,
        at.Name AS ApplicantTypeName, at.NameEN AS ApplicantTypeNameEN, s.ResidentialTypeId,
        rt.Name AS ResidentialTypeName, rt.NameEN AS ResidentialTypeNameEN, s.OperatingPolicyId,
        opol.Name AS OperatingPolicyName, opol.NameEN AS OperatingPolicyNameEN, s.AreaTypeId,
        atype.Name AS AreaTypeName, atype.NameEN AS AreaTypeNameEN, s.LocationTypeId,
        ltype.Name AS LocationTypeName, ltype.NameEN AS LocationTypeNameEN, s.HasWarehouse,
        s.HasDiningRoom, s.CommunityId, s.WalkersId,
        spic.FirstName AS PersonInChargeFirstName, spic.MiddleName AS PersonInChargeMiddleName, 
        spic.FatherLastName AS PersonInChargeFatherLastName, spic.MotherLastName AS PersonInChargeMotherLastName,
        spic.SitePhone AS PersonInChargeSitePhone, spic.Extension AS PersonInChargeExtension, 
        spic.MobilePhone AS PersonInChargeMobilePhone,
        s.SiteTypeId, s.SiteLocationId, sl.Name AS SiteLocationName, sl.NameEN AS SiteLocationNameEN, sl.OptionKey AS SiteLocationOptionKey, s.ExperienceId, s.ReviewResultId, s.ReviewDate, s.ReviewJustification,
        s.IsActive, s.InactiveJustification, s.InactiveDate, s.GeneralEnrollment, s.SiteNumber,
        s.OrganizedAthleticPrograms, s.AtRiskService, s.PublicAllianceContractId, pac.Name AS PublicAllianceContractName, pac.NameEN AS PublicAllianceContractNameEN, s.IsAffiliatedCenter, s.IsDayCareHomeId, os_idch.Name AS IsDayCareHomeName, os_idch.NameEN AS IsDayCareHomeNameEN, os_idch.OptionKey AS IsDayCareHomeOptionKey, os_idch.BooleanValue AS IsDayCareHomeBooleanValue, s.IsMainSite, a.AgencyCode,
        s.SiteCode,
        s.CreatedAt, s.UpdatedAt
    FROM Site s
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
        LEFT JOIN AreaType ltype ON s.LocationTypeId = ltype.Id
        LEFT JOIN OptionSelection sl ON s.SiteLocationId = sl.Id
        LEFT JOIN OptionSelection pac ON s.PublicAllianceContractId = pac.Id
        LEFT JOIN OptionSelection os_idch ON s.IsDayCareHomeId = os_idch.Id
        LEFT JOIN SitePersonInCharge spic ON s.Id = spic.SiteId
    WHERE s.Id = @id;

    -- Obtener niveles educativos del sitio
    SELECT sel.Id, sel.SiteId, sel.EducationLevelId, el.Name AS EducationLevelName,
        el.NameEN AS EducationLevelNameEN, sel.IsActive, sel.CreatedAt, sel.UpdatedAt
    FROM SiteEducationLevel sel
        LEFT JOIN EducationLevel el ON sel.EducationLevelId = el.Id
    WHERE sel.SiteId = @id AND sel.IsActive = 1;

    -- Obtener días de la semana de operación del sitio
    SELECT sodow.DayOfWeek AS DayOfWeekId, dow.Name AS DayOfWeekName, dow.NameEN AS DayOfWeekNameEN
    FROM SiteOperatingDaysOfWeek sodow
        LEFT JOIN DayOfWeek dow ON sodow.DayOfWeek = dow.Id
    WHERE sodow.SiteId = @id AND sodow.IsActive = 1
    ORDER BY sodow.DayOfWeek;

    -- Obtener servicios de alimentación del sitio
    SELECT ss.Id, ss.SiteId, ss.ChildGroupId, cg.Name AS ChildGroupName,
        cg.OptionKey AS ChildGroupOptionKey, ss.Breakfast, ss.BreakfastFrom, ss.BreakfastTo,
        ss.Lunch, ss.LunchFrom, ss.LunchTo, ss.SnackAM, ss.SnackAMFrom, ss.SnackAMTo,
        ss.Dinner, ss.DinnerFrom, ss.DinnerTo, ss.SnackPM, ss.SnackPMFrom, ss.SnackPMTo,
        ss.SnackNight, ss.SnackNightFrom, ss.SnackNightTo, ss.DinnerExtended, ss.DinnerExtendedFrom,
        ss.DinnerExtendedTo, ss.DinnerAtRisk, ss.DinnerAtRiskFrom, ss.DinnerAtRiskTo,
        ss.SnackExtended, ss.SnackExtendedFrom, ss.SnackExtendedTo, ss.SnackAtRisk,
        ss.SnackAtRiskFrom, ss.SnackAtRiskTo, ss.CreatedAt, ss.UpdatedAt
    FROM SiteService ss
        LEFT JOIN OptionSelection cg ON ss.ChildGroupId = cg.Id
    WHERE ss.SiteId = @id;

    -- Obtener información específica de Day Care Home
    SELECT sdch.Id, sdch.SiteId, sdch.IsAuthorizedToOperate, sdch.HasFamilyDepartmentLicense,
        sdch.NumberOfEnrolledChildren, sdch.NumberOfProviderChildren, sdch.NumberOfParticipantsWithBloodTies,
        sdch.NumberOfParticipantsWithoutBloodTies, sdch.MinorsLiveWithProvider, sdch.RelationshipTypeId,
        rt.Name AS RelationshipTypeName, rt.NameEN AS RelationshipTypeNameEN, rt.OptionKey AS RelationshipTypeOptionKey,
        sdch.OffersServiceToImmigrantChildren, sdch.HomeTypeId, ht.Name AS HomeTypeName,
        ht.NameEN AS HomeTypeNameEN, ht.OptionKey AS HomeTypeOptionKey, sdch.AdministratorAuthorizedName,
        sdch.AdministratorBirthDate, sdch.OffersServiceToDifferentGroups, sdch.CreatedAt, sdch.UpdatedAt
    FROM SiteDayCareHome sdch
        LEFT JOIN OptionSelection rt ON sdch.RelationshipTypeId = rt.Id
        LEFT JOIN OptionSelection ht ON sdch.HomeTypeId = ht.Id
    WHERE sdch.SiteId = @id;

    -- Obtener tipos de participantes del sitio
    SELECT sp.Id, sp.SiteId, sp.ParticipantTypeId, pt.Name AS ParticipantTypeName,
        pt.NameEN AS ParticipantTypeNameEN, pt.OptionKey AS ParticipantTypeOptionKey,
        sp.IsActive, sp.CreatedAt, sp.UpdatedAt
    FROM SiteParticipant sp
        LEFT JOIN OptionSelection pt ON sp.ParticipantTypeId = pt.Id
    WHERE sp.SiteId = @id AND sp.IsActive = 1;

    -- Obtener grupos de niños específicos del sitio
    SELECT scg.Id, scg.SiteId, scg.GroupName, scg.NumberOfChildren, scg.CreatedAt, scg.UpdatedAt
    FROM SiteChildGroup scg
    WHERE scg.SiteId = @id
    ORDER BY scg.GroupName;
END;

--EXEC [105_GetSiteById] @id = 1; 