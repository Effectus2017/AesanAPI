-- 100_UpdatePermission.sql
-- Actualiza un permiso existente
CREATE OR ALTER PROCEDURE [100_UpdatePermission]
    @id INT,
    @name NVARCHAR(100),
    @description NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;

    UPDATE Permission
    SET Description = @description
    WHERE Id = @id;

    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END; 