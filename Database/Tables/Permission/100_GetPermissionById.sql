-- 100_GetPermissionById.sql
-- Obtiene un permiso por id
CREATE OR ALTER PROCEDURE [100_GetPermissionById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Description
    FROM Permission
    WHERE Id = @id;
END; 