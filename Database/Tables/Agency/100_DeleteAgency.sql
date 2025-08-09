
-- Procedimiento para eliminar una agencia (soft delete)
CREATE OR ALTER PROCEDURE [100_DeleteAgency]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Agency
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 

