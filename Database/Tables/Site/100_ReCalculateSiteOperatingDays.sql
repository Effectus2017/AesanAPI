-- =============================================
-- Stored Procedure: 100_ReCalculateSiteOperatingDays
-- Descripción: Recalcula los días totales de funcionamiento de un sitio como el
--             conteo real de filas en SiteOperatingDays en el rango del sitio.
--             Excluye días marcados como feriados (IsHoliday = 1) del total.
-- Fecha: 2025-01-XX
-- Versión: 3.1
-- Cambios v3.0: Cálculo robusto por COUNT de SiteOperatingDays en el rango
-- Cambios v3.1: Excluir días feriados del conteo (IsHoliday = 0 OR IsHoliday IS NULL)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_ReCalculateSiteOperatingDays]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @operatingFromDate DATE;
    DECLARE @operatingToDate DATE;
    DECLARE @totalDays INT = 0;

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

        -- Fuente de verdad: conteo real de días en el calendario (SiteOperatingDays)
        -- Excluir días feriados: no cuentan para el total de días de funcionamiento
        SELECT @totalDays = COUNT(*)
        FROM SiteOperatingDays
        WHERE SiteId = @siteId
          AND OperatingDate >= @operatingFromDate
          AND OperatingDate <= @operatingToDate
          AND IsActive = 1
          AND (IsHoliday = 0 OR IsHoliday IS NULL);

        -- Asegurar que el total no sea negativo
        IF @totalDays < 0
            SET @totalDays = 0;

        -- Actualizar el campo en la tabla Site
        UPDATE Site
        SET OperatingDaysCalculated = @totalDays
        WHERE Id = @siteId;

    END TRY
    BEGIN CATCH
        -- En caso de error, establecer NULL para no mostrar un total incorrecto
        UPDATE Site
        SET OperatingDaysCalculated = NULL
        WHERE Id = @siteId;
    END CATCH
END;
GO
