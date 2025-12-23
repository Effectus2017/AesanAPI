-- =============================================
-- Stored Procedure: 100_EnableServicesForNonHolidayDay
-- Descripción: Habilita todos los servicios de un día que ya no es feriado
-- Fecha: 2025-01-XX
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_EnableServicesForNonHolidayDay]
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

        -- Verificar que el día existe y NO es feriado
        IF NOT EXISTS (
            SELECT 1
    FROM SiteOperatingDays
    WHERE Id = @operatingDayId AND IsHoliday = 0
        )
        BEGIN
        RAISERROR('El día especificado no existe o aún es un día feriado', 16, 1);
        RETURN;
    END

        -- Habilitar todos los servicios del día que fueron deshabilitados por feriado
        -- Solo actualizar servicios que tengan el comentario específico de feriado
        UPDATE SiteOperatingDayService
        SET IsEnabled = 1,
            Comment = NULL,
            UpdatedAt = GETDATE()
        WHERE OperatingDayId = @operatingDayId
        AND Comment = 'Deshabilitado por feriado / Disabled by holiday';

    END TRY
    BEGIN CATCH
        SET @errorMessage = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO

