-- =============================================
-- Stored Procedure: 100_DeleteStaffRelationship
-- =============================================
-- Desactiva una relación de parentesco (soft delete)
-- Cambia el estado IsActive a false en lugar de eliminar el registro

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteStaffRelationship]
    @id INT,
    @rowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que la relación exista
    IF NOT EXISTS (SELECT 1
    FROM StaffRelationship
    WHERE Id = @id)
        BEGIN
        RAISERROR ('La relación no existe', 16, 1);
        RETURN;
    END

    -- Realizar soft delete (desactivar la relación)
    UPDATE StaffRelationship
    SET 
        IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;

-- La relación se desactivó correctamente
END
