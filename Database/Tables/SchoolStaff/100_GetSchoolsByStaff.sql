-- =============================================
-- Stored Procedure: 100_GetSchoolsByStaff
-- =============================================
-- Obtiene todos los sitios asignados a un empleado específico
-- Incluye información del sitio y tipo de asignación

CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolsByStaff]
    @staffId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ss.Id,
        ss.SchoolId,
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
        sch.Name AS SchoolName,
        sch.Address AS SchoolAddress,
        sch.CityId AS SchoolCityId,
        sch.RegionId AS SchoolRegionId,
        sch.ZipCode AS SchoolZipCode,
        sch.Latitude,
        sch.Longitude,
        sch.PostalAddress AS SchoolPostalAddress,
        sch.PostalCityId AS SchoolPostalCityId,
        sch.PostalRegionId AS SchoolPostalRegionId,
        sch.PostalZipCode AS SchoolPostalZipCode,
        sch.SameAsPhysicalAddress,
        sch.OrganizationTypeId,
        sch.CenterTypeId,
        sch.NonProfit,
        sch.BaseYear,
        sch.RenewalYear,
        sch.OperatingFromDate,
        sch.OperatingToDate,
        sch.OperatingDaysCalculated,
        sch.KitchenTypeId,
        sch.GroupTypeId,
        sch.DeliveryTypeId,
        sch.SponsorTypeId,
        sch.ApplicantTypeId,
        sch.ResidentialTypeId,
        sch.OperatingPolicyId,
        sch.AreaTypeId,
        sch.HasWarehouse,
        sch.HasDiningRoom,
        sch.SitePhone,
        sch.Extension,
        sch.MobilePhone,
        sch.CommunityId,
        sch.WalkersId,
        sch.SiteTypeId,
        sch.ExperienceId,
        sch.ReviewResultId,
        sch.ReviewDate,
        sch.ReviewJustification,
        sch.IsActive AS SchoolIsActive,
        sch.InactiveJustification,
        sch.InactiveDate,
        sch.CreatedAt AS SchoolCreatedAt,
        sch.UpdatedAt AS SchoolUpdatedAt,

        -- Información de la Asignación
        os.Name AS AssignmentTypeName,
        os.NameEN AS AssignmentTypeNameEn,

        -- Información de la agencia del sitio
        a.Name AS AgencyName,
        a.IsActive AS AgencyIsActive

    FROM SchoolStaff ss
        INNER JOIN School sch ON ss.SchoolId = sch.Id
        INNER JOIN OptionSelection os ON ss.AssignmentTypeId = os.Id
        INNER JOIN Agency a ON sch.AgencyId = a.Id

    WHERE ss.StaffId = @staffId
        AND ss.IsActive = 1
        AND sch.IsActive = 1
        AND a.IsActive = 1

    ORDER BY ss.IsPrimary DESC, ss.AssignmentDate DESC;
END
