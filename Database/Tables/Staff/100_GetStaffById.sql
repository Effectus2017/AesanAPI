-- =============================================
-- Stored Procedure: 100_GetStaffById
-- =============================================
-- Obtiene un miembro del staff específico por su ID

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        s.StatusId,
        os_status.Name AS StatusName,
        os_status.NameEN AS StatusNameEN,
        s.PositionId,
        os_position.Name AS PositionName,
        os_position.NameEN AS PositionNameEN,
        s.StaffTypeId,
        st.Name AS StaffTypeName,
        st.NameEn AS StaffTypeNameEn,
        s.StaffClassificationId,
        sc.Name AS StaffClassificationName,
        sc.NameEn AS StaffClassificationNameEn,
        s.ContractStartDate,
        s.ContractEndDate,
        s.BirthDate,
        s.Email,
        s.PostalAddress,
        s.CityId,
        c.Name AS CityName,
        s.RegionId,
        r.Name AS RegionName,
        s.AreaCode,
        s.AgencyId,
        a.Name AS AgencyName,
        s.Comments,
        s.UserId,
        u.UserName,
        s.CreatedAt,
        s.UpdatedAt,
        s.IsActive,
        s.ReviewResultId,
        s.ReviewDate,
        s.ReviewJustification,

        -- Datos de la relación SchoolStaff
        ss.SchoolId,
        sch.Name AS SchoolName,
        ss.AssignmentTypeId,
        os_assignment.Name AS AssignmentTypeName,
        os_assignment.NameEN AS AssignmentTypeNameEN,
        ss.IsPrimary
    FROM Staff s
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
        LEFT JOIN StaffType st ON s.StaffTypeId = st.Id
        LEFT JOIN StaffClassification sc ON s.StaffClassificationId = sc.Id
        LEFT JOIN City c ON s.CityId = c.Id
        LEFT JOIN Region r ON s.RegionId = r.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
        LEFT JOIN AspNetUsers u ON s.UserId = u.Id
        LEFT JOIN SchoolStaff ss ON s.Id = ss.StaffId AND ss.IsActive = 1
        LEFT JOIN School sch ON ss.SchoolId = sch.Id
        LEFT JOIN OptionSelection os_assignment ON ss.AssignmentTypeId = os_assignment.Id
    WHERE s.Id = @id;
END