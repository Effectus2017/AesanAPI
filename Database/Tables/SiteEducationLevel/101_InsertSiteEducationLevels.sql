-- =============================================
-- Stored Procedure: 101_InsertSiteEducationLevels
-- Descripción: Inserta múltiples niveles educativos para un sitio.
--              Usa DISTINCT para evitar violación de UK al recibir IDs duplicados.
-- Versión: 1.1 (sobre 100_InsertSiteEducationLevels)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteEducationLevels]
    @siteId INT,
    @educationLevelIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EducationLevelTable TABLE (EducationLevelId INT);

    INSERT INTO @EducationLevelTable
        (EducationLevelId)
    SELECT DISTINCT CAST(value AS INT)
    FROM STRING_SPLIT(@educationLevelIds, ',')
    WHERE value IS NOT NULL AND value != '';

    INSERT INTO SiteEducationLevel
        (SiteId, EducationLevelId, IsActive, CreatedAt)
    SELECT @siteId, EducationLevelId, 1, GETDATE()
    FROM @EducationLevelTable;
END;
