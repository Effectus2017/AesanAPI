-- =============================================
-- Stored Procedure: 100_UpdateSiteEducationLevels
-- Descripción: Actualiza múltiples niveles educativos para un sitio
-- Reemplaza: 100_UpdateSchoolEducationLevels
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteEducationLevels]
    @siteId INT,
    @educationLevelIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear tabla temporal para los IDs
    DECLARE @EducationLevelTable TABLE (EducationLevelId INT);

    -- Insertar IDs en la tabla temporal
    INSERT INTO @EducationLevelTable
        (EducationLevelId)
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@educationLevelIds, ',')
    WHERE value IS NOT NULL AND value != '';

    -- Eliminar niveles educativos existentes
    DELETE FROM SiteEducationLevel WHERE SiteId = @siteId;

    -- Insertar nuevos niveles educativos
    INSERT INTO SiteEducationLevel
        (SiteId, EducationLevelId, IsActive, CreatedAt)
    SELECT @siteId, EducationLevelId, 1, GETDATE()
    FROM @EducationLevelTable;
END;
