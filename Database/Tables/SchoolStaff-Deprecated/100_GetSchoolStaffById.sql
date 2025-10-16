-- =============================================
-- DEPRECATED: Este SP ha sido reemplazado por 100_GetSiteStaffById
-- Fecha de deprecación: 2025-01-15
-- Razón: Migración de School a Site
-- =============================================
-- Stored Procedure: 100_GetSchoolStaffById
-- =============================================
-- Obtiene una asignación específica por su ID
-- Incluye información completa del staff, sitio y asignación

CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolStaffById]
    @id INT
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

        -- Información del Staff
        s.FirstName AS StaffFirstName,
        s.MiddleName AS StaffMiddleName,
        s.FatherLastName AS StaffFatherLastName,
        s.MotherLastName AS StaffMotherLastName,
        s.Email AS StaffEmail,
        s.PositionId AS StaffPositionId,
        s.StaffTypeId,
        s.ContractStartDate,
        s.ContractEndDate,
        s.BirthDate,
        s.PostalAddress,
        s.CityId AS StaffCityId,
        s.RegionId AS StaffRegionId,
        s.AreaCode,
        s.Comments AS StaffComments,
        s.UserId AS StaffUserId,
        s.IsActive AS StaffIsActive,

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
        sch.AdministratorAuthorizedName,
        sch.SitePhone,
        sch.Extension,
        sch.MobilePhone,
        sch.Breakfast,
        sch.BreakfastFrom,
        sch.BreakfastTo,
        sch.Lunch,
        sch.LunchFrom,
        sch.LunchTo,
        sch.Snack,
        sch.SnackFrom,
        sch.SnackTo,
        sch.Dinner,
        sch.DinnerFrom,
        sch.DinnerTo,
        sch.SnackNight,
        sch.SnackNightFrom,
        sch.SnackNightTo,
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
        os.OptionValue AS AssignmentTypeName,
        os.OptionValueEn AS AssignmentTypeNameEn,

        -- Información de la posición del staff
        pos.OptionValue AS StaffPositionName,
        pos.OptionValueEn AS StaffPositionNameEn,

        -- Información del tipo de staff
        st.Name AS StaffTypeName,
        st.NameEn AS StaffTypeNameEn,

        -- Información de la ciudad del staff
        c.Name AS StaffCityName,
        c.NameEn AS StaffCityNameEn,

        -- Información de la región del staff
        r.Name AS StaffRegionName,
        r.NameEn AS StaffRegionNameEn,

        -- Información de la agencia del sitio
        a.Name AS AgencyName,
        a.IsActive AS AgencyIsActive

    FROM SchoolStaff ss
        INNER JOIN Staff s ON ss.StaffId = s.Id
        INNER JOIN School sch ON ss.SchoolId = sch.Id
        INNER JOIN OptionSelection os ON ss.AssignmentTypeId = os.Id
        INNER JOIN OptionSelection pos ON s.PositionId = pos.Id
        INNER JOIN StaffType st ON s.StaffTypeId = st.Id
        INNER JOIN City c ON s.CityId = c.Id
        INNER JOIN Region r ON s.RegionId = r.Id
        INNER JOIN Agency a ON sch.AgencyId = a.Id

    WHERE ss.Id = @id;
END
