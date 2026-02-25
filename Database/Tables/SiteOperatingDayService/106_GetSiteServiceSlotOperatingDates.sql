-- =============================================
-- Stored Procedure: 106_GetSiteServiceSlotOperatingDates
-- Descripción: Obtiene las fechas de operación por (ChildGroupId, ServiceTypeId)
--              con horas por día (StartTime, EndTime) para mostrar horarios distintos
--              en días extra del calendario.
--              Incluye isholiday, isweekend e ismanuallyadded para diferenciar tipos de día en la UI.
-- Parámetros: @siteid, @fromdate, @todate (rango del sitio).
-- Fecha: 2026-02-06 (v1.2: añadido isholiday, isweekend)
-- Fecha: 2026-02-24 (v1.3: añadido ismanuallyadded para distinguir día extra en la UI)
-- IMPORTANTE: Ejecutar este script en la base de datos para que el indicador muestre
--             el color "Día extra" (teal) en los días agregados manualmente desde el calendario.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[106_GetSiteServiceSlotOperatingDates]
    @siteid INT,
    @fromdate DATE,
    @todate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        childgroupid = sods.ChildGroupId,
        servicetypeid = sods.ServiceTypeId,
        operatingdate = sod.OperatingDate,
        starttime = sods.StartTime,
        endtime = sods.EndTime,
        isholiday = ISNULL(sod.IsHoliday, 0),
        isweekend = ISNULL(sod.IsWeekend, 0),
        ismanuallyadded = ISNULL(sod.IsManuallyAdded, 0)
    FROM SiteOperatingDayService sods
    INNER JOIN SiteOperatingDays sod ON sods.OperatingDayId = sod.Id
    WHERE sod.SiteId = @siteid
      AND sod.OperatingDate >= @fromdate
      AND sod.OperatingDate <= @todate
      AND sod.IsActive = 1
    ORDER BY sod.OperatingDate, sods.ChildGroupId, sods.ServiceTypeId;
END;
