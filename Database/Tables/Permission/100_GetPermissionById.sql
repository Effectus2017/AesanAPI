-- 100_GetPermissionById.sql
-- Obtiene un permiso por id
CREATE OR ALTER PROCEDURE [100_GetPermissionById]
    @id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ValueKey, Name, NameEn, IsActive
    FROM Permission
    WHERE Id = @id;
END; 