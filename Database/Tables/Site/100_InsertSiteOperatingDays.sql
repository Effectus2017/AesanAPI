-- Stored Procedure para insertar días de funcionamiento de un sitio
-- Genera automáticamente todos los días entre OperatingFromDate y OperatingToDate
-- Excluye fines de semana por defecto, pero permite sobrescribir con IsWeekendOverride
-- NOTA: Los servicios se crean desde el código C# usando los datos del frontend
-- Fecha: 2025-03-14
-- Versión: 3.0
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

        -- Avanzar al siguiente día
        SET @currentDate = DATEADD(DAY, 1, @currentDate);
    END -- WHILE
        
        -- Retornar el número de días insertados
        SELECT @rowsInserted AS DaysInserted;
        
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
