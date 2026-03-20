-- =============================================
-- Stored Procedure: 100_GetSchoolsByStaff
-- Descripción: Escuelas asignadas a un miembro del staff
-- =============================================

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
        sch.Name AS SchoolName,
        sch.SchoolCode,
        sch.AgencyId,
        sch.SchoolNumber,
        sch.IsActive AS SchoolIsActive,
        os.Name AS AssignmentTypeName,
        os.NameEN AS AssignmentTypeNameEN,
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
END;
