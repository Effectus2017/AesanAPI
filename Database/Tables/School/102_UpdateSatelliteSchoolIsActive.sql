CREATE PROCEDURE [dbo].[102_UpdateSatelliteSchoolIsActive]
    @main_school_id INT,
    @is_active BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SatelliteSchool
    SET IsActive = @is_active
    WHERE MainSchoolId = @main_school_id;
END
GO 