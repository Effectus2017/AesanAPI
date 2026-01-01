-- =============================================
-- Stored Procedure: 100_ReCalculateSiteOperatingDays
-- Descripción: Recalcula los días totales de funcionamiento de un sitio considerando
--             días seleccionados en SiteOperatingDaysOfWeek, días de fin de semana con override y días feriados
-- Fecha: 2025-01-XX
-- Versión: 2.0
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
        INSERT INTO @selectedDays
        (DayOfWeek)
    SELECT DayOfWeek
    FROM SiteOperatingDaysOfWeek
    WHERE SiteId = @siteId AND IsActive = 1;
        
        -- Verificar si hay días seleccionados
        IF EXISTS (SELECT 1
    FROM @selectedDays)
        BEGIN
        SET @hasSelectedDays = 1;
    END
        
        -- Iterar sobre cada fecha en el rango
        SET @currentDate = @operatingFromDate;
        WHILE @currentDate <= @operatingToDate
        BEGIN
        -- Obtener día de la semana en formato SQL Server (1=Domingo, 2=Lunes, ..., 7=Sábado)
        SET @dayOfWeek = DATEPART(WEEKDAY, @currentDate);

        -- Convertir al formato del sistema (1=Lunes, 2=Martes, ..., 7=Domingo)
        -- SQL Server: 1=Domingo, 2=Lunes, ..., 7=Sábado
        -- Sistema: 1=Lunes, 2=Martes, ..., 7=Domingo
        SET @systemDayOfWeek = CASE 
                WHEN @dayOfWeek = 1 THEN 7  -- Domingo
                WHEN @dayOfWeek = 2 THEN 1  -- Lunes
                WHEN @dayOfWeek = 3 THEN 2  -- Martes
                WHEN @dayOfWeek = 4 THEN 3  -- Miércoles
                WHEN @dayOfWeek = 5 THEN 4  -- Jueves
                WHEN @dayOfWeek = 6 THEN 5  -- Viernes
                WHEN @dayOfWeek = 7 THEN 6  -- Sábado
            END;

        -- Verificar si el día está marcado como feriado
        SET @isHoliday = 0;
        IF EXISTS (
                SELECT 1
        FROM SiteOperatingDays
        WHERE SiteId = @siteId
            AND OperatingDate = @currentDate
            AND IsHoliday = 1
            )
            BEGIN
            SET @isHoliday = 1;
        END

        -- Verificar si el día está marcado como fin de semana con override
        SET @isWeekendOverride = 0;
        IF EXISTS (
                SELECT 1
        FROM SiteOperatingDays
        WHERE SiteId = @siteId
            AND OperatingDate = @currentDate
            AND IsWeekend = 1
            )
            BEGIN
            SET @isWeekendOverride = 1;
        END

        -- Verificar si el día fue agregado manualmente en el calendario
        -- Un día se considera agregado manualmente si existe en SiteOperatingDays
        DECLARE @isManuallyAdded BIT = 0;
        IF EXISTS (
                SELECT 1
        FROM SiteOperatingDays
        WHERE SiteId = @siteId
            AND OperatingDate = @currentDate
            AND IsActive = 1
            )
            BEGIN
            SET @isManuallyAdded = 1;
        END

        -- Determinar si el día debe contarse
        DECLARE @shouldCount BIT = 0;

        -- PRIORIDAD 1: Si el día fue agregado manualmente en el calendario, siempre contarlo
        -- (a menos que sea feriado, que se verifica después)
        IF @isManuallyAdded = 1
            BEGIN
            SET @shouldCount = 1;
        END

        -- PRIORIDAD 2: Si no fue agregado manualmente, verificar si debe contarse según los días seleccionados
        IF @shouldCount = 0
        BEGIN
            IF @hasSelectedDays = 1
            BEGIN
                -- Si hay días seleccionados, verificar si el día está en la lista
                IF EXISTS (SELECT 1
                FROM @selectedDays
                WHERE DayOfWeek = @systemDayOfWeek)
                    BEGIN
                    SET @shouldCount = 1;
                END
            END
            ELSE
            BEGIN
                -- Si no hay días seleccionados, usar comportamiento por defecto (lunes a viernes)
                -- Días laborables: 2, 3, 4, 5, 6 (Lunes a Viernes en formato SQL Server)
                IF @dayOfWeek BETWEEN 2 AND 6
                    BEGIN
                    SET @shouldCount = 1;
                END
                -- También contar fines de semana con override si no hay días seleccionados
                IF @isWeekendOverride = 1
                    BEGIN
                    SET @shouldCount = 1;
                END
            END
        END

        -- Contar el día solo si debe contarse y no es feriado
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
        -- Log del error (puede usar RAISERROR con nivel de severidad bajo)
        
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

