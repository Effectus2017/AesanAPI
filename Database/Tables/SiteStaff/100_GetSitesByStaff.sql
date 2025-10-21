-- =============================================
-- Stored Procedure: 100_GetSitesByStaff
-- Descripción: Obtiene todos los sitios asignados a un empleado específico
-- Reemplaza: 100_GetSchoolsByStaff
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSitesByStaff]
    @staffId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ss.Id,
        ss.SiteId,
        ss.StaffId,
        ss.AssignmentDate,
        ss.AssignmentTypeId,
        ss.IsPrimary,
        ss.StartDate,
        ss.EndDate,
        ss.Comments,
        ss.IsActive,
        ss.CreatedAt,
        ss.UpdatedAt,

        -- Información del Sitio
        site.Name AS SiteName,
        site.Address AS SiteAddress,
        site.CityId AS SiteCityId,
        site.RegionId AS SiteRegionId,
        site.ZipCode AS SiteZipCode,
        site.Latitude,
        site.Longitude,
        site.PostalAddress AS SitePostalAddress,
        site.PostalCityId AS SitePostalCityId,
        site.PostalRegionId AS SitePostalRegionId,
        site.PostalZipCode AS SitePostalZipCode,
        site.SameAsPhysicalAddress,
        site.OrganizationTypeId,
        site.CenterTypeId,
        site.NonProfit,
        site.BaseYear,
        site.RenewalYear,
        site.OperatingFromDate,
        site.OperatingToDate,
        site.OperatingDaysCalculated,
        site.KitchenTypeId,
        site.GroupTypeId,
        site.DeliveryTypeId,
        site.SponsorTypeId,
        site.ApplicantTypeId,
        site.ResidentialTypeId,
        site.OperatingPolicyId,
        site.AreaTypeId,
        site.LocationTypeId,
        site.HasWarehouse,
        site.HasDiningRoom,
        site.AdministratorAuthorizedName,
        site.SitePhone,
        site.Extension,
        site.MobilePhone,
        site.CommunityId,
        site.WalkersId,
        site.SiteTypeId,
        site.ExperienceId,
        site.ReviewResultId,
        site.ReviewDate,
        site.ReviewJustification,
        site.IsActive AS SiteIsActive,
        site.InactiveJustification,
        site.InactiveDate,
        site.CreatedAt AS SiteCreatedAt,
        site.UpdatedAt AS SiteUpdatedAt,

        -- Información de la Asignación
        os.Name AS AssignmentTypeName,
        os.NameEN AS AssignmentTypeNameEn,

        -- Información de la agencia del sitio
        a.Name AS AgencyName,
        a.IsActive AS AgencyIsActive

    FROM SiteStaff ss
        INNER JOIN Site site ON ss.SiteId = site.Id
        INNER JOIN OptionSelection os ON ss.AssignmentTypeId = os.Id
        INNER JOIN Agency a ON site.AgencyId = a.Id

    WHERE ss.StaffId = @staffId
        AND ss.IsActive = 1
        AND site.IsActive = 1
        AND a.IsActive = 1

    ORDER BY ss.IsPrimary DESC, ss.AssignmentDate DESC;
END
