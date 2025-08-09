-- =============================================
-- Stored Procedure: 100_DeleteMessage
-- =============================================
-- Elimina un mensaje de la base de datos (baja lógica)

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteMessage]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Baja lógica del mensaje
    UPDATE Messages
    SET IsDeleted = 1,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END 