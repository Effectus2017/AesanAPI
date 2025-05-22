CREATE OR ALTER PROCEDURE [dbo].[100_GetAllOptionSelections]
    @take INT,
    @skip INT,
    @name NVARCHAR(255) = NULL,
    @optionKey NVARCHAR(255) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM OptionSelection
    WHERE (@alls = 1)
        AND (@name IS NULL OR Name LIKE '%' + @name + '%')
        AND (@optionKey IS NULL OR OptionKey = @optionKey)
        AND IsActive = 1
    ORDER BY DisplayOrder, Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    -- Total count
    SELECT COUNT(*)
    FROM OptionSelection
    WHERE (@alls = 1)
        AND (@name IS NULL OR Name LIKE '%' + @name + '%')
        AND (@optionKey IS NULL OR OptionKey = @optionKey)
        AND IsActive = 1;
END;
GO 