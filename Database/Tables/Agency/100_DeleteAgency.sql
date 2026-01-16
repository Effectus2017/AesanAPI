
-- Procedimiento para eliminar una agencia (soft delete)
CREATE OR ALTER PROCEDURE [100_DeleteAgency]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Protección: No permitir eliminar agencias propietarias (isPropietary = 1)
    IF EXISTS (SELECT 1 FROM Agency WHERE Id = @Id AND IsPropietary = 1)
    BEGIN
        RAISERROR('No se puede eliminar una agencia propietaria. Esta agencia está protegida y no puede ser eliminada bajo ninguna circunstancia.', 16, 1);
        RETURN 0;
    END
    
    DECLARE @rowsAffected INT;

    UPDATE Agency
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 

