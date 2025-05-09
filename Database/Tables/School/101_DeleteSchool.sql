CREATE OR ALTER PROCEDURE [dbo].[101_DeleteSchool]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Baja lógica de relaciones dependientes
    UPDATE SchoolFacility SET IsActive = 0, UpdatedAt = GETDATE() WHERE SchoolId = @Id;
    UPDATE SchoolMeal SET IsActive = 0, UpdatedAt = GETDATE() WHERE SchoolId = @Id;
    UPDATE SatelliteSchool SET IsActive = 0, UpdatedAt = GETDATE() WHERE MainSchoolId = @Id OR SatelliteSchoolId = @Id;

    -- Baja lógica de la escuela
    UPDATE School
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END; 