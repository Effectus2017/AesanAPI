CREATE OR ALTER PROCEDURE [dbo].[100_GetAllAreaTypes]
    @take INT,
    @skip INT,
    @name NVARCHAR(100) = NULL,
    @alls BIT = 0,
    @isList BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameEN, IsActive, DisplayOrder
    FROM AreaType
    WHERE (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY DisplayOrder
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    IF @isList = 0
    BEGIN
        SELECT COUNT(*)
        FROM AreaType
        WHERE (@name IS NULL OR Name LIKE '%' + @name + '%');
    END
END 