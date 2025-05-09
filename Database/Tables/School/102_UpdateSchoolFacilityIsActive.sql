CREATE PROCEDURE [dbo].[102_UpdateSchoolFacilityIsActive]
    @school_id INT,
    @is_active BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SchoolFacility
    SET IsActive = @is_active
    WHERE SchoolId = @school_id;
END
GO 