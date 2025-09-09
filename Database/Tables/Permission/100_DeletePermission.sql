-- 100_DeletePermission.sql
-- Elimina un permiso (soft delete)
CREATE OR ALTER PROCEDURE [100_DeletePermission]
    @id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Permission
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SELECT @@ROWCOUNT;
END;