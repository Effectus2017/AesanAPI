-- =============================================
-- Stored Procedure: 100_DeleteSiteServicesByChildGroupId
-- Descripción: Elimina todos los servicios de alimentación de un grupo específico
-- Fecha: 2025-01-25
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteServicesByChildGroupId]
    @childGroupId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM SiteService 
    WHERE ChildGroupId = @childGroupId;
END;
