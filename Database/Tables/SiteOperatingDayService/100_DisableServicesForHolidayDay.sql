-- =============================================
-- Stored Procedure: 100_DisableServicesForHolidayDay
-- Descripción: Deshabilita todos los servicios de un día feriado
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DisableServicesForHolidayDay]
    @operatingDayId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @errorMessage NVARCHAR(4000);

    BEGIN TRY
        -- Validar parámetros
        IF @operatingDayId IS NULL OR @operatingDayId <= 0
        BEGIN
            RAISERROR('OperatingDayId es requerido y debe ser mayor a 0', 16, 1);
            RETURN;
        END

        -- Verificar que el día existe y es feriado
        IF NOT EXISTS (
            SELECT 1 
            FROM SiteOperatingDays 
            WHERE Id = @operatingDayId AND IsHoliday = 1
        )
        BEGIN
            RAISERROR('El día especificado no existe o no es un día feriado', 16, 1);
            RETURN;
        END

        -- Deshabilitar todos los servicios del día feriado
        UPDATE SiteOperatingDayService
        SET IsEnabled = 0,
            Comment = 'Deshabilitado por feriado / Disabled by holiday',
            UpdatedAt = GETDATE()
        WHERE OperatingDayId = @operatingDayId;

    END TRY
    BEGIN CATCH
        SET @errorMessage = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO

