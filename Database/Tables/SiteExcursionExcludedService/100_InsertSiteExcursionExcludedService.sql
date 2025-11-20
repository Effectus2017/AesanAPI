-- =============================================
-- Stored Procedure: 100_InsertSiteExcursionExcludedService
-- Descripción: Inserta un servicio excluido para una excursión
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteExcursionExcludedService]
    @siteExcursionId INT,
    @serviceTypeId INT,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SiteExcursionExcludedService
        (SiteExcursionId, ServiceTypeId, CreatedAt)
    VALUES
        (@siteExcursionId, @serviceTypeId, GETDATE());

    SET @id = SCOPE_IDENTITY();
END;

