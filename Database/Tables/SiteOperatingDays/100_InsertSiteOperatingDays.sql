-- =============================================
-- Stored Procedure: 100_InsertSiteOperatingDays
-- Descripción: Inserta días de funcionamiento para un sitio basado en fechas desde y hasta
-- Reemplaza: 100_InsertSchoolOperatingDays
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteOperatingDays]
    @siteId INT,
    @operatingFromDate DATE,
    @operatingToDate DATE,
    @defaultStartTime TIME = '08:00:00',
    @defaultEndTime TIME = '16:00:00',
    @defaultComment NVARCHAR(255) = 'Día de funcionamiento generado automáticamente'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DaysInserted INT = 0;
    DECLARE @CurrentDate DATE = @operatingFromDate;

    -- Insertar días de funcionamiento desde la fecha inicial hasta la fecha final
    WHILE @CurrentDate <= @operatingToDate
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
        SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
    END

    -- Retornar el número de días insertados
    RETURN @DaysInserted;
END;
