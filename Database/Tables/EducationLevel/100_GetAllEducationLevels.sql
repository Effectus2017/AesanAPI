CREATE OR ALTER PROCEDURE [dbo].[100_GetAllEducationLevels]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
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
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Name, NameEN
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM EducationLevel
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
END;
GO