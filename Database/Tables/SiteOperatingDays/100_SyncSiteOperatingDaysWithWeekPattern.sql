-- =============================================
-- Stored Procedure: 100_SyncSiteOperatingDaysWithWeekPattern
-- Descripción: Sincroniza los días del calendario (SiteOperatingDays) con el patrón
--              semanal definido en SiteOperatingDaysOfWeek.
--              - Elimina días que ya no corresponden al patrón (excepto los manuales)
--              - Agrega días nuevos según el nuevo patrón semanal
--              - Genera servicios para los nuevos días agregados
-- Fecha: 2026-01-19
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_SyncSiteOperatingDaysWithWeekPattern]
    @siteId INT,
    @defaultStartTime TIME = '08:00:00',
    @defaultEndTime TIME = '18:00:00'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Establecer Lunes como primer día de la semana para consistencia
    SET DATEFIRST 1;
    
    DECLARE @operatingFromDate DATE;
    DECLARE @operatingToDate DATE;
    DECLARE @deletedCount INT = 0;
    DECLARE @insertedCount INT = 0;
    DECLARE @servicesInserted INT = 0;
    DECLARE @errorMessage NVARCHAR(4000);
    
    BEGIN TRY
        -- Obtener fechas de operación del sitio
        SELECT 
            @operatingFromDate = OperatingFromDate,
            @operatingToDate = OperatingToDate,
            @defaultStartTime = COALESCE(OperatingStartTime, @defaultStartTime),
            @defaultEndTime = COALESCE(OperatingEndTime, @defaultEndTime)
        FROM Site
        WHERE Id = @siteId;
        
        -- Validar que el sitio existe
        IF @operatingFromDate IS NULL AND @operatingToDate IS NULL
        BEGIN
            -- Si no hay fechas, simplemente retornar
            SELECT 
                @deletedCount AS DaysDeleted,
                @insertedCount AS DaysInserted,
                @servicesInserted AS ServicesInserted,
                'No hay fechas de operación definidas para el sitio' AS Message;
            RETURN;
        END
        
        -- Asegurar que las fechas estén en el orden correcto
        IF @operatingFromDate > @operatingToDate
        BEGIN
            DECLARE @tempDate DATE = @operatingFromDate;
            SET @operatingFromDate = @operatingToDate;
            SET @operatingToDate = @tempDate;
        END
        
        -- Obtener los días de la semana activos del patrón
        -- Formato: 1=Lunes, 2=Martes, ..., 7=Domingo
        DECLARE @activeDaysOfWeek TABLE (DayOfWeek INT);
        INSERT INTO @activeDaysOfWeek (DayOfWeek)
        SELECT DayOfWeek 
        FROM SiteOperatingDaysOfWeek 
        WHERE SiteId = @siteId AND IsActive = 1;
        
        -- =============================================
        -- PASO 1: Eliminar días que ya no corresponden al patrón
        -- Solo eliminar días que NO fueron agregados manualmente
        -- =============================================
        
        -- Identificar días a eliminar:
        -- 1. Pertenecen al sitio
        -- 2. Están dentro del rango de fechas de operación
        -- 3. NO fueron agregados manualmente (IsManuallyAdded = 0 o NULL)
        -- 4. El día de la semana de esa fecha NO está en el patrón activo
        DELETE FROM SiteOperatingDays
        WHERE SiteId = @siteId
            AND OperatingDate >= @operatingFromDate
            AND OperatingDate <= @operatingToDate
            AND COALESCE(IsManuallyAdded, 0) = 0
            AND (
                -- Convertir día de la semana SQL Server a formato sistema
                -- Con SET DATEFIRST 1, DATEPART(WEEKDAY) devuelve: 1=Lunes, 2=Martes, ..., 7=Domingo
                DATEPART(WEEKDAY, OperatingDate) NOT IN (SELECT DayOfWeek FROM @activeDaysOfWeek)
            );
        
        SET @deletedCount = @@ROWCOUNT;
        
        -- =============================================
        -- PASO 2: Agregar días nuevos según el patrón semanal
        -- =============================================
        
        -- Crear tabla temporal para almacenar nuevos días
        CREATE TABLE #NewDays (
            OperatingDate DATE PRIMARY KEY,
            DayOfWeek INT
        );
        
        -- Generar todas las fechas dentro del rango que corresponden al patrón
        DECLARE @currentDate DATE = @operatingFromDate;
        DECLARE @dayOfWeek INT;
        
        WHILE @currentDate <= @operatingToDate
        BEGIN
            -- Obtener día de la semana (1=Lunes, 7=Domingo con DATEFIRST 1)
            SET @dayOfWeek = DATEPART(WEEKDAY, @currentDate);
            
            -- Si el día está en el patrón activo
            IF EXISTS (SELECT 1 FROM @activeDaysOfWeek WHERE DayOfWeek = @dayOfWeek)
            BEGIN
                -- Solo insertar si no existe ya en SiteOperatingDays
                IF NOT EXISTS (
                    SELECT 1 
                    FROM SiteOperatingDays 
                    WHERE SiteId = @siteId AND OperatingDate = @currentDate
                )
                BEGIN
                    INSERT INTO #NewDays (OperatingDate, DayOfWeek)
                    VALUES (@currentDate, @dayOfWeek);
                END
            END
            
            SET @currentDate = DATEADD(DAY, 1, @currentDate);
        END
        
        -- Insertar los nuevos días
        INSERT INTO SiteOperatingDays (
            SiteId, 
            OperatingDate, 
            StartTime, 
            EndTime, 
            Comment, 
            IsActive, 
            IsManuallyAdded,
            CreatedAt
        )
        SELECT 
            @siteId,
            OperatingDate,
            @defaultStartTime,
            @defaultEndTime,
            'Día generado automáticamente por sincronización',
            1,
            0, -- IsManuallyAdded = 0 (generado automáticamente)
            GETDATE()
        FROM #NewDays;
        
        SET @insertedCount = @@ROWCOUNT;
        
        -- =============================================
        -- PASO 3: Generar servicios para los nuevos días
        -- =============================================
        
        -- Solo si se insertaron nuevos días y hay servicios configurados para el sitio
        IF @insertedCount > 0
        BEGIN
            -- Verificar si existen slots de servicios (SiteChildGroupService) para los grupos del sitio
            IF EXISTS (SELECT 1 FROM SiteChildGroupService scgs INNER JOIN SiteChildGroup scg ON scgs.ChildGroupId = scg.Id WHERE scg.SiteId = @siteId)
            BEGIN
                -- Preparar tabla de servicios (formato ancho) desde SiteChildGroupService para el SP de inserción
                DECLARE @services [dbo].[SiteServiceForOperatingDaysType];

                INSERT INTO @services (
                    ChildGroupId,
                    Breakfast, BreakfastFrom, BreakfastTo,
                    Lunch, LunchFrom, LunchTo,
                    SnackAM, SnackAMFrom, SnackAMTo,
                    Dinner, DinnerFrom, DinnerTo,
                    SnackPM, SnackPMFrom, SnackPMTo,
                    SnackNight, SnackNightFrom, SnackNightTo,
                    DinnerExtended, DinnerExtendedFrom, DinnerExtendedTo,
                    DinnerAtRisk, DinnerAtRiskFrom, DinnerAtRiskTo,
                    SnackExtended, SnackExtendedFrom, SnackExtendedTo,
                    SnackAtRisk, SnackAtRiskFrom, SnackAtRiskTo
                )
                SELECT
                    scg.Id AS ChildGroupId,
                    CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 1 THEN scgs.IsOffered END AS INT)) AS BIT),
                    MAX(CASE WHEN scgs.ServiceTypeId = 1 THEN scgs.FromTime END),
                    MAX(CASE WHEN scgs.ServiceTypeId = 1 THEN scgs.ToTime END),
                    CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 2 THEN scgs.IsOffered END AS INT)) AS BIT),
                    MAX(CASE WHEN scgs.ServiceTypeId = 2 THEN scgs.FromTime END),
                    MAX(CASE WHEN scgs.ServiceTypeId = 2 THEN scgs.ToTime END),
                    CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 3 THEN scgs.IsOffered END AS INT)) AS BIT),
                    MAX(CASE WHEN scgs.ServiceTypeId = 3 THEN scgs.FromTime END),
                    MAX(CASE WHEN scgs.ServiceTypeId = 3 THEN scgs.ToTime END),
                    CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 4 THEN scgs.IsOffered END AS INT)) AS BIT),
                    MAX(CASE WHEN scgs.ServiceTypeId = 4 THEN scgs.FromTime END),
                    MAX(CASE WHEN scgs.ServiceTypeId = 4 THEN scgs.ToTime END),
                    CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 5 THEN scgs.IsOffered END AS INT)) AS BIT),
                    MAX(CASE WHEN scgs.ServiceTypeId = 5 THEN scgs.FromTime END),
                    MAX(CASE WHEN scgs.ServiceTypeId = 5 THEN scgs.ToTime END),
                    CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 6 THEN scgs.IsOffered END AS INT)) AS BIT),
                    MAX(CASE WHEN scgs.ServiceTypeId = 6 THEN scgs.FromTime END),
                    MAX(CASE WHEN scgs.ServiceTypeId = 6 THEN scgs.ToTime END),
                    CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 7 THEN scgs.IsOffered END AS INT)) AS BIT),
                    MAX(CASE WHEN scgs.ServiceTypeId = 7 THEN scgs.FromTime END),
                    MAX(CASE WHEN scgs.ServiceTypeId = 7 THEN scgs.ToTime END),
                    CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 8 THEN scgs.IsOffered END AS INT)) AS BIT),
                    MAX(CASE WHEN scgs.ServiceTypeId = 8 THEN scgs.FromTime END),
                    MAX(CASE WHEN scgs.ServiceTypeId = 8 THEN scgs.ToTime END),
                    CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 9 THEN scgs.IsOffered END AS INT)) AS BIT),
                    MAX(CASE WHEN scgs.ServiceTypeId = 9 THEN scgs.FromTime END),
                    MAX(CASE WHEN scgs.ServiceTypeId = 9 THEN scgs.ToTime END),
                    CAST(MAX(CAST(CASE WHEN scgs.ServiceTypeId = 10 THEN scgs.IsOffered END AS INT)) AS BIT),
                    MAX(CASE WHEN scgs.ServiceTypeId = 10 THEN scgs.FromTime END),
                    MAX(CASE WHEN scgs.ServiceTypeId = 10 THEN scgs.ToTime END)
                FROM SiteChildGroup scg
                LEFT JOIN SiteChildGroupService scgs ON scgs.ChildGroupId = scg.Id
                WHERE scg.SiteId = @siteId
                GROUP BY scg.Id;
                
                -- Obtener rango de fechas de los nuevos días insertados
                DECLARE @minNewDate DATE, @maxNewDate DATE;
                SELECT @minNewDate = MIN(OperatingDate), @maxNewDate = MAX(OperatingDate)
                FROM #NewDays;
                
                -- Insertar servicios solo para los nuevos días
                -- Usamos el SP existente pero limitamos al rango de nuevos días
                EXEC [dbo].[100_InsertServicesForOperatingDays]
                    @siteId = @siteId,
                    @operatingFromDate = @minNewDate,
                    @operatingToDate = @maxNewDate,
                    @services = @services,
                    @rowsInserted = @servicesInserted OUTPUT;
            END
        END
        
        -- Limpiar tabla temporal
        DROP TABLE #NewDays;
        
        -- =============================================
        -- PASO 4: Recalcular días totales de operación
        -- =============================================
        EXEC [dbo].[100_ReCalculateSiteOperatingDays] @siteId = @siteId;
        
        -- Retornar resultados
        SELECT 
            @deletedCount AS DaysDeleted,
            @insertedCount AS DaysInserted,
            @servicesInserted AS ServicesInserted,
            'Sincronización completada exitosamente' AS Message;
            
    END TRY
    BEGIN CATCH
        -- Limpiar tabla temporal si existe
        IF OBJECT_ID('tempdb..#NewDays') IS NOT NULL
            DROP TABLE #NewDays;
        
        SET @errorMessage = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO
