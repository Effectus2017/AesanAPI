-- 100_InsertPermission.sql
-- Inserta un nuevo permiso
CREATE OR ALTER PROCEDURE [100_InsertPermission]
    @valueKey NVARCHAR(50),
    @name NVARCHAR(100),
    @nameEn NVARCHAR(100) = NULL,
    @isActive BIT = 1,
    @id VARCHAR(36) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @id = NEWID();

    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (@id, @valueKey, @name, @nameEn, @isActive, GETDATE(), GETDATE());

    RETURN @id;
END; 