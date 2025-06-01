-- 100_DeletePermission.sql
-- Elimina un permiso por id
CREATE OR ALTER PROCEDURE [100_DeletePermission]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;

    DELETE FROM Permission WHERE Id = @id;

    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END; 