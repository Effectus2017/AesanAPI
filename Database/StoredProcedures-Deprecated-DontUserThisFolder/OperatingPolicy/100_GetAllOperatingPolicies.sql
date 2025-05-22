CREATE OR ALTER PROCEDURE [dbo].[100_GetAllOperatingPolicies]
    @take INT,
    @skip INT,
    @description NVARCHAR(255),
    @alls BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id,
           Description
    FROM dbo.OperatingPolicy
    WHERE (@alls = 1)
        OR
        (@description IS NULL OR Description LIKE '%' + @description + '%')
    ORDER BY Description
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM dbo.OperatingPolicy
    WHERE (@alls = 1)
        OR
        (@description IS NULL OR Description LIKE '%' + @description + '%');
END; 