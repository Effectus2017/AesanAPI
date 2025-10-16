-- =============================================
-- Stored Procedure: 100_InsertSiteEducationLevels
-- Descripción: Inserta múltiples niveles educativos para un sitio
-- Reemplaza: 100_InsertSchoolEducationLevels
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteEducationLevels]
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

    -- Insertar niveles educativos
    INSERT INTO SiteEducationLevel
        (SiteId, EducationLevelId, IsActive, CreatedAt)
    SELECT @siteId, EducationLevelId, 1, GETDATE()
    FROM @EducationLevelTable;
END;
