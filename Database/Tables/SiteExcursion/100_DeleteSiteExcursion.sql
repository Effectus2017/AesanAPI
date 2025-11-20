-- =============================================
-- Stored Procedure: 100_DeleteSiteExcursion
-- Descripción: Elimina una excursión (soft delete)
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteExcursion]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Soft delete: marcar como inactivo
    UPDATE SiteExcursion
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @id;
END;

