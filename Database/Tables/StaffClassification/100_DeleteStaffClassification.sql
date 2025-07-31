-- =============================================
-- Stored Procedure: 100_DeleteStaffClassification
-- =============================================
-- Elimina una clasificación de staff de la base de datos (baja lógica)

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteStaffClassification]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE StaffClassification
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SELECT @@ROWCOUNT AS RowsAffected;
END 