-- =============================================
-- Stored Procedure: 101_RecalcOperatingDaysFromCalendar
-- Formulario: respeta el rango del sitio (OperatingFromDate/OperatingToDate).
-- Calendario: excluye feriados del conteo (IsHoliday = 1 no cuenta).
-- Cuenta días en SiteOperatingDays dentro del rango del sitio que NO son feriados,
-- actualiza Site.OperatingDaysCalculated. Si no hay rango, deja NULL.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_RecalcOperatingDaysFromCalendar]
    @siteId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @operatingFromDate DATE;
    DECLARE @operatingToDate DATE;
    DECLARE @totalDays INT = 0;

    SELECT
        @operatingFromDate = OperatingFromDate,
        @operatingToDate = OperatingToDate
    FROM Site
    WHERE Id = @siteId;

    IF @operatingFromDate IS NULL OR @operatingToDate IS NULL
    BEGIN
        UPDATE Site SET OperatingDaysCalculated = NULL WHERE Id = @siteId;
        RETURN;
    END

    IF @operatingFromDate > @operatingToDate
    BEGIN
        DECLARE @temp DATE = @operatingFromDate;
        SET @operatingFromDate = @operatingToDate;
        SET @operatingToDate = @temp;
    END

    -- Días en el rango del sitio que no son feriados (formulario + calendario)
    SELECT @totalDays = COUNT(*)
    FROM SiteOperatingDays
    WHERE SiteId = @siteId
      AND OperatingDate >= @operatingFromDate
      AND OperatingDate <= @operatingToDate
      AND IsActive = 1
      AND (IsHoliday = 0 OR IsHoliday IS NULL);

    IF @totalDays < 0
        SET @totalDays = 0;

    UPDATE Site
    SET OperatingDaysCalculated = @totalDays
    WHERE Id = @siteId;
END;
GO
