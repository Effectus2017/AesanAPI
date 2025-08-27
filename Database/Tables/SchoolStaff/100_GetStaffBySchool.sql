-- =============================================
-- Stored Procedure: 100_GetStaffBySchool
-- =============================================
-- Obtiene todos los empleados asignados a un sitio específico
-- Incluye información del staff, sitio y tipo de asignación

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffBySchool]
    @schoolId INT
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
        r.NameEn AS StaffRegionNameEn

    FROM SchoolStaff ss
        INNER JOIN Staff s ON ss.StaffId = s.Id
        INNER JOIN School sch ON ss.SchoolId = sch.Id
        INNER JOIN OptionSelection os ON ss.AssignmentTypeId = os.Id
        INNER JOIN OptionSelection pos ON s.PositionId = pos.Id
        INNER JOIN StaffType st ON s.StaffTypeId = st.Id
        INNER JOIN City c ON s.CityId = c.Id
        INNER JOIN Region r ON s.RegionId = r.Id

    WHERE ss.SchoolId = @schoolId
        AND ss.IsActive = 1
        AND s.IsActive = 1
        AND sch.IsActive = 1

    ORDER BY ss.IsPrimary DESC, ss.AssignmentDate DESC;
END
