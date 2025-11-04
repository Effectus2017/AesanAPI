-- =============================================
-- Stored Procedure: 100_UpdateSiteOperatingDayService
-- Descripción: Actualiza un servicio de alimentación para un día de funcionamiento
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSiteOperatingDayService]
    @id INT,
    @startTime TIME = NULL,
    @endTime TIME = NULL,
    @isEnabled BIT = NULL,
    @comment NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @dayStartTime TIME;
    DECLARE @dayEndTime TIME;
    DECLARE @currentStartTime TIME;
    DECLARE @currentEndTime TIME;
    DECLARE @operatingDayId INT;
    DECLARE @errorMessage NVARCHAR(4000);

    BEGIN TRY
        -- Validar parámetros
        IF @id IS NULL OR @id <= 0
        BEGIN
        RAISERROR('Id es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        -- Verificar que el servicio existe
        SELECT
        @operatingDayId = OperatingDayId,
        @currentStartTime = StartTime,
        @currentEndTime = EndTime
    FROM SiteOperatingDayService
    WHERE Id = @id;

        IF @operatingDayId IS NULL
        BEGIN
        RAISERROR('El servicio especificado no existe', 16, 1);
        RETURN;
    END

        -- Obtener horarios del día de funcionamiento
        SELECT
        @dayStartTime = StartTime,
        @dayEndTime = EndTime
    FROM SiteOperatingDays
    WHERE Id = @operatingDayId;

        -- Usar valores actuales si no se proporcionan nuevos
        SET @startTime = ISNULL(@startTime, @currentStartTime);
        SET @endTime = ISNULL(@endTime, @currentEndTime);

        -- Validar horarios
        IF @startTime >= @endTime
        BEGIN
        RAISERROR('StartTime debe ser menor que EndTime', 16, 1);
        RETURN;
    END

        -- Validar que los horarios estén dentro del rango del día
        IF @startTime < @dayStartTime OR @endTime > @dayEndTime
        BEGIN
        SET @errorMessage = 'Los horarios del servicio deben estar dentro del rango del día de funcionamiento (' + 
                CONVERT(VARCHAR(8), @dayStartTime) + ' - ' + CONVERT(VARCHAR(8), @dayEndTime) + ')';
        RAISERROR(@errorMessage, 16, 1);
        RETURN;
    END

        -- Actualizar solo los campos proporcionados
        UPDATE SiteOperatingDayService
        SET 
            StartTime = @startTime,
            EndTime = @endTime,
            IsEnabled = ISNULL(@isEnabled, IsEnabled),
            Comment = ISNULL(@comment, Comment),
            UpdatedAt = GETDATE()
        WHERE Id = @id;

    END TRY
    BEGIN CATCH
        SET @errorMessage = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO

