-- =============================================
-- Stored Procedure: 102_HasMainSite
-- Descripción: Verifica si existe un sitio principal en la base de datos
-- Reemplaza: 102_HasMainSchool
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[102_HasMainSite]
AS
BEGIN
    SET NOCOUNT ON;

    -- Retornar 1 si existe un sitio principal activo, 0 en caso contrario
    SELECT COUNT(*) AS HasMainSite
    FROM Site
    WHERE IsMainSite = 1 AND IsActive = 1;
END;
