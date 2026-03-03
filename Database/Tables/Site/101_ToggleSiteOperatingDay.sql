-- Versión 101: usa 101_RecalcOperatingDaysFromCalendar para el recálculo de días.
CREATE OR ALTER PROCEDURE [dbo].[101_ToggleSiteOperatingDay]
    @id INT,
    @siteId INT,
    @operatingDate DATE,
    @startTime TIME = NULL,
    @endTime TIME = NULL,
    @isWeekend BIT = 0,
    @isHoliday BIT = 0,
    @comment NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRY
        IF @id IS NULL OR @id <= 0 BEGIN RAISERROR('Id es requerido y debe ser mayor a 0', 16, 1); RETURN; END
        IF @SiteId IS NULL OR @SiteId <= 0 BEGIN RAISERROR('SiteId es requerido y debe ser mayor a 0', 16, 1); RETURN; END
        IF @OperatingDate IS NULL BEGIN RAISERROR('OperatingDate es requerido', 16, 1); RETURN; END
        IF NOT EXISTS (SELECT 1 FROM Site WHERE Id = @SiteId) BEGIN RAISERROR('El sitio especificado no existe', 16, 1); RETURN; END

        IF @startTime IS NULL OR @endTime IS NULL BEGIN SET @startTime = ISNULL(@startTime, '08:00:00'); SET @endTime = ISNULL(@endTime, '18:00:00'); END
        IF @comment IS NULL SET @comment = CASE WHEN @isHoliday = 1 THEN 'Feriado - Funciona por excepción' WHEN @isWeekend = 1 THEN 'Fin de semana - Funciona por excepción' ELSE 'Día de funcionamiento' END;

        UPDATE SiteOperatingDays SET StartTime = @startTime, EndTime = @endTime, IsWeekend = @isWeekend, IsHoliday = @isHoliday, Comment = @comment, UpdatedAt = GETDATE() WHERE Id = @id;
        SET @rowsAffected = @@ROWCOUNT;

        EXEC [dbo].[101_RecalcOperatingDaysFromCalendar] @siteId = @siteId;
        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
