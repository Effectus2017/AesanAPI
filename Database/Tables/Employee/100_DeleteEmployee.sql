-- =============================================
-- Stored Procedure: 100_DeleteEmployee
-- =============================================
-- Elimina un empleado de la base de datos (baja lógica)

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteEmployee]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Baja lógica del empleado
    UPDATE Employee
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END 