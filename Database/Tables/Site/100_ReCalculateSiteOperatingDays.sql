-- =============================================
-- Stored Procedure: 100_ReCalculateSiteOperatingDays
-- Descripción: Recalcula los días totales de funcionamiento de un sitio considerando
--             días seleccionados en SiteOperatingDaysOfWeek, días de fin de semana con override,
--             días feriados y días agregados manualmente
-- Fecha: 2025-01-XX
-- Versión: 2.1
-- Cambios v2.1: Uso de columna IsManuallyAdded para identificar días manuales
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_ReCalculateSiteOperatingDays]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @operatingFromDate DATE;
    DECLARE @operatingToDate DATE;
    DECLARE @totalDays INT = 0;
    DECLARE @currentDate DATE;
    DECLARE @dayOfWeek INT;
    DECLARE @systemDayOfWeek INT;
    -- Formato sistema: 1=Lunes, 2=Martes, ..., 7=Domingo
    DECLARE @hasSelectedDays BIT = 0;
    DECLARE @isHoliday BIT = 0;
    DECLARE @isWeekendOverride BIT = 0;
    DECLARE @existsInCalendar BIT = 0;
    DECLARE @isManuallyAddedDay BIT = 0;
    DECLARE @shouldCount BIT = 0;

    -- Tabla temporal para almacenar días seleccionados
    DECLARE @selectedDays TABLE (DayOfWeek INT);

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
        
        -- Obtener días seleccionados del sitio
        INSERT INTO @selectedDays (DayOfWeek)
        SELECT DayOfWeek
        FROM SiteOperatingDaysOfWeek
        WHERE SiteId = @siteId AND IsActive = 1;
        
        -- Verificar si hay días seleccionados
        IF EXISTS (SELECT 1 FROM @selectedDays)
        BEGIN
            SET @hasSelectedDays = 1;
        END
        
        -- Iterar sobre cada fecha en el rango
        SET @currentDate = @operatingFromDate;
        WHILE @currentDate <= @operatingToDate
        BEGIN
            -- Reiniciar variables para cada día
            SET @isHoliday = 0;
            SET @isWeekendOverride = 0;
            SET @existsInCalendar = 0;
            SET @isManuallyAddedDay = 0;
            SET @shouldCount = 0;

            -- Obtener día de la semana en formato SQL Server (1=Domingo, 2=Lunes, ..., 7=Sábado)
            SET @dayOfWeek = DATEPART(WEEKDAY, @currentDate);

            -- Convertir al formato del sistema (1=Lunes, 2=Martes, ..., 7=Domingo)
            SET @systemDayOfWeek = CASE 
                WHEN @dayOfWeek = 1 THEN 7  -- Domingo
                WHEN @dayOfWeek = 2 THEN 1  -- Lunes
                WHEN @dayOfWeek = 3 THEN 2  -- Martes
                WHEN @dayOfWeek = 4 THEN 3  -- Miércoles
                WHEN @dayOfWeek = 5 THEN 4  -- Jueves
                WHEN @dayOfWeek = 6 THEN 5  -- Viernes
                WHEN @dayOfWeek = 7 THEN 6  -- Sábado
            END;

            -- Verificar información del día en el calendario
            SELECT 
                @existsInCalendar = 1,
                @isHoliday = COALESCE(IsHoliday, 0),
                @isWeekendOverride = COALESCE(IsWeekend, 0),
                @isManuallyAddedDay = COALESCE(IsManuallyAdded, 0)
            FROM SiteOperatingDays
            WHERE SiteId = @siteId
                AND OperatingDate = @currentDate
                AND IsActive = 1;

            -- Determinar si el día debe contarse
            -- PRIORIDAD 1: Si el día existe en el calendario y está activo, contarlo
            -- (esto incluye días manuales y días generados automáticamente)
            IF @existsInCalendar = 1
            BEGIN
                SET @shouldCount = 1;
            END
            -- PRIORIDAD 2: Si no existe en el calendario, verificar según patrón semanal
            ELSE IF @hasSelectedDays = 1
            BEGIN
                IF EXISTS (SELECT 1 FROM @selectedDays WHERE DayOfWeek = @systemDayOfWeek)
                BEGIN
                    SET @shouldCount = 1;
                END
            END
            ELSE
            BEGIN
                -- Sin días seleccionados, usar comportamiento por defecto (lunes a viernes)
                IF @dayOfWeek BETWEEN 2 AND 6
                BEGIN
                    SET @shouldCount = 1;
                END
            END

            -- Contar el día solo si debe contarse y NO es feriado
            IF @shouldCount = 1 AND @isHoliday = 0
            BEGIN
                SET @totalDays = @totalDays + 1;
            END

            -- Avanzar al siguiente día
            SET @currentDate = DATEADD(DAY, 1, @currentDate);
        END
        
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
        
        -- Intentar actualizar con cálculo base (lunes a viernes)
        DECLARE @fallbackDays INT = 0;
        DECLARE @fallbackDate DATE = @operatingFromDate;
        DECLARE @fallbackDayOfWeek INT;
        
        WHILE @fallbackDate <= @operatingToDate
        BEGIN
            SET @fallbackDayOfWeek = DATEPART(WEEKDAY, @fallbackDate);
            IF @fallbackDayOfWeek BETWEEN 2 AND 6
            BEGIN
                SET @fallbackDays = @fallbackDays + 1;
            END
            SET @fallbackDate = DATEADD(DAY, 1, @fallbackDate);
        END
        
        UPDATE Site 
        SET OperatingDaysCalculated = @fallbackDays
        WHERE Id = @siteId;
    END CATCH
END;
GO

