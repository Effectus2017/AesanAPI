-- =============================================
-- Stored Procedure: 101_MergeServicesForOperatingDays
-- Descripción: Fusiona servicios del template con SiteOperatingDayService en un rango de fechas:
--              actualiza los existentes (OperatingDayId, ServiceTypeId, ChildGroupId) e inserta
--              los que no existen. No elimina filas (preserva servicios agregados desde el calendario).
-- Fecha: 2026-02-02
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_MergeServicesForOperatingDays]
    @siteid INT,
    @operatingfromdate DATE,
    @operatingtodate DATE,
    @services [dbo].[SiteServiceForOperatingDaysType] READONLY,
    @rowsinserted INT OUTPUT,
    @rowsupdated INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @rowsinserted = 0;
    SET @rowsupdated = 0;

    DECLARE @errormessage NVARCHAR(4000);

    BEGIN TRY
        IF @siteid IS NULL OR @siteid <= 0
        BEGIN
            RAISERROR('siteId es requerido y debe ser mayor a 0', 16, 1);
            RETURN;
        END

        IF @operatingfromdate IS NULL OR @operatingtodate IS NULL
        BEGIN
            RAISERROR('operatingFromDate y operatingToDate son requeridos', 16, 1);
            RETURN;
        END

        IF @operatingfromdate > @operatingtodate
        BEGIN
            RAISERROR('operatingFromDate no puede ser mayor que operatingToDate', 16, 1);
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM @services)
        BEGIN
            RETURN;
        END

        IF NOT EXISTS (
            SELECT 1
            FROM SiteOperatingDays
            WHERE SiteId = @siteid
              AND OperatingDate >= @operatingfromdate
              AND OperatingDate <= @operatingtodate
              AND IsActive = 1
        )
        BEGIN
            RETURN;
        END

        CREATE TABLE #ServicesToMerge
        (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            OperatingDayId INT NOT NULL,
            ServiceTypeId INT NOT NULL,
            ChildGroupId INT NOT NULL,
            StartTime TIME NOT NULL,
            EndTime TIME NOT NULL,
            IsEnabled BIT NOT NULL,
            Comment NVARCHAR(500) NULL
        );

        INSERT INTO #ServicesToMerge
        (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, Comment)
        SELECT sod.Id, 1, s.ChildGroupId, s.BreakfastFrom, s.BreakfastTo, 1, NULL
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteid
          AND sod.OperatingDate >= @operatingfromdate
          AND sod.OperatingDate <= @operatingtodate
          AND sod.IsActive = 1
          AND s.Breakfast = 1 AND s.BreakfastFrom IS NOT NULL AND s.BreakfastTo IS NOT NULL AND s.BreakfastFrom < s.BreakfastTo
        UNION ALL
        SELECT sod.Id, 2, s.ChildGroupId, s.LunchFrom, s.LunchTo, 1, NULL
        FROM SiteOperatingDays sod CROSS JOIN @services s
        WHERE sod.SiteId = @siteid AND sod.OperatingDate >= @operatingfromdate AND sod.OperatingDate <= @operatingtodate AND sod.IsActive = 1
          AND s.Lunch = 1 AND s.LunchFrom IS NOT NULL AND s.LunchTo IS NOT NULL AND s.LunchFrom < s.LunchTo
        UNION ALL
        SELECT sod.Id, 3, s.ChildGroupId, s.SnackAMFrom, s.SnackAMTo, 1, NULL
        FROM SiteOperatingDays sod CROSS JOIN @services s
        WHERE sod.SiteId = @siteid AND sod.OperatingDate >= @operatingfromdate AND sod.OperatingDate <= @operatingtodate AND sod.IsActive = 1
          AND s.SnackAM = 1 AND s.SnackAMFrom IS NOT NULL AND s.SnackAMTo IS NOT NULL AND s.SnackAMFrom < s.SnackAMTo
        UNION ALL
        SELECT sod.Id, 4, s.ChildGroupId, s.DinnerFrom, s.DinnerTo, 1, NULL
        FROM SiteOperatingDays sod CROSS JOIN @services s
        WHERE sod.SiteId = @siteid AND sod.OperatingDate >= @operatingfromdate AND sod.OperatingDate <= @operatingtodate AND sod.IsActive = 1
          AND s.Dinner = 1 AND s.DinnerFrom IS NOT NULL AND s.DinnerTo IS NOT NULL AND s.DinnerFrom < s.DinnerTo
        UNION ALL
        SELECT sod.Id, 5, s.ChildGroupId, s.SnackPMFrom, s.SnackPMTo, 1, NULL
        FROM SiteOperatingDays sod CROSS JOIN @services s
        WHERE sod.SiteId = @siteid AND sod.OperatingDate >= @operatingfromdate AND sod.OperatingDate <= @operatingtodate AND sod.IsActive = 1
          AND s.SnackPM = 1 AND s.SnackPMFrom IS NOT NULL AND s.SnackPMTo IS NOT NULL AND s.SnackPMFrom < s.SnackPMTo
        UNION ALL
        SELECT sod.Id, 6, s.ChildGroupId, s.SnackNightFrom, s.SnackNightTo, 1, NULL
        FROM SiteOperatingDays sod CROSS JOIN @services s
        WHERE sod.SiteId = @siteid AND sod.OperatingDate >= @operatingfromdate AND sod.OperatingDate <= @operatingtodate AND sod.IsActive = 1
          AND s.SnackNight = 1 AND s.SnackNightFrom IS NOT NULL AND s.SnackNightTo IS NOT NULL AND s.SnackNightFrom < s.SnackNightTo
        UNION ALL
        SELECT sod.Id, 7, s.ChildGroupId, s.DinnerExtendedFrom, s.DinnerExtendedTo, 1, NULL
        FROM SiteOperatingDays sod CROSS JOIN @services s
        WHERE sod.SiteId = @siteid AND sod.OperatingDate >= @operatingfromdate AND sod.OperatingDate <= @operatingtodate AND sod.IsActive = 1
          AND s.DinnerExtended = 1 AND s.DinnerExtendedFrom IS NOT NULL AND s.DinnerExtendedTo IS NOT NULL AND s.DinnerExtendedFrom < s.DinnerExtendedTo
        UNION ALL
        SELECT sod.Id, 8, s.ChildGroupId, s.DinnerAtRiskFrom, s.DinnerAtRiskTo, 1, NULL
        FROM SiteOperatingDays sod CROSS JOIN @services s
        WHERE sod.SiteId = @siteid AND sod.OperatingDate >= @operatingfromdate AND sod.OperatingDate <= @operatingtodate AND sod.IsActive = 1
          AND s.DinnerAtRisk = 1 AND s.DinnerAtRiskFrom IS NOT NULL AND s.DinnerAtRiskTo IS NOT NULL AND s.DinnerAtRiskFrom < s.DinnerAtRiskTo
        UNION ALL
        SELECT sod.Id, 9, s.ChildGroupId, s.SnackExtendedFrom, s.SnackExtendedTo, 1, NULL
        FROM SiteOperatingDays sod CROSS JOIN @services s
        WHERE sod.SiteId = @siteid AND sod.OperatingDate >= @operatingfromdate AND sod.OperatingDate <= @operatingtodate AND sod.IsActive = 1
          AND s.SnackExtended = 1 AND s.SnackExtendedFrom IS NOT NULL AND s.SnackExtendedTo IS NOT NULL AND s.SnackExtendedFrom < s.SnackExtendedTo
        UNION ALL
        SELECT sod.Id, 10, s.ChildGroupId, s.SnackAtRiskFrom, s.SnackAtRiskTo, 1, NULL
        FROM SiteOperatingDays sod CROSS JOIN @services s
        WHERE sod.SiteId = @siteid AND sod.OperatingDate >= @operatingfromdate AND sod.OperatingDate <= @operatingtodate AND sod.IsActive = 1
          AND s.SnackAtRisk = 1 AND s.SnackAtRiskFrom IS NOT NULL AND s.SnackAtRiskTo IS NOT NULL AND s.SnackAtRiskFrom < s.SnackAtRiskTo;

        IF NOT EXISTS (SELECT 1 FROM #ServicesToMerge)
        BEGIN
            DROP TABLE #ServicesToMerge;
            RETURN;
        END

        DELETE s
        FROM #ServicesToMerge s
        INNER JOIN (
            SELECT Id, ROW_NUMBER() OVER (PARTITION BY OperatingDayId, ServiceTypeId, COALESCE(ChildGroupId, -1) ORDER BY Id) AS RowNum
            FROM #ServicesToMerge
        ) AS Ranked ON s.Id = Ranked.Id
        WHERE Ranked.RowNum > 1;

        DECLARE @MergeOutput TABLE (Action NVARCHAR(10));

        MERGE SiteOperatingDayService AS target
        USING (SELECT OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, Comment FROM #ServicesToMerge) AS source
        ON target.OperatingDayId = source.OperatingDayId
           AND target.ServiceTypeId = source.ServiceTypeId
           AND target.ChildGroupId = source.ChildGroupId
        WHEN MATCHED THEN
            UPDATE SET
                StartTime = source.StartTime,
                EndTime = source.EndTime,
                IsEnabled = source.IsEnabled,
                Comment = source.Comment,
                UpdatedAt = GETDATE()
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, Comment, CreatedAt)
            VALUES (source.OperatingDayId, source.ServiceTypeId, source.ChildGroupId, source.StartTime, source.EndTime, source.IsEnabled, source.Comment, GETDATE())
        OUTPUT $action INTO @MergeOutput (Action);

        SET @rowsinserted = (SELECT COUNT(*) FROM @MergeOutput WHERE Action = 'INSERT');
        SET @rowsupdated = (SELECT COUNT(*) FROM @MergeOutput WHERE Action = 'UPDATE');

        DROP TABLE #ServicesToMerge;
    END TRY
    BEGIN CATCH
        IF OBJECT_ID('tempdb..#ServicesToMerge') IS NOT NULL
            DROP TABLE #ServicesToMerge;
        SET @errormessage = ERROR_MESSAGE();
        DECLARE @errorseverity INT = ERROR_SEVERITY();
        DECLARE @errorstate INT = ERROR_STATE();
        SET @rowsinserted = 0;
        SET @rowsupdated = 0;
        RAISERROR(@errormessage, @errorseverity, @errorstate);
    END CATCH
END;
GO
