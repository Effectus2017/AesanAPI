-- =============================================
-- Stored Procedure: 100_InsertSiteOperatingDaysOfWeek
-- Descripción: Inserta múltiples días de la semana para un sitio
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteOperatingDaysOfWeek]
    @siteId INT,
    @dayOfWeekIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear tabla temporal para los IDs
    DECLARE @DayOfWeekTable TABLE (DayOfWeek INT);

    -- Insertar IDs en la tabla temporal
    INSERT INTO @DayOfWeekTable
        (DayOfWeek)
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@dayOfWeekIds, ',')
    WHERE value IS NOT NULL AND value != '';

    -- Insertar días de la semana
    INSERT INTO SiteOperatingDaysOfWeek
        (SiteId, DayOfWeek, IsActive, CreatedAt)
    SELECT @siteId, DayOfWeek, 1, GETDATE()
    FROM @DayOfWeekTable;
END;

