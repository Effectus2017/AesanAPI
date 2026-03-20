-- =============================================
-- Stored Procedure: 100_GetSchoolStaffById
-- Descripción: Una asignación SchoolStaff por Id
-- =============================================

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
        sch.Name AS SchoolName,
        sch.SchoolCode,
        sch.AgencyId,
        a.Name AS AgencyName,
        a.IsActive AS AgencyIsActive,
        st.FirstName,
        st.MiddleName,
        st.FatherLastName,
        st.MotherLastName,
        st.Email
    FROM SchoolStaff ss
        INNER JOIN School sch ON ss.SchoolId = sch.Id
        INNER JOIN Agency a ON sch.AgencyId = a.Id
        INNER JOIN Staff st ON ss.StaffId = st.Id
    WHERE ss.Id = @id;
END;
