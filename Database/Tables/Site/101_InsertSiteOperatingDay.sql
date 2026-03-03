-- Versión 101: usa 101_RecalcOperatingDaysFromCalendar para el recálculo de días.
CREATE OR ALTER PROCEDURE [dbo].[101_InsertSiteOperatingDay]
    @siteId INT,
    @operatingDate DATE,
    @startTime TIME = NULL,
    @endTime TIME = NULL,
    @isWeekend BIT = 0,
    @isHoliday BIT = 0,
    @comment NVARCHAR(500) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DayOfWeek INT;

    BEGIN TRY
        IF @SiteId IS NULL OR @SiteId <= 0
        BEGIN RAISERROR('SiteId es requerido y debe ser mayor a 0', 16, 1); RETURN; END
        IF @OperatingDate IS NULL
        BEGIN RAISERROR('OperatingDate es requerido', 16, 1); RETURN; END
        IF NOT EXISTS (SELECT 1 FROM Site WHERE Id = @SiteId)
        BEGIN RAISERROR('El sitio especificado no existe', 16, 1); RETURN; END
        IF EXISTS (SELECT 1 FROM SiteOperatingDays WHERE SiteId = @SiteId AND OperatingDate = @OperatingDate)
        BEGIN RAISERROR('Ya existe un día de funcionamiento para esta fecha y sitio', 16, 1); RETURN; END

        SET @DayOfWeek = DATEPART(WEEKDAY, @operatingDate);
        IF @isWeekend IS NULL
        SET @isWeekend = CASE WHEN @DayOfWeek IN (1, 7) THEN 1 ELSE 0 END;
        IF @startTime IS NULL OR @endTime IS NULL
        BEGIN SET @startTime = ISNULL(@startTime, '08:00:00'); SET @endTime = ISNULL(@endTime, '18:00:00'); END
        IF @comment IS NULL
        SET @comment = CASE WHEN @isHoliday = 1 THEN 'Feriado - Funciona por excepción' WHEN @isWeekend = 1 THEN 'Fin de semana - Funciona por excepción' ELSE 'Día de funcionamiento' END;

        INSERT INTO SiteOperatingDays (SiteId, OperatingDate, StartTime, EndTime, IsWeekend, IsHoliday, Comment, IsActive, IsManuallyAdded, CreatedAt, UpdatedAt)
        VALUES (@siteId, @operatingDate, @startTime, @endTime, @isWeekend, @isHoliday, @comment, 1, 1, GETDATE(), GETDATE());
        SET @id = SCOPE_IDENTITY();

        EXEC [dbo].[101_RecalcOperatingDaysFromCalendar] @siteId = @siteId;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        SET @id = NULL;
    END CATCH
END
GO
