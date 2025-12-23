-- =============================================
-- Stored Procedure: 100_InsertServicesForOperatingDays
-- Descripción: Inserta servicios para todos los días de funcionamiento de un sitio en un rango de fechas
-- Fecha: 2025-01-15
-- Versión: 1.0
-- Optimizado: Usa operaciones basadas en conjuntos (set-based) para mejor performance
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertServicesForOperatingDays]
    @siteId INT,
    @operatingFromDate DATE,
    @operatingToDate DATE,
    @services [dbo].[SiteServiceForOperatingDaysType] READONLY,
    @rowsInserted INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @errorMessage NVARCHAR(4000);

    BEGIN TRY
        -- Validar parámetros
        IF @siteId IS NULL OR @siteId <= 0
        BEGIN
        RAISERROR('SiteId es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        IF @operatingFromDate IS NULL OR @operatingToDate IS NULL
        BEGIN
        RAISERROR('OperatingFromDate y OperatingToDate son requeridos', 16, 1);
        RETURN;
    END

        IF @operatingFromDate > @operatingToDate
        BEGIN
        RAISERROR('OperatingFromDate no puede ser mayor que OperatingToDate', 16, 1);
        RETURN;
    END

        -- Validar que hay servicios para procesar
        IF NOT EXISTS (SELECT 1
    FROM @services)
        BEGIN
        SET @rowsInserted = 0;
        RETURN;
    END

        -- Si no hay días de funcionamiento, retornar 0
        IF NOT EXISTS (
            SELECT 1
    FROM SiteOperatingDays
    WHERE SiteId = @siteId
        AND OperatingDate >= @operatingFromDate
        AND OperatingDate <= @operatingToDate
        AND IsActive = 1
        )
        BEGIN
        SET @rowsInserted = 0;
        RETURN;
    END

        -- Preparar tabla temporal para servicios a insertar usando operaciones basadas en conjuntos
        CREATE TABLE #ServicesToInsert
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        OperatingDayId INT NOT NULL,
        ServiceTypeId INT NOT NULL,
        ChildGroupId INT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        IsEnabled BIT NOT NULL,
        Comment NVARCHAR(500) NULL
    );

        -- Generar todos los servicios usando CROSS JOIN (producto cartesiano entre días y servicios)
        -- Esto es mucho más eficiente que usar cursores
        INSERT INTO #ServicesToInsert
        (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, Comment)
    -- Breakfast (ServiceTypeId = 1)
                                            SELECT
            sod.Id AS OperatingDayId,
            1 AS ServiceTypeId,
            s.ChildGroupId,
            s.BreakfastFrom AS StartTime,
            s.BreakfastTo AS EndTime,
            1 AS IsEnabled,
            NULL AS Comment
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteId
            AND sod.OperatingDate >= @operatingFromDate
            AND sod.OperatingDate <= @operatingToDate
            AND sod.IsActive = 1
            AND s.Breakfast = 1
            AND s.BreakfastFrom IS NOT NULL
            AND s.BreakfastTo IS NOT NULL
            AND s.BreakfastFrom < s.BreakfastTo

    UNION ALL

        -- Lunch (ServiceTypeId = 2)
        SELECT
            sod.Id AS OperatingDayId,
            2 AS ServiceTypeId,
            s.ChildGroupId,
            s.LunchFrom AS StartTime,
            s.LunchTo AS EndTime,
            1 AS IsEnabled,
            NULL AS Comment
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteId
            AND sod.OperatingDate >= @operatingFromDate
            AND sod.OperatingDate <= @operatingToDate
            AND sod.IsActive = 1
            AND s.Lunch = 1
            AND s.LunchFrom IS NOT NULL
            AND s.LunchTo IS NOT NULL
            AND s.LunchFrom < s.LunchTo

    UNION ALL

        -- SnackAM (ServiceTypeId = 3)
        SELECT
            sod.Id AS OperatingDayId,
            3 AS ServiceTypeId,
            s.ChildGroupId,
            s.SnackAMFrom AS StartTime,
            s.SnackAMTo AS EndTime,
            1 AS IsEnabled,
            NULL AS Comment
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteId
            AND sod.OperatingDate >= @operatingFromDate
            AND sod.OperatingDate <= @operatingToDate
            AND sod.IsActive = 1
            AND s.SnackAM = 1
            AND s.SnackAMFrom IS NOT NULL
            AND s.SnackAMTo IS NOT NULL
            AND s.SnackAMFrom < s.SnackAMTo

    UNION ALL

        -- Dinner (ServiceTypeId = 4)
        SELECT
            sod.Id AS OperatingDayId,
            4 AS ServiceTypeId,
            s.ChildGroupId,
            s.DinnerFrom AS StartTime,
            s.DinnerTo AS EndTime,
            1 AS IsEnabled,
            NULL AS Comment
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteId
            AND sod.OperatingDate >= @operatingFromDate
            AND sod.OperatingDate <= @operatingToDate
            AND sod.IsActive = 1
            AND s.Dinner = 1
            AND s.DinnerFrom IS NOT NULL
            AND s.DinnerTo IS NOT NULL
            AND s.DinnerFrom < s.DinnerTo

    UNION ALL

        -- SnackPM (ServiceTypeId = 5)
        SELECT
            sod.Id AS OperatingDayId,
            5 AS ServiceTypeId,
            s.ChildGroupId,
            s.SnackPMFrom AS StartTime,
            s.SnackPMTo AS EndTime,
            1 AS IsEnabled,
            NULL AS Comment
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteId
            AND sod.OperatingDate >= @operatingFromDate
            AND sod.OperatingDate <= @operatingToDate
            AND sod.IsActive = 1
            AND s.SnackPM = 1
            AND s.SnackPMFrom IS NOT NULL
            AND s.SnackPMTo IS NOT NULL
            AND s.SnackPMFrom < s.SnackPMTo

    UNION ALL

        -- SnackNight (ServiceTypeId = 6)
        SELECT
            sod.Id AS OperatingDayId,
            6 AS ServiceTypeId,
            s.ChildGroupId,
            s.SnackNightFrom AS StartTime,
            s.SnackNightTo AS EndTime,
            1 AS IsEnabled,
            NULL AS Comment
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteId
            AND sod.OperatingDate >= @operatingFromDate
            AND sod.OperatingDate <= @operatingToDate
            AND sod.IsActive = 1
            AND s.SnackNight = 1
            AND s.SnackNightFrom IS NOT NULL
            AND s.SnackNightTo IS NOT NULL
            AND s.SnackNightFrom < s.SnackNightTo

    UNION ALL

        -- DinnerExtended (ServiceTypeId = 7)
        SELECT
            sod.Id AS OperatingDayId,
            7 AS ServiceTypeId,
            s.ChildGroupId,
            s.DinnerExtendedFrom AS StartTime,
            s.DinnerExtendedTo AS EndTime,
            1 AS IsEnabled,
            NULL AS Comment
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteId
            AND sod.OperatingDate >= @operatingFromDate
            AND sod.OperatingDate <= @operatingToDate
            AND sod.IsActive = 1
            AND s.DinnerExtended = 1
            AND s.DinnerExtendedFrom IS NOT NULL
            AND s.DinnerExtendedTo IS NOT NULL
            AND s.DinnerExtendedFrom < s.DinnerExtendedTo

    UNION ALL

        -- DinnerAtRisk (ServiceTypeId = 8)
        SELECT
            sod.Id AS OperatingDayId,
            8 AS ServiceTypeId,
            s.ChildGroupId,
            s.DinnerAtRiskFrom AS StartTime,
            s.DinnerAtRiskTo AS EndTime,
            1 AS IsEnabled,
            NULL AS Comment
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteId
            AND sod.OperatingDate >= @operatingFromDate
            AND sod.OperatingDate <= @operatingToDate
            AND sod.IsActive = 1
            AND s.DinnerAtRisk = 1
            AND s.DinnerAtRiskFrom IS NOT NULL
            AND s.DinnerAtRiskTo IS NOT NULL
            AND s.DinnerAtRiskFrom < s.DinnerAtRiskTo

    UNION ALL

        -- SnackExtended (ServiceTypeId = 9)
        SELECT
            sod.Id AS OperatingDayId,
            9 AS ServiceTypeId,
            s.ChildGroupId,
            s.SnackExtendedFrom AS StartTime,
            s.SnackExtendedTo AS EndTime,
            1 AS IsEnabled,
            NULL AS Comment
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteId
            AND sod.OperatingDate >= @operatingFromDate
            AND sod.OperatingDate <= @operatingToDate
            AND sod.IsActive = 1
            AND s.SnackExtended = 1
            AND s.SnackExtendedFrom IS NOT NULL
            AND s.SnackExtendedTo IS NOT NULL
            AND s.SnackExtendedFrom < s.SnackExtendedTo

    UNION ALL

        -- SnackAtRisk (ServiceTypeId = 10)
        SELECT
            sod.Id AS OperatingDayId,
            10 AS ServiceTypeId,
            s.ChildGroupId,
            s.SnackAtRiskFrom AS StartTime,
            s.SnackAtRiskTo AS EndTime,
            1 AS IsEnabled,
            NULL AS Comment
        FROM SiteOperatingDays sod
        CROSS JOIN @services s
        WHERE sod.SiteId = @siteId
            AND sod.OperatingDate >= @operatingFromDate
            AND sod.OperatingDate <= @operatingToDate
            AND sod.IsActive = 1
            AND s.SnackAtRisk = 1
            AND s.SnackAtRiskFrom IS NOT NULL
            AND s.SnackAtRiskTo IS NOT NULL
            AND s.SnackAtRiskFrom < s.SnackAtRiskTo;

        -- Si no hay servicios válidos para insertar, retornar 0
        IF NOT EXISTS (SELECT 1
    FROM #ServicesToInsert)
        BEGIN
        SET @rowsInserted = 0;
        DROP TABLE #ServicesToInsert;
        RETURN;
    END

        -- Eliminar duplicados antes de insertar (mismo día, mismo tipo de servicio, mismo grupo)
        -- Mantener solo el primero encontrado usando ROW_NUMBER
        DELETE s
        FROM #ServicesToInsert s
        INNER JOIN (
            SELECT
            Id,
            ROW_NUMBER() OVER (
                    PARTITION BY OperatingDayId, ServiceTypeId, 
                        COALESCE(ChildGroupId, -1)
                    ORDER BY Id
                ) AS RowNum
        FROM #ServicesToInsert
        ) AS Ranked ON s.Id = Ranked.Id
        WHERE Ranked.RowNum > 1;

        -- Eliminar servicios que ya existen en la base de datos (evitar duplicados)
        DELETE s
        FROM #ServicesToInsert s
        INNER JOIN SiteOperatingDayService sods
        ON sods.OperatingDayId = s.OperatingDayId
            AND sods.ServiceTypeId = s.ServiceTypeId
            AND (s.ChildGroupId IS NULL AND sods.ChildGroupId IS NULL
            OR sods.ChildGroupId = s.ChildGroupId);

        -- Insertar todos los servicios en batch
        INSERT INTO SiteOperatingDayService
        (
        OperatingDayId,
        ServiceTypeId,
        ChildGroupId,
        StartTime,
        EndTime,
        IsEnabled,
        Comment,
        CreatedAt
        )
    SELECT
        OperatingDayId,
        ServiceTypeId,
        ChildGroupId,
        StartTime,
        EndTime,
        IsEnabled,
        Comment,
        GETDATE()
    FROM #ServicesToInsert;

        SET @rowsInserted = @@ROWCOUNT;

        -- Limpiar tabla temporal
        DROP TABLE #ServicesToInsert;

    END TRY
    BEGIN CATCH
        -- Limpiar tabla temporal si existe
        IF OBJECT_ID('tempdb..#ServicesToInsert') IS NOT NULL
        BEGIN
        DROP TABLE #ServicesToInsert;
    END

        SET @errorMessage = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();

        SET @rowsInserted = 0;
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO

