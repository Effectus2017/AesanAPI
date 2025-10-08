CREATE OR ALTER PROCEDURE [dbo].[101_GetAgenciesList]
    @id INT = NULL,
    @name NVARCHAR(100) = NULL,
    @alls BIT = 0
AS
BEGIN
    
SELECT DISTINCT
        a.Id as id, 
        a.Name as name  
FROM Agency a
WHERE (@alls = 1) 
    OR (
    	(@id IS NULL OR a.Id = @id)
    	AND (@name IS NULL OR a.Name LIKE '%' + @name + '%')
    )

GROUP BY a.Id, a.Name
ORDER BY a.Name

END