-- =============================================
-- Stored Procedure: 103_UpdateSiteActiveStatus
-- Descripción: Actualiza el estado activo/inactivo de un sitio
-- Reemplaza: 103_UpdateSchoolActiveStatus
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[103_UpdateSiteActiveStatus]
    @id INT,
    @isActive BIT,
    @inactiveJustification NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Site 
    SET 
        IsActive = @isActive,
        InactiveJustification = @inactiveJustification,
        InactiveDate = CASE WHEN @isActive = 0 THEN GETDATE() ELSE NULL END,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    -- Retornar el número de filas afectadas
    RETURN @@ROWCOUNT;
END;
