-- =============================================
-- Stored Procedure: 102_DeleteSite
-- Descripción: Elimina un sitio de la base de datos (soft delete)
-- Reemplaza: 102_DeleteSchool
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[102_DeleteSite]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Soft delete: marcar como inactivo en lugar de eliminar físicamente
    UPDATE Site 
    SET 
        IsActive = 0,
        InactiveDate = GETDATE(),
        UpdatedAt = GETDATE()
    WHERE Id = @Id;

    -- Retornar el número de filas afectadas
    RETURN @@ROWCOUNT;
END;
