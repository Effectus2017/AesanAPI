-- =============================================
-- Stored Procedure: 101_SyncSiteOperatingDaysWithWeekPattern
-- Versión 101: usa 101_RecalcOperatingDaysFromCalendar para el recálculo de días.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_SyncSiteOperatingDaysWithWeekPattern]
    @siteId INT,
    @defaultStartTime TIME = '08:00:00',
    @defaultEndTime TIME = '18:00:00'
AS
BEGIN
    SET NOCOUNT ON;
    SET DATEFIRST 1;
    DECLARE @operatingFromDate DATE;
    DECLARE @operatingToDate DATE;
    DECLARE @deletedCount INT = 0;
    DECLARE @insertedCount INT = 0;
    DECLARE @servicesInserted INT = 0;
    DECLARE @errorMessage NVARCHAR(4000);

    BEGIN TRY
        SELECT 
            @operatingFromDate = OperatingFromDate,
            @operatingToDate = OperatingToDate,
            @defaultStartTime = COALESCE(OperatingStartTime, @defaultStartTime),
            @defaultEndTime = COALESCE(OperatingEndTime, @defaultEndTime)
        FROM Site WHERE Id = @siteId;

        IF @operatingFromDate IS NULL AND @operatingToDate IS NULL
        BEGIN
            SELECT @deletedCount AS DaysDeleted, @insertedCount AS DaysInserted, @servicesInserted AS ServicesInserted, 'No hay fechas de operación definidas para el sitio' AS Message;
            RETURN;
        END
        IF @operatingFromDate > @operatingToDate
        BEGIN
            DECLARE @tempDate DATE = @operatingFromDate;
            SET @operatingFromDate = @operatingToDate;
            SET @operatingToDate = @tempDate;
        END

        DECLARE @activeDaysOfWeek TABLE (DayOfWeek INT);
        INSERT INTO @activeDaysOfWeek (DayOfWeek)
        SELECT DayOfWeek FROM SiteOperatingDaysOfWeek WHERE SiteId = @siteId AND IsActive = 1;

        DELETE FROM SiteOperatingDays
        WHERE SiteId = @siteId
            AND OperatingDate >= @operatingFromDate AND OperatingDate <= @operatingToDate
            AND COALESCE(IsManuallyAdded, 0) = 0
            AND DATEPART(WEEKDAY, OperatingDate) NOT IN (SELECT DayOfWeek FROM @activeDaysOfWeek);
        SET @deletedCount = @@ROWCOUNT;

        CREATE TABLE #NewDays (OperatingDate DATE PRIMARY KEY, DayOfWeek INT);
        DECLARE @currentDate DATE = @operatingFromDate;
        DECLARE @dayOfWeek INT;
        WHILE @currentDate <= @operatingToDate
        BEGIN
            SET @dayOfWeek = DATEPART(WEEKDAY, @currentDate);
            IF EXISTS (SELECT 1 FROM @activeDaysOfWeek WHERE DayOfWeek = @dayOfWeek)
            AND NOT EXISTS (SELECT 1 FROM SiteOperatingDays WHERE SiteId = @siteId AND OperatingDate = @currentDate)
            BEGIN
                INSERT INTO #NewDays (OperatingDate, DayOfWeek) VALUES (@currentDate, @dayOfWeek);
            END
            SET @currentDate = DATEADD(DAY, 1, @currentDate);
        END

        INSERT INTO SiteOperatingDays (SiteId, OperatingDate, StartTime, EndTime, Comment, IsActive, IsManuallyAdded, CreatedAt)
        SELECT @siteId, OperatingDate, @defaultStartTime, @defaultEndTime, 'Día generado automáticamente por sincronización', 1, 0, GETDATE()
        FROM #NewDays;
        SET @insertedCount = @@ROWCOUNT;

        IF @insertedCount > 0 AND EXISTS (SELECT 1 FROM SiteChildGroupService scgs INNER JOIN SiteChildGroup scg ON scgs.ChildGroupId = scg.Id WHERE scg.SiteId = @siteId)
        BEGIN
            DECLARE @services [dbo].[SiteServiceForOperatingDaysType];
            INSERT INTO @services (ChildGroupId, Breakfast, BreakfastFrom, BreakfastTo, Lunch, LunchFrom, LunchTo, SnackAM, SnackAMFrom, SnackAMTo, Dinner, DinnerFrom, DinnerTo, SnackPM, SnackPMFrom, SnackPMTo, SnackNight, SnackNightFrom, SnackNightTo, DinnerExtended, DinnerExtendedFrom, DinnerExtendedTo, DinnerAtRisk, DinnerAtRiskFrom, DinnerAtRiskTo, SnackExtended, SnackExtendedFrom, SnackExtendedTo, SnackAtRisk, SnackAtRiskFrom, SnackAtRiskTo)
            SELECT scg.Id, CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 1 THEN scgs.IsOffered END AS INT)) AS BIT), MAX(CASE WHEN scgs.ServiceTypeId = 1 THEN scgs.FromTime END), MAX(CASE WHEN scgs.ServiceTypeId = 1 THEN scgs.ToTime END), CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 2 THEN scgs.IsOffered END AS INT)) AS BIT), MAX(CASE WHEN scgs.ServiceTypeId = 2 THEN scgs.FromTime END), MAX(CASE WHEN scgs.ServiceTypeId = 2 THEN scgs.ToTime END), CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 3 THEN scgs.IsOffered END AS INT)) AS BIT), MAX(CASE WHEN scgs.ServiceTypeId = 3 THEN scgs.FromTime END), MAX(CASE WHEN scgs.ServiceTypeId = 3 THEN scgs.ToTime END), CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 4 THEN scgs.IsOffered END AS INT)) AS BIT), MAX(CASE WHEN scgs.ServiceTypeId = 4 THEN scgs.FromTime END), MAX(CASE WHEN scgs.ServiceTypeId = 4 THEN scgs.ToTime END), CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 5 THEN scgs.IsOffered END AS INT)) AS BIT), MAX(CASE WHEN scgs.ServiceTypeId = 5 THEN scgs.FromTime END), MAX(CASE WHEN scgs.ServiceTypeId = 5 THEN scgs.ToTime END), CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 6 THEN scgs.IsOffered END AS INT)) AS BIT), MAX(CASE WHEN scgs.ServiceTypeId = 6 THEN scgs.FromTime END), MAX(CASE WHEN scgs.ServiceTypeId = 6 THEN scgs.ToTime END), CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 7 THEN scgs.IsOffered END AS INT)) AS BIT), MAX(CASE WHEN scgs.ServiceTypeId = 7 THEN scgs.FromTime END), MAX(CASE WHEN scgs.ServiceTypeId = 7 THEN scgs.ToTime END), CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 8 THEN scgs.IsOffered END AS INT)) AS BIT), MAX(CASE WHEN scgs.ServiceTypeId = 8 THEN scgs.FromTime END), MAX(CASE WHEN scgs.ServiceTypeId = 8 THEN scgs.ToTime END), CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 9 THEN scgs.IsOffered END AS INT)) AS BIT), MAX(CASE WHEN scgs.ServiceTypeId = 9 THEN scgs.FromTime END), MAX(CASE WHEN scgs.ServiceTypeId = 9 THEN scgs.ToTime END), CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 10 THEN scgs.IsOffered END AS INT)) AS BIT), MAX(CASE WHEN scgs.ServiceTypeId = 10 THEN scgs.FromTime END), MAX(CASE WHEN scgs.ServiceTypeId = 10 THEN scgs.ToTime END)
            FROM SiteChildGroup scg LEFT JOIN SiteChildGroupService scgs ON scgs.ChildGroupId = scg.Id WHERE scg.SiteId = @siteId GROUP BY scg.Id;
            DECLARE @minNewDate DATE, @maxNewDate DATE;
            SELECT @minNewDate = MIN(OperatingDate), @maxNewDate = MAX(OperatingDate) FROM #NewDays;
            EXEC [dbo].[100_InsertServicesForOperatingDays] @siteId = @siteId, @operatingFromDate = @minNewDate, @operatingToDate = @maxNewDate, @services = @services, @rowsInserted = @servicesInserted OUTPUT;
        END

        DROP TABLE #NewDays;
        EXEC [dbo].[101_RecalcOperatingDaysFromCalendar] @siteId = @siteId;
        SELECT @deletedCount AS DaysDeleted, @insertedCount AS DaysInserted, @servicesInserted AS ServicesInserted, 'Sincronización completada exitosamente' AS Message;
    END TRY
    BEGIN CATCH
        IF OBJECT_ID('tempdb..#NewDays') IS NOT NULL DROP TABLE #NewDays;
        SET @errorMessage = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO
