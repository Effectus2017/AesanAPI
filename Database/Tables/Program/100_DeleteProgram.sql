-- Procedimiento para eliminar un programa
CREATE OR ALTER PROCEDURE [100_DeleteProgram]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @rowsAffected INT;

    -- Soft delete: marcar como inactivo en lugar de eliminar físicamente
    UPDATE Program
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 