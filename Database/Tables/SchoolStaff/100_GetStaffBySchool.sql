-- =============================================
-- Stored Procedure: 100_GetStaffBySchool
-- Descripción: Personal asignado a una escuela
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffBySchool]
    @schoolId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ss.Id,
        ss.SchoolId,
        ss.StaffId,
        st.FirstName,
        st.MiddleName,
        st.FatherLastName,
        st.MotherLastName,
        st.Email,
        ss.AssignmentDate,
        ss.AssignmentTypeId,
        at.Name AS AssignmentTypeName,
        at.NameEN AS AssignmentTypeNameEN,
        ss.IsPrimary,
        ss.StartDate,
        ss.EndDate,
        ss.Comments,
        ss.IsActive,
        ss.CreatedAt,
        ss.UpdatedAt
    FROM SchoolStaff ss
        INNER JOIN Staff st ON ss.StaffId = st.Id
        LEFT JOIN OptionSelection at ON ss.AssignmentTypeId = at.Id
    WHERE ss.SchoolId = @schoolId AND ss.IsActive = 1
    ORDER BY ss.IsPrimary DESC, ss.AssignmentDate DESC;
END;
