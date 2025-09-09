-- 100_UpdatePermission.sql
-- Actualiza un permiso existente
CREATE OR ALTER PROCEDURE [100_UpdatePermission]
    @id VARCHAR(36),
    @valueKey NVARCHAR(50),
    @name NVARCHAR(100),
    @nameEn NVARCHAR(100) = NULL,
    @isActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Permission
    SET ValueKey = @valueKey,
        Name = @name,
        NameEn = @nameEn,
        IsActive = @isActive,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END;