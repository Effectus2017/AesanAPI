-- 100_GetAllPermissions.sql
-- Obtiene todos los permisos
CREATE OR ALTER PROCEDURE [100_GetAllPermissions]
    @take INT,
    @skip INT,
    @name NVARCHAR(255) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Description
    FROM Permission
    WHERE (@alls = 1)
        OR (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY Id ASC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM Permission
    WHERE (@alls = 1)
        OR (@name IS NULL OR Name LIKE '%' + @name + '%');
END;

EXEC [100_GetAllPermissions] @take = 10, @skip = 0, @name = NULL, @alls = 1;