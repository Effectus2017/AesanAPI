-- Stored Procedure para insertar días de funcionamiento de un sitio
-- Genera automáticamente todos los días entre OperatingFromDate y OperatingToDate
-- Excluye fines de semana por defecto, pero permite sobrescribir con IsWeekendOverride
-- MODIFICADO: Replica servicios automáticamente desde SiteService
-- Fecha: 2025-03-14
-- Versión: 2.0
CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteOperatingDays]
    @siteId INT,
    @operatingFromDate DATE,
    @operatingToDate DATE,
    @defaultStartTime TIME = '08:00:00',
    @defaultEndTime TIME = '16:00:00',
    @defaultComment NVARCHAR(500) = 'Día de funcionamiento generado automáticamente'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @currentDate DATE = @operatingFromDate;
    DECLARE @dayOfWeek INT;
    DECLARE @isWeekend BIT;
    DECLARE @isWeekendOverride BIT = 0;
    DECLARE @isExcluded BIT = 0;
    DECLARE @startTime TIME;
    DECLARE @endTime TIME;
    DECLARE @comment NVARCHAR(500);
    DECLARE @rowsInserted INT = 0;
    DECLARE @operatingDayId INT;
    DECLARE @servicesCreated INT = 0;

    -- IDs de tipos de servicio (fijos e inmutables desde tabla ServiceType)
    -- Estos IDs están predefinidos y NO cambian
    DECLARE @serviceTypeIdBreakfast INT = 1;
    DECLARE @serviceTypeIdLunch INT = 2;
    DECLARE @serviceTypeIdSnackAM INT = 3;
    DECLARE @serviceTypeIdDinner INT = 4;
    DECLARE @serviceTypeIdSnackPM INT = 5;
    DECLARE @serviceTypeIdSnackNight INT = 6;
    DECLARE @serviceTypeIdDinnerExtended INT = 7;
    DECLARE @serviceTypeIdDinnerAtRisk INT = 8;
    DECLARE @serviceTypeIdSnackExtended INT = 9;
    DECLARE @serviceTypeIdSnackAtRisk INT = 10;

    BEGIN TRY
        -- Validar parámetros
        IF @siteId IS NULL OR @operatingFromDate IS NULL OR @operatingToDate IS NULL
        BEGIN
        RAISERROR('SiteId, OperatingFromDate y OperatingToDate son requeridos', 16, 1);
        RETURN;
    END
        
        IF @operatingFromDate > @operatingToDate
        BEGIN
        RAISERROR('OperatingFromDate no puede ser mayor que OperatingToDate', 16, 1);
        RETURN;
    END
        
        -- Eliminar días existentes para este sitio en el rango de fechas
        -- (CASCADE DELETE eliminará automáticamente los servicios relacionados)
        DELETE FROM SiteOperatingDays 
        WHERE SiteId = @siteId
        AND OperatingDate BETWEEN @operatingFromDate AND @operatingToDate;

        -- Los IDs de tipos de servicio ya están definidos como constantes fijas en las variables DECLARE
        -- Tabla ServiceType contiene los IDs: 1-10 (ver ServiceType/Insert-ServiceTypes.sql)
        
        -- Generar días de funcionamiento
        WHILE @currentDate <= @operatingToDate
        BEGIN
        -- Obtener día de la semana (1=Domingo, 2=Lunes, ..., 7=Sábado)
        SET @dayOfWeek = DATEPART(WEEKDAY, @currentDate);

        -- Determinar si es fin de semana
        SET @isWeekend = CASE WHEN @dayOfWeek IN (1, 7) THEN 1 ELSE 0 END;

        -- Configurar valores por defecto
        SET @isWeekendOverride = 0;
        SET @isExcluded = 0;
        SET @startTime = @defaultStartTime;
        SET @endTime = @defaultEndTime;
        SET @comment = @defaultComment;

        -- Si es fin de semana, marcarlo como excluido por defecto
        IF @isWeekend = 1
            BEGIN
            SET @isExcluded = 1;
            SET @comment = 'Fin de semana - No funciona por defecto';
        END

        -- Insertar el día
        INSERT INTO SiteOperatingDays
            (
            SiteId,
            OperatingDate,
            StartTime,
            EndTime,
            IsWeekendOverride,
            IsExcluded,
            Comment,
            CreatedAt,
            UpdatedAt
            )
        VALUES
            (
                @siteId,
                @currentDate,
                @startTime,
                @endTime,
                @isWeekendOverride,
                @isExcluded,
                @comment,
                GETDATE(),
                GETDATE()
                );

        SET @operatingDayId = SCOPE_IDENTITY();
        SET @rowsInserted = @rowsInserted + 1;

        -- Si el día NO está excluido y está activo, crear servicios automáticamente desde SiteService
        -- Nota: IsActive se establece por defecto en 1, pero verificamos ambos campos
        IF @isExcluded = 0
            BEGIN
            -- Crear servicios para cada SiteService del sitio
            -- Procesar cada servicio habilitado

            -- Breakfast (ID fijo: 1)
            BEGIN
                INSERT INTO SiteOperatingDayService
                    (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, CreatedAt)
                SELECT
                    @operatingDayId,
                    @serviceTypeIdBreakfast,
                    ss.ChildGroupId,
                    ss.BreakfastFrom,
                    ss.BreakfastTo,
                    1,
                    GETDATE()
                FROM SiteService ss
                WHERE ss.SiteId = @siteId
                    AND ss.Breakfast = 1
                    AND ss.BreakfastFrom IS NOT NULL
                    AND ss.BreakfastTo IS NOT NULL
                    AND ss.BreakfastFrom >= @startTime
                    AND ss.BreakfastTo <= @endTime
                    AND ss.BreakfastFrom < ss.BreakfastTo;

                SET @servicesCreated = @servicesCreated + @@ROWCOUNT;
            END

            -- Lunch (ID fijo: 2)
            BEGIN
                INSERT INTO SiteOperatingDayService
                    (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, CreatedAt)
                SELECT
                    @operatingDayId,
                    @serviceTypeIdLunch,
                    ss.ChildGroupId,
                    ss.LunchFrom,
                    ss.LunchTo,
                    1,
                    GETDATE()
                FROM SiteService ss
                WHERE ss.SiteId = @siteId
                    AND ss.Lunch = 1
                    AND ss.LunchFrom IS NOT NULL
                    AND ss.LunchTo IS NOT NULL
                    AND ss.LunchFrom >= @startTime
                    AND ss.LunchTo <= @endTime
                    AND ss.LunchFrom < ss.LunchTo;

                SET @servicesCreated = @servicesCreated + @@ROWCOUNT;
            END

            -- SnackAM (ID fijo: 3)
            BEGIN
                INSERT INTO SiteOperatingDayService
                    (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, CreatedAt)
                SELECT
                    @operatingDayId,
                    @serviceTypeIdSnackAM,
                    ss.ChildGroupId,
                    ss.SnackAMFrom,
                    ss.SnackAMTo,
                    1,
                    GETDATE()
                FROM SiteService ss
                WHERE ss.SiteId = @siteId
                    AND ss.SnackAM = 1
                    AND ss.SnackAMFrom IS NOT NULL
                    AND ss.SnackAMTo IS NOT NULL
                    AND ss.SnackAMFrom >= @startTime
                    AND ss.SnackAMTo <= @endTime
                    AND ss.SnackAMFrom < ss.SnackAMTo;

                SET @servicesCreated = @servicesCreated + @@ROWCOUNT;
            END

            -- Dinner (ID fijo: 4)
            BEGIN
                INSERT INTO SiteOperatingDayService
                    (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, CreatedAt)
                SELECT
                    @operatingDayId,
                    @serviceTypeIdDinner,
                    ss.ChildGroupId,
                    ss.DinnerFrom,
                    ss.DinnerTo,
                    1,
                    GETDATE()
                FROM SiteService ss
                WHERE ss.SiteId = @siteId
                    AND ss.Dinner = 1
                    AND ss.DinnerFrom IS NOT NULL
                    AND ss.DinnerTo IS NOT NULL
                    AND ss.DinnerFrom >= @startTime
                    AND ss.DinnerTo <= @endTime
                    AND ss.DinnerFrom < ss.DinnerTo;

                SET @servicesCreated = @servicesCreated + @@ROWCOUNT;
            END

            -- SnackPM (ID fijo: 5)
            BEGIN
                INSERT INTO SiteOperatingDayService
                    (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, CreatedAt)
                SELECT
                    @operatingDayId,
                    @serviceTypeIdSnackPM,
                    ss.ChildGroupId,
                    ss.SnackPMFrom,
                    ss.SnackPMTo,
                    1,
                    GETDATE()
                FROM SiteService ss
                WHERE ss.SiteId = @siteId
                    AND ss.SnackPM = 1
                    AND ss.SnackPMFrom IS NOT NULL
                    AND ss.SnackPMTo IS NOT NULL
                    AND ss.SnackPMFrom >= @startTime
                    AND ss.SnackPMTo <= @endTime
                    AND ss.SnackPMFrom < ss.SnackPMTo;

                SET @servicesCreated = @servicesCreated + @@ROWCOUNT;
            END

            -- SnackNight (ID fijo: 6)
            BEGIN
                INSERT INTO SiteOperatingDayService
                    (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, CreatedAt)
                SELECT
                    @operatingDayId,
                    @serviceTypeIdSnackNight,
                    ss.ChildGroupId,
                    ss.SnackNightFrom,
                    ss.SnackNightTo,
                    1,
                    GETDATE()
                FROM SiteService ss
                WHERE ss.SiteId = @siteId
                    AND ss.SnackNight = 1
                    AND ss.SnackNightFrom IS NOT NULL
                    AND ss.SnackNightTo IS NOT NULL
                    AND ss.SnackNightFrom >= @startTime
                    AND ss.SnackNightTo <= @endTime
                    AND ss.SnackNightFrom < ss.SnackNightTo;

                SET @servicesCreated = @servicesCreated + @@ROWCOUNT;
            END

            -- DinnerExtended (ID fijo: 7)
            BEGIN
                INSERT INTO SiteOperatingDayService
                    (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, CreatedAt)
                SELECT
                    @operatingDayId,
                    @serviceTypeIdDinnerExtended,
                    ss.ChildGroupId,
                    ss.DinnerExtendedFrom,
                    ss.DinnerExtendedTo,
                    1,
                    GETDATE()
                FROM SiteService ss
                WHERE ss.SiteId = @siteId
                    AND ss.DinnerExtended = 1
                    AND ss.DinnerExtendedFrom IS NOT NULL
                    AND ss.DinnerExtendedTo IS NOT NULL
                    AND ss.DinnerExtendedFrom >= @startTime
                    AND ss.DinnerExtendedTo <= @endTime
                    AND ss.DinnerExtendedFrom < ss.DinnerExtendedTo;

                SET @servicesCreated = @servicesCreated + @@ROWCOUNT;
            END

            -- DinnerAtRisk (ID fijo: 8)
            BEGIN
                INSERT INTO SiteOperatingDayService
                    (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, CreatedAt)
                SELECT
                    @operatingDayId,
                    @serviceTypeIdDinnerAtRisk,
                    ss.ChildGroupId,
                    ss.DinnerAtRiskFrom,
                    ss.DinnerAtRiskTo,
                    1,
                    GETDATE()
                FROM SiteService ss
                WHERE ss.SiteId = @siteId
                    AND ss.DinnerAtRisk = 1
                    AND ss.DinnerAtRiskFrom IS NOT NULL
                    AND ss.DinnerAtRiskTo IS NOT NULL
                    AND ss.DinnerAtRiskFrom >= @startTime
                    AND ss.DinnerAtRiskTo <= @endTime
                    AND ss.DinnerAtRiskFrom < ss.DinnerAtRiskTo;

                SET @servicesCreated = @servicesCreated + @@ROWCOUNT;
            END

            -- SnackExtended (ID fijo: 9)
            BEGIN
                INSERT INTO SiteOperatingDayService
                    (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, CreatedAt)
                SELECT
                    @operatingDayId,
                    @serviceTypeIdSnackExtended,
                    ss.ChildGroupId,
                    ss.SnackExtendedFrom,
                    ss.SnackExtendedTo,
                    1,
                    GETDATE()
                FROM SiteService ss
                WHERE ss.SiteId = @siteId
                    AND ss.SnackExtended = 1
                    AND ss.SnackExtendedFrom IS NOT NULL
                    AND ss.SnackExtendedTo IS NOT NULL
                    AND ss.SnackExtendedFrom >= @startTime
                    AND ss.SnackExtendedTo <= @endTime
                    AND ss.SnackExtendedFrom < ss.SnackExtendedTo;

                SET @servicesCreated = @servicesCreated + @@ROWCOUNT;
            END

            -- SnackAtRisk (ID fijo: 10)
            BEGIN
                INSERT INTO SiteOperatingDayService
                    (OperatingDayId, ServiceTypeId, ChildGroupId, StartTime, EndTime, IsEnabled, CreatedAt)
                SELECT
                    @operatingDayId,
                    @serviceTypeIdSnackAtRisk,
                    ss.ChildGroupId,
                    ss.SnackAtRiskFrom,
                    ss.SnackAtRiskTo,
                    1,
                    GETDATE()
                FROM SiteService ss
                WHERE ss.SiteId = @siteId
                    AND ss.SnackAtRisk = 1
                    AND ss.SnackAtRiskFrom IS NOT NULL
                    AND ss.SnackAtRiskTo IS NOT NULL
                    AND ss.SnackAtRiskFrom >= @startTime
                    AND ss.SnackAtRiskTo <= @endTime
                    AND ss.SnackAtRiskFrom < ss.SnackAtRiskTo;

                SET @servicesCreated = @servicesCreated + @@ROWCOUNT;
            END
        END
        -- IF @isExcluded = 0

        -- Avanzar al siguiente día
        SET @currentDate = DATEADD(DAY, 1, @currentDate);
    END -- WHILE
        
        -- Retornar el número de días insertados y servicios creados
        SELECT @rowsInserted AS DaysInserted, @servicesCreated AS ServicesCreated;
        
    END TRY
    BEGIN CATCH
        -- Log del error
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
