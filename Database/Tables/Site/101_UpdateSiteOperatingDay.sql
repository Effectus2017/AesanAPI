-- Versión 101: usa 101_RecalcOperatingDaysFromCalendar para el recálculo de días.
CREATE OR ALTER PROCEDURE [dbo].[101_UpdateSiteOperatingDay]
    @id INT,
    @startTime TIME = NULL,
    @endTime TIME = NULL,
    @isWeekend BIT = NULL,
    @isHoliday BIT = NULL,
    @comment NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;

    BEGIN TRY
        IF @id IS NULL OR @id <= 0
        BEGIN
        RAISERROR('Id es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        IF NOT EXISTS (SELECT 1 FROM SiteOperatingDays WHERE Id = @id)
        BEGIN
        RAISERROR('El día de funcionamiento especificado no existe', 16, 1);
        RETURN;
    END

        UPDATE SiteOperatingDays
        SET 
            StartTime = CASE WHEN @startTime IS NOT NULL THEN @startTime ELSE StartTime END,
            EndTime = CASE WHEN @endTime IS NOT NULL THEN @endTime ELSE EndTime END,
            IsWeekend = CASE WHEN @isWeekend IS NOT NULL THEN @isWeekend ELSE IsWeekend END,
            IsHoliday = ISNULL(@isHoliday, IsHoliday),
            Comment = CASE WHEN @comment IS NOT NULL THEN @comment ELSE Comment END,
            UpdatedAt = GETDATE()
        WHERE Id = @id;

        SET @rowsAffected = @@ROWCOUNT;

        IF @rowsAffected > 0
        BEGIN
        UPDATE SiteOperatingDays
                SET 
                    StartTime = CASE WHEN StartTime IS NULL THEN '08:00:00' ELSE StartTime END,
                    EndTime = CASE WHEN EndTime IS NULL THEN '18:00:00' ELSE EndTime END,
                    UpdatedAt = GETDATE()
                WHERE Id = @id AND (StartTime IS NULL OR EndTime IS NULL);
    END

        DECLARE @siteIdForRecalc INT;
        SELECT @siteIdForRecalc = SiteId FROM SiteOperatingDays WHERE Id = @id;

        IF @siteIdForRecalc IS NOT NULL
        BEGIN
        EXEC [dbo].[101_RecalcOperatingDaysFromCalendar] @siteId = @siteIdForRecalc;
    END

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
