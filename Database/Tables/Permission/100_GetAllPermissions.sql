-- 100_GetAllPermissions.sql
-- Obtiene todos los permisos
CREATE OR ALTER PROCEDURE [100_GetAllPermissions]
    @take INT,
    @skip INT,
    @valueKey NVARCHAR(50) = NULL,
    @name NVARCHAR(100) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ValueKey, Name, NameEn, IsActive
    FROM Permission
    WHERE (@alls = 1)
        OR (@valueKey IS NULL OR ValueKey LIKE '%' + @valueKey + '%')
        OR (@name IS NULL OR Name LIKE '%' + @name + '%')
    ORDER BY ValueKey ASC
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM Permission
    WHERE (@alls = 1)
        OR (@valueKey IS NULL OR ValueKey LIKE '%' + @valueKey + '%')
        OR (@name IS NULL OR Name LIKE '%' + @name + '%');
END;

EXEC [100_GetAllPermissions] @take = 10, @skip = 0, @valueKey = NULL, @name = NULL, @alls = 1;