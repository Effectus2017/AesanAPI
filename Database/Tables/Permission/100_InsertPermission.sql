-- 100_InsertPermission.sql
-- Inserta un nuevo permiso
CREATE OR ALTER PROCEDURE [100_InsertPermission]
    @name NVARCHAR(100),
    @description NVARCHAR(255) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Permission
        (Name, Description)
    VALUES
        (@name, @description);

    SET @id = SCOPE_IDENTITY();
    RETURN @id;
END; 