-- =============================================
-- Stored Procedure: 107_DeleteSiteOperatingDayServicesByGroupServiceAndDateRange
-- Descripción: Elimina de SiteOperatingDayService las filas de un (ChildGroupId, ServiceTypeId)
--              cuyos días pertenecen al sitio y están en el rango de fechas dado.
--              Usado al reemplazar días de operación por slot (respetar operatingDates por servicio).
-- Fecha: 2026-02-03
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[107_DeleteSiteOperatingDayServicesByGroupServiceAndDateRange]
    @siteid INT,
    @childgroupid INT,
    @servicetypeid INT,
    @operatingfromdate DATE,
    @operatingtodate DATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @siteid IS NULL OR @siteid <= 0
        BEGIN
            RAISERROR('siteId es requerido y debe ser mayor a 0', 16, 1);
            RETURN;
        END

        IF @childgroupid IS NULL OR @childgroupid <= 0
        BEGIN
            RAISERROR('childGroupId es requerido y debe ser mayor a 0', 16, 1);
            RETURN;
        END

        IF @servicetypeid IS NULL OR @servicetypeid <= 0
        BEGIN
            RAISERROR('serviceTypeId es requerido y debe ser mayor a 0', 16, 1);
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

        DELETE FROM SiteOperatingDayService
        WHERE ChildGroupId = @childgroupid
          AND ServiceTypeId = @servicetypeid
          AND OperatingDayId IN (
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
