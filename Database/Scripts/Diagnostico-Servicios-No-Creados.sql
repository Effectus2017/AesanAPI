-- =============================================
-- Script de Diagnóstico: Servicios no creados para días de funcionamiento
-- Descripción: Verifica por qué no se crearon servicios automáticamente
-- =============================================

-- 1. Verificar si existe SiteService para el sitio
-- Reemplazar @SiteId con el ID del sitio que creaste
DECLARE @SiteId INT = 1; -- ⚠️ CAMBIAR ESTE VALOR

SELECT 
    '=== 1. Verificación de SiteService ===' AS Seccion,
    s.Id AS SiteId,
    s.Name AS SiteName,
    ss.Id AS SiteServiceId,
    ss.ChildGroupId,
    -- Servicios y sus horarios
    ss.Breakfast,
    ss.BreakfastFrom,
    ss.BreakfastTo,
    ss.Lunch,
    ss.LunchFrom,
    ss.LunchTo,
    ss.SnackAM,
    ss.SnackAMFrom,
    ss.SnackAMTo,
    ss.Dinner,
    ss.DinnerFrom,
    ss.DinnerTo,
    -- Contar servicios habilitados
    (CASE WHEN ss.Breakfast = 1 THEN 1 ELSE 0 END +
     CASE WHEN ss.Lunch = 1 THEN 1 ELSE 0 END +
     CASE WHEN ss.SnackAM = 1 THEN 1 ELSE 0 END +
     CASE WHEN ss.Dinner = 1 THEN 1 ELSE 0 END +
     CASE WHEN ss.SnackPM = 1 THEN 1 ELSE 0 END +
     CASE WHEN ss.SnackNight = 1 THEN 1 ELSE 0 END +
     CASE WHEN ss.DinnerExtended = 1 THEN 1 ELSE 0 END +
     CASE WHEN ss.DinnerAtRisk = 1 THEN 1 ELSE 0 END +
     CASE WHEN ss.SnackExtended = 1 THEN 1 ELSE 0 END +
     CASE WHEN ss.SnackAtRisk = 1 THEN 1 ELSE 0 END) AS ServiciosHabilitados
FROM Site s
LEFT JOIN SiteService ss ON s.Id = ss.SiteId
WHERE s.Id = @SiteId;

-- 2. Verificar días de funcionamiento creados
SELECT 
    '=== 2. Días de Funcionamiento Creados ===' AS Seccion,
    sod.Id AS OperatingDayId,
    sod.OperatingDate,
    sod.StartTime AS DayStartTime,
    sod.EndTime AS DayEndTime,
    sod.IsExcluded,
    sod.IsActive,
    -- Contar servicios creados para este día
    (SELECT COUNT(*) FROM SiteOperatingDayService WHERE OperatingDayId = sod.Id) AS ServiciosCreados
FROM SiteOperatingDays sod
WHERE sod.SiteId = @SiteId
ORDER BY sod.OperatingDate;

-- 3. Verificar servicios creados
SELECT 
    '=== 3. Servicios Creados por Día ===' AS Seccion,
    sod.OperatingDate,
    sods.Id AS ServiceId,
    st.Name AS ServiceTypeName,
    sods.StartTime,
    sods.EndTime,
    sods.IsEnabled,
    sods.ChildGroupId
FROM SiteOperatingDayService sods
INNER JOIN SiteOperatingDays sod ON sods.OperatingDayId = sod.Id
INNER JOIN ServiceType st ON sods.ServiceTypeId = st.Id
WHERE sod.SiteId = @SiteId
ORDER BY sod.OperatingDate, st.DisplayOrder;

-- 4. Análisis: ¿Por qué no se crearon servicios?
-- Verificar condiciones que deben cumplirse
SELECT 
    '=== 4. Análisis: Condiciones para Crear Servicios ===' AS Seccion,
    ss.Id AS SiteServiceId,
    ss.Breakfast AS BreakfastEnabled,
    ss.BreakfastFrom,
    ss.BreakfastTo,
    sod.StartTime AS DayStartTime,
    sod.EndTime AS DayEndTime,
    sod.IsExcluded,
    -- Verificar si cumple condiciones para Breakfast
    CASE 
        WHEN ss.Breakfast = 1 
            AND ss.BreakfastFrom IS NOT NULL 
            AND ss.BreakfastTo IS NOT NULL
            AND ss.BreakfastFrom >= sod.StartTime
            AND ss.BreakfastTo <= sod.EndTime
            AND ss.BreakfastFrom < ss.BreakfastTo
            AND sod.IsExcluded = 0
        THEN '✅ CUMPLE'
        ELSE '❌ NO CUMPLE'
    END AS BreakfastCumpleCondiciones,
    -- Motivo si no cumple
    CASE 
        WHEN ss.Breakfast = 0 THEN 'Servicio deshabilitado'
        WHEN ss.BreakfastFrom IS NULL THEN 'BreakfastFrom es NULL'
        WHEN ss.BreakfastTo IS NULL THEN 'BreakfastTo es NULL'
        WHEN ss.BreakfastFrom < sod.StartTime THEN 'BreakfastFrom está antes del StartTime del día'
        WHEN ss.BreakfastTo > sod.EndTime THEN 'BreakfastTo está después del EndTime del día'
        WHEN ss.BreakfastFrom >= ss.BreakfastTo THEN 'BreakfastFrom >= BreakfastTo (inválido)'
        WHEN sod.IsExcluded = 1 THEN 'Día está excluido'
        ELSE '✅ Todas las condiciones cumplidas'
    END AS MotivoBreakfast
FROM SiteService ss
CROSS JOIN SiteOperatingDays sod
WHERE ss.SiteId = @SiteId
    AND sod.SiteId = @SiteId
    AND sod.IsExcluded = 0
ORDER BY sod.OperatingDate;

GO

