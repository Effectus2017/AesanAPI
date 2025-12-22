-- =============================================
-- Stored Procedure: 100_UpdateSiteOperatingDaysOfWeek
-- Descripción: Actualiza múltiples días de la semana para un sitio
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteOperatingDaysOfWeek]
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

    -- Desactivar todos los días existentes
    UPDATE SiteOperatingDaysOfWeek 
    SET IsActive = 0, UpdatedAt = GETDATE()
    WHERE SiteId = @siteId;

    -- Insertar o reactivar los nuevos días
    -- Si el día ya existe pero está inactivo, reactivarlo
    -- Si no existe, insertarlo
    MERGE SiteOperatingDaysOfWeek AS target
    USING @DayOfWeekTable AS source
    ON target.SiteId = @siteId AND target.DayOfWeek = source.DayOfWeek
    WHEN MATCHED THEN
        UPDATE SET IsActive = 1, UpdatedAt = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (SiteId, DayOfWeek, IsActive, CreatedAt)
        VALUES (@siteId, source.DayOfWeek, 1, GETDATE());
END;

