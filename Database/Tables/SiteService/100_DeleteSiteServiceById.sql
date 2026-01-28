-- =============================================
-- Stored Procedure: 100_DeleteSiteServiceById
-- Descripción: Elimina un servicio de alimentación por su ID
-- Fecha: 2025-01-25
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteServiceById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SiteService 
    WHERE Id = @id;
END;
