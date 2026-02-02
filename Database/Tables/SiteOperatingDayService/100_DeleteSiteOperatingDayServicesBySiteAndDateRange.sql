-- =============================================
-- Stored Procedure: 100_DeleteSiteOperatingDayServicesBySiteAndDateRange
-- Descripción: Elimina todos los servicios de SiteOperatingDayService para los días
--              de funcionamiento del sitio en el rango de fechas dado.
--              Usado antes de reinsertar servicios al actualizar el sitio.
-- Fecha: 2026-02-02
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteOperatingDayServicesBySiteAndDateRange]
    @siteid INT,
    @operatingfromdate DATE,
    @operatingtodate DATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar parámetros
        IF @siteid IS NULL OR @siteid <= 0
        BEGIN
            RAISERROR('siteId es requerido y debe ser mayor a 0', 16, 1);
            RETURN;
        END

        IF @operatingfromdate IS NULL OR @operatingtodate IS NULL
        BEGIN
            RAISERROR('operatingFromDate y operatingToDate son requeridos', 16, 1);
            RETURN;
        END

        IF @operatingfromdate > @operatingtodate
        BEGIN
            RAISERROR('operatingFromDate no puede ser mayor que operatingToDate', 16, 1);
            RETURN;
        END

        -- Eliminar servicios cuyos días pertenecen al sitio y están en el rango
        DELETE FROM SiteOperatingDayService
        WHERE OperatingDayId IN (
            SELECT Id
            FROM SiteOperatingDays
            WHERE SiteId = @siteid
              AND OperatingDate >= @operatingfromdate
              AND OperatingDate <= @operatingtodate
        );

    END TRY
    BEGIN CATCH
        DECLARE @errormessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @errorseverity INT = ERROR_SEVERITY();
        DECLARE @errorstate INT = ERROR_STATE();

        RAISERROR(@errormessage, @errorseverity, @errorstate);
    END CATCH
END;
GO
