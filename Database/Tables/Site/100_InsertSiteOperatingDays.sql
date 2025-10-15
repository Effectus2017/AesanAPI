-- Stored Procedure para insertar días de funcionamiento de un sitio
-- Genera automáticamente todos los días entre OperatingFromDate y OperatingToDate
-- Excluye fines de semana por defecto, pero permite sobrescribir con IsWeekendOverride
CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteOperatingDays]
    @SiteId INT,
    @OperatingFromDate DATE,
    @OperatingToDate DATE,
    @DefaultStartTime TIME = '08:00:00',
    @DefaultEndTime TIME = '16:00:00',
    @DefaultComment NVARCHAR(500) = 'Día de funcionamiento generado automáticamente'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentDate DATE = @OperatingFromDate;
    DECLARE @DayOfWeek INT;
    DECLARE @IsWeekend BIT;
    DECLARE @IsWeekendOverride BIT = 0;
    DECLARE @IsExcluded BIT = 0;
    DECLARE @StartTime TIME;
    DECLARE @EndTime TIME;
    DECLARE @Comment NVARCHAR(500);
    DECLARE @RowsInserted INT = 0;

    BEGIN TRY
        -- Validar parámetros
        IF @SiteId IS NULL OR @OperatingFromDate IS NULL OR @OperatingToDate IS NULL
        BEGIN
        RAISERROR('SiteId, OperatingFromDate y OperatingToDate son requeridos', 16, 1);
        RETURN;
    END
        
        IF @OperatingFromDate > @OperatingToDate
        BEGIN
        RAISERROR('OperatingFromDate no puede ser mayor que OperatingToDate', 16, 1);
        RETURN;
    END
        
        -- Eliminar días existentes para este sitio en el rango de fechas
        DELETE FROM SiteOperatingDays 
        WHERE SiteId = @SiteId
        AND OperatingDate BETWEEN @OperatingFromDate AND @OperatingToDate;
        
        -- Generar días de funcionamiento
        WHILE @CurrentDate <= @OperatingToDate
        BEGIN
        -- Obtener día de la semana (1=Domingo, 2=Lunes, ..., 7=Sábado)
        SET @DayOfWeek = DATEPART(WEEKDAY, @CurrentDate);

        -- Determinar si es fin de semana
        SET @IsWeekend = CASE WHEN @DayOfWeek IN (1, 7) THEN 1 ELSE 0 END;

        -- Configurar valores por defecto
        SET @IsWeekendOverride = 0;
        SET @IsExcluded = 0;
        SET @StartTime = @DefaultStartTime;
        SET @EndTime = @DefaultEndTime;
        SET @Comment = @DefaultComment;

        -- Si es fin de semana, marcarlo como excluido por defecto
        -- Mantener horarios para referencia y consistencia
        IF @IsWeekend = 1
            BEGIN
            SET @IsExcluded = 1;
            -- Mantener horarios por defecto incluso para fines de semana
            SET @Comment = 'Fin de semana - No funciona por defecto';
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
                @SiteId,
                @CurrentDate,
                @StartTime,
                @EndTime,
                @IsWeekendOverride,
                @IsExcluded,
                @Comment,
                GETDATE(),
                GETDATE()
            );

        SET @RowsInserted = @RowsInserted + 1;

        -- Avanzar al siguiente día
        SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
    END
        
        -- Retornar el número de días insertados
        SELECT @RowsInserted AS DaysInserted;
        
    END TRY
    BEGIN CATCH
        -- Log del error
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
