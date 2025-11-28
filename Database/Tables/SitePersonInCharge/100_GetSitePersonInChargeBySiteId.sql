-- =============================================
-- Stored Procedure: 100_GetSitePersonInChargeBySiteId
-- Descripción: Obtiene información de Persona a Cargo por SiteId
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSitePersonInChargeBySiteId]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id, SiteId, FirstName, MiddleName, FatherLastName, MotherLastName,
        SitePhone, Extension, MobilePhone, CreatedAt, UpdatedAt
    FROM SitePersonInCharge
    WHERE SiteId = @siteId;
END;

