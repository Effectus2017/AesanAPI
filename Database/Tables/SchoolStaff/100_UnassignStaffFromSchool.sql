-- =============================================
-- Stored Procedure: 100_UnassignStaffFromSchool
-- Descripción: Desactiva la asignación staff–escuela (IsActive = 0)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UnassignStaffFromSchool]
    @schoolId INT,
    @staffId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SchoolStaff
    SET IsActive = 0, UpdatedAt = GETDATE()
    WHERE SchoolId = @schoolId AND StaffId = @staffId AND IsActive = 1;

    SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Result;
END;
