-- 100_GetPermissionByValueKey.sql
-- Obtiene un permiso por ValueKey
CREATE OR ALTER PROCEDURE [100_GetPermissionByValueKey]
    @valueKey NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ValueKey, Name, NameEn, IsActive
    FROM Permission
    WHERE ValueKey = @valueKey AND IsActive = 1;
END;
