-- =============================================
-- Stored Procedure: 100_DeleteStaff
-- =============================================
-- Elimina un miembro del staff de la base de datos (baja lógica)

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteStaff]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Baja lógica del miembro del staff
    UPDATE Staff
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END