CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolChildGroupsBySchoolId]
    @schoolId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que la escuela existe
    IF NOT EXISTS (SELECT 1
    FROM School
    WHERE Id = @schoolId)
    BEGIN
        RAISERROR('La escuela especificada no existe.', 16, 1);
        RETURN;
    END

    SELECT
        scg.Id,
        scg.SchoolId,
        scg.GroupName,
        scg.NumberOfChildren,
        scg.CreatedAt,
        scg.UpdatedAt
    FROM SchoolChildGroup scg
    WHERE scg.SchoolId = @schoolId
    ORDER BY scg.GroupName;
END;
