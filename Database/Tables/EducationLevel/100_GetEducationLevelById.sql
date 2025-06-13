CREATE OR ALTER PROCEDURE [dbo].[100_GetEducationLevelById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
        Name,
        NameEN,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM EducationLevel
    WHERE Id = @id;
END;
GO