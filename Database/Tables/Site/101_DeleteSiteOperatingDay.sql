-- Versión 101: usa 101_RecalcOperatingDaysFromCalendar para el recálculo de días.
CREATE OR ALTER PROCEDURE [dbo].[101_DeleteSiteOperatingDay]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @id IS NULL OR @id <= 0
        BEGIN RAISERROR('Id es requerido y debe ser mayor a 0', 16, 1); RETURN; END
        IF NOT EXISTS (SELECT 1 FROM SiteOperatingDays WHERE Id = @id)
        BEGIN RAISERROR('El día de funcionamiento especificado no existe', 16, 1); RETURN; END

        DECLARE @siteIdForRecalc INT;
        SELECT @siteIdForRecalc = SiteId FROM SiteOperatingDays WHERE Id = @id;

        DELETE FROM SiteOperatingDays WHERE Id = @id;

        IF @siteIdForRecalc IS NOT NULL
        BEGIN
        EXEC [dbo].[101_RecalcOperatingDaysFromCalendar] @siteId = @siteIdForRecalc;
    END

        SELECT @@ROWCOUNT AS RowsAffected;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
