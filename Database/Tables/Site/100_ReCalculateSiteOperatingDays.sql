-- =============================================
-- Stored Procedure: 100_ReCalculateSiteOperatingDays
-- Descripción: Recalcula los días totales de funcionamiento de un sitio considerando
--             días laborables base, días de fin de semana con override y días feriados
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_ReCalculateSiteOperatingDays]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @operatingFromDate DATE;
    DECLARE @operatingToDate DATE;
    DECLARE @baseWorkingDays INT = 0;
    DECLARE @weekends INT = 0;
    DECLARE @holidayDays INT = 0;
    DECLARE @totalDays INT = 0;
    DECLARE @currentDate DATE;
    DECLARE @dayOfWeek INT;

    BEGIN TRY
        -- Obtener fechas de operación del sitio
        SELECT
        @operatingFromDate = OperatingFromDate,
        @operatingToDate = OperatingToDate
    FROM Site
    WHERE Id = @siteId;
        
        -- Validar que existan las fechas
        IF @operatingFromDate IS NULL OR @operatingToDate IS NULL
        BEGIN
        -- Si no hay fechas, establecer NULL
        UPDATE Site 
            SET OperatingDaysCalculated = NULL
            WHERE Id = @siteId;
        RETURN;
    END
        
        -- Asegurar que las fechas estén en el orden correcto
        IF @operatingFromDate > @operatingToDate
        BEGIN
        DECLARE @tempDate DATE = @operatingFromDate;
        SET @operatingFromDate = @operatingToDate;
        SET @operatingToDate = @tempDate;
    END
        
        -- Calcular días laborables base (lunes a viernes)
        SET @currentDate = @operatingFromDate;
        WHILE @currentDate <= @operatingToDate
        BEGIN
        SET @dayOfWeek = DATEPART(WEEKDAY, @currentDate);
        -- DATEPART(WEEKDAY) retorna: 1=Domingo, 2=Lunes, ..., 7=Sábado
        -- Días laborables: 2, 3, 4, 5, 6 (Lunes a Viernes)
        IF @dayOfWeek BETWEEN 2 AND 6
            BEGIN
            SET @baseWorkingDays = @baseWorkingDays + 1;
        END
        SET @currentDate = DATEADD(DAY, 1, @currentDate);
    END
        
        -- Obtener días del calendario dentro del rango
        SELECT
        @weekends = COUNT(CASE WHEN IsWeekend = 1 THEN 1 END),
        @holidayDays = COUNT(CASE WHEN IsHoliday = 1 THEN 1 END)
    FROM SiteOperatingDays
    WHERE SiteId = @siteId
        AND OperatingDate >= @operatingFromDate
        AND OperatingDate <= @operatingToDate;
        
        -- Calcular total ajustado (restar días feriados)
        SET @totalDays = @baseWorkingDays + @weekends - @holidayDays;
        
        -- Asegurar que el total no sea negativo
        IF @totalDays < 0
        BEGIN
        SET @totalDays = 0;
    END
        
        -- Actualizar el campo en la tabla Site
        UPDATE Site 
        SET OperatingDaysCalculated = @totalDays
        WHERE Id = @siteId;
        
    END TRY
    BEGIN CATCH
        -- En caso de error, usar cálculo base como fallback
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        -- Log del error (puede usar RAISERROR con nivel de severidad bajo)
        
        -- Intentar actualizar con cálculo base
        UPDATE Site 
        SET OperatingDaysCalculated = @baseWorkingDays
        WHERE Id = @siteId;
    END CATCH
END;
GO

