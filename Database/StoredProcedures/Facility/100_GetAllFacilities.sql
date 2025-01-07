CREATE OR ALTER PROCEDURE [dbo].[100_GetAllFacilities]
    @take INT,
    @skip INT,
    @name NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Name
    FROM dbo.Facility
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM dbo.Facility
    WHERE (@alls = 1)
        OR
        (@name IS NULL OR Name LIKE '%' + @name + '%');
END; 