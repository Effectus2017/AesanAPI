-- =============================================
-- Script de Prueba: Sincronización de Días de Operación
-- Descripción: Verifica que la sincronización entre SiteOperatingDaysOfWeek y 
--              SiteOperatingDays funcione correctamente
-- Fecha: 2026-01-19
-- =============================================

-- NOTA: Este script es solo para pruebas y debe ejecutarse manualmente
-- Asegúrese de ejecutar primero los scripts de actualización de estructura:
-- 1. Update-SiteOperatingDays-AddIsManuallyAdded.sql
-- 2. 100_SyncSiteOperatingDaysWithWeekPattern.sql

-- =============================================
-- ESCENARIO DE PRUEBA
-- =============================================
-- Estado inicial:
--   - SiteOperatingDaysOfWeek: Lunes(1), Martes(2)
--   - SiteOperatingDays: días de Lunes y Martes generados + Jueves agregado manualmente
-- Cambio del usuario:
--   - Quita Martes, agrega Miércoles
-- Resultado esperado:
--   - Se eliminan todos los Martes del calendario (IsManuallyAdded=0)
--   - Se generan todos los Miércoles del calendario
--   - El Jueves manual se preserva (IsManuallyAdded=1)

DECLARE @testSiteId INT;
DECLARE @agencyId INT;

-- Buscar un sitio existente para probar (o crear uno de prueba)
SELECT TOP 1 @testSiteId = Id, @agencyId = AgencyId
FROM Site
WHERE OperatingFromDate IS NOT NULL 
  AND OperatingToDate IS NOT NULL
  AND IsActive = 1;

IF @testSiteId IS NULL
BEGIN
    PRINT 'No se encontró un sitio válido para probar';
    PRINT 'Por favor, cree un sitio con fechas de operación antes de ejecutar este script';
    RETURN;
END

PRINT '=== PRUEBA DE SINCRONIZACIÓN DE DÍAS DE OPERACIÓN ===';
PRINT 'Site ID: ' + CAST(@testSiteId AS NVARCHAR(10));
PRINT '';

-- Mostrar estado actual
PRINT '=== ESTADO ACTUAL ===';

PRINT 'Días de la semana configurados (SiteOperatingDaysOfWeek):';
SELECT 
    DayOfWeek,
    CASE DayOfWeek 
        WHEN 1 THEN 'Lunes'
        WHEN 2 THEN 'Martes'
        WHEN 3 THEN 'Miércoles'
        WHEN 4 THEN 'Jueves'
        WHEN 5 THEN 'Viernes'
        WHEN 6 THEN 'Sábado'
        WHEN 7 THEN 'Domingo'
    END AS DayName,
    IsActive
FROM SiteOperatingDaysOfWeek
WHERE SiteId = @testSiteId
ORDER BY DayOfWeek;

PRINT '';
PRINT 'Días del calendario (SiteOperatingDays):';
SELECT 
    Id,
    OperatingDate,
    DATENAME(WEEKDAY, OperatingDate) AS DayName,
    StartTime,
    EndTime,
    IsActive,
    COALESCE(IsManuallyAdded, 0) AS IsManuallyAdded,
    IsHoliday,
    Comment
FROM SiteOperatingDays
WHERE SiteId = @testSiteId
ORDER BY OperatingDate;

PRINT '';
PRINT 'Total de días en el calendario: ' + CAST((SELECT COUNT(*) FROM SiteOperatingDays WHERE SiteId = @testSiteId) AS NVARCHAR(10));
PRINT 'Días manuales: ' + CAST((SELECT COUNT(*) FROM SiteOperatingDays WHERE SiteId = @testSiteId AND COALESCE(IsManuallyAdded, 0) = 1) AS NVARCHAR(10));
PRINT 'Días automáticos: ' + CAST((SELECT COUNT(*) FROM SiteOperatingDays WHERE SiteId = @testSiteId AND COALESCE(IsManuallyAdded, 0) = 0) AS NVARCHAR(10));

PRINT '';
PRINT '=== EJECUTANDO SINCRONIZACIÓN ===';
PRINT 'Ejecutando 100_SyncSiteOperatingDaysWithWeekPattern...';

-- Ejecutar sincronización
EXEC [dbo].[100_SyncSiteOperatingDaysWithWeekPattern] @siteId = @testSiteId;

PRINT '';
PRINT '=== ESTADO DESPUÉS DE SINCRONIZACIÓN ===';

PRINT 'Días del calendario (SiteOperatingDays):';
SELECT 
    Id,
    OperatingDate,
    DATENAME(WEEKDAY, OperatingDate) AS DayName,
    StartTime,
    EndTime,
    IsActive,
    COALESCE(IsManuallyAdded, 0) AS IsManuallyAdded,
    IsHoliday,
    Comment
FROM SiteOperatingDays
WHERE SiteId = @testSiteId
ORDER BY OperatingDate;

PRINT '';
PRINT 'Total de días en el calendario: ' + CAST((SELECT COUNT(*) FROM SiteOperatingDays WHERE SiteId = @testSiteId) AS NVARCHAR(10));
PRINT 'Días manuales: ' + CAST((SELECT COUNT(*) FROM SiteOperatingDays WHERE SiteId = @testSiteId AND COALESCE(IsManuallyAdded, 0) = 1) AS NVARCHAR(10));
PRINT 'Días automáticos: ' + CAST((SELECT COUNT(*) FROM SiteOperatingDays WHERE SiteId = @testSiteId AND COALESCE(IsManuallyAdded, 0) = 0) AS NVARCHAR(10));

-- Verificar OperatingDaysCalculated
PRINT '';
PRINT 'OperatingDaysCalculated del sitio: ' + CAST((SELECT OperatingDaysCalculated FROM Site WHERE Id = @testSiteId) AS NVARCHAR(10));

PRINT '';
PRINT '=== FIN DE PRUEBA ===';
GO
