-- =============================================
-- SP: Obtener niveles educativos de una escuela
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[100_GetSchoolEducationLevels]
    @schoolId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sel.Id,
        sel.SchoolId,
        sel.EducationLevelId,
        el.Name as EducationLevelName,
        el.NameEN as EducationLevelNameEN,
        sel.IsActive,
        sel.CreatedAt,
        sel.UpdatedAt
    FROM SchoolEducationLevel sel
        INNER JOIN EducationLevel el ON sel.EducationLevelId = el.Id
    WHERE sel.SchoolId = @schoolId
        AND sel.IsActive = 1
    ORDER BY el.Name;
END;
GO