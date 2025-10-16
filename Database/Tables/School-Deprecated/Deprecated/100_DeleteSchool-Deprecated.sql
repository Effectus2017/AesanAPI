CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSchool]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Primero eliminamos las relaciones en las tablas dependientes
    DELETE FROM SchoolFacility WHERE SchoolId = @id;
    DELETE FROM SchoolMeal WHERE SchoolId = @id;

    -- Luego eliminamos la escuela
    DELETE FROM School WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END; 