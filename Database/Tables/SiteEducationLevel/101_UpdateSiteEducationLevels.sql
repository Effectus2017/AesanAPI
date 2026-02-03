-- =============================================
-- Stored Procedure: 101_UpdateSiteEducationLevels
-- Descripción: Actualiza múltiples niveles educativos para un sitio.
--              Usa DISTINCT para evitar violación de UK al recibir IDs duplicados.
-- Versión: 1.1 (sobre 100_UpdateSiteEducationLevels)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteEducationLevels]
    @siteId INT,
    @educationLevelIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear tabla temporal para los IDs (solo valores distintos)
    DECLARE @EducationLevelTable TABLE (EducationLevelId INT);

    INSERT INTO @EducationLevelTable
        (EducationLevelId)
    SELECT DISTINCT CAST(value AS INT)
    FROM STRING_SPLIT(@educationLevelIds, ',')
    WHERE value IS NOT NULL AND value != '';

    -- Eliminar niveles educativos existentes
    DELETE FROM SiteEducationLevel WHERE SiteId = @siteId;

    -- Insertar nuevos niveles educativos (sin duplicados)
    INSERT INTO SiteEducationLevel
        (SiteId, EducationLevelId, IsActive, CreatedAt)
    SELECT @siteId, EducationLevelId, 1, GETDATE()
    FROM @EducationLevelTable;
END;
