-- =============================================
-- Stored Procedure: 100_DeleteStaffType
-- =============================================
-- Elimina un tipo de staff de la base de datos (baja lógica)

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteStaffType]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Baja lógica del tipo de staff
    UPDATE StaffType
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END