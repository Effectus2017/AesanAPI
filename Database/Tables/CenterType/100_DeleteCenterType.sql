-- 100_DeleteCenterType
-- Elimina un tipo de centro
CREATE OR ALTER PROCEDURE [100_DeleteCenterType]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE CenterType
    SET IsActive = 0, UpdatedAt = GETDATE()
    WHERE Id = @id;
END;
GO 