-- =============================================
-- Stored Procedure: 100_InsertSiteOperatingDays
-- Descripción: Inserta días de funcionamiento para un sitio basado en fechas desde y hasta
-- Reemplaza: 100_InsertSchoolOperatingDays
-- Fecha: 2025-01-15
-- Versión: 2.0
-- Cambios v2.0: Agregado parámetro @includeWeekends para controlar si se incluyen fines de semana
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteOperatingDays]
    @siteId INT,
    @operatingFromDate DATE,
    @operatingToDate DATE,
    @defaultStartTime TIME = '08:00:00',
    @defaultEndTime TIME = '18:00:00',
    @defaultComment NVARCHAR(255) = 'Día de funcionamiento generado automáticamente',
    @includeWeekends BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    -- Establecer Lunes como primer día de la semana para consistencia
    SET DATEFIRST 1;

    DECLARE @DaysInserted INT = 0;
    DECLARE @CurrentDate DATE = @operatingFromDate;
    DECLARE @DayOfWeek INT;

    -- Insertar días de funcionamiento desde la fecha inicial hasta la fecha final
    WHILE @CurrentDate <= @operatingToDate
    BEGIN
        -- Obtener el día de la semana (1 = Lunes, 7 = Domingo)
        -- Con SET DATEFIRST 1, DATEPART(WEEKDAY) devuelve: 1=Lunes, 2=Martes, ..., 7=Domingo
        SET @DayOfWeek = DATEPART(WEEKDAY, @CurrentDate);

        -- Si @includeWeekends = 0, solo insertar Lunes a Viernes (días 1-5)
        -- Si @includeWeekends = 1, insertar todos los días
        IF (@includeWeekends = 1) OR (@includeWeekends = 0 AND @DayOfWeek BETWEEN 1 AND 5)
        BEGIN
            INSERT INTO SiteOperatingDays
                (
                SiteId, OperatingDate, StartTime, EndTime, Comment, IsActive, CreatedAt
                )
            VALUES
                (
                    @siteId, @CurrentDate, @defaultStartTime, @defaultEndTime, @defaultComment, 1, GETDATE()
            );

            SET @DaysInserted = @DaysInserted + 1;
        END

        SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
    END

    -- Retornar el número de días insertados
    SELECT @DaysInserted AS DaysInserted;
END;
