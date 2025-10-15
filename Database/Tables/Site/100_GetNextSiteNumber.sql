-- =============================================
-- Stored Procedure: 100_GetNextSiteNumber
-- Descripción: Obtiene el siguiente número de sitio disponible para una agencia
-- Fecha: 2025-01-15
-- Autor: Sistema AESAN
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetNextSiteNumber]
    @AgencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NextSiteNumber INT;

    -- Obtener el siguiente número disponible para la agencia
    SELECT @NextSiteNumber = ISNULL(MAX(SiteNumber), 0) + 1
    FROM [Site]
    WHERE AgencyId = @AgencyId;

    -- Retornar el siguiente número
    SELECT @NextSiteNumber AS NextSiteNumber;
END
