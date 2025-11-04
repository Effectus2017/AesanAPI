-- =============================================
-- Stored Procedure: 100_ValidateServiceTimeRange
-- Descripción: Valida que los horarios del servicio estén dentro del rango del día de funcionamiento
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_ValidateServiceTimeRange]
    @operatingDayId INT,
    @startTime TIME,
    @endTime TIME,
    @isValid BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar parámetros
        IF @operatingDayId IS NULL OR @operatingDayId <= 0
        BEGIN
            RAISERROR('OperatingDayId es requerido y debe ser mayor a 0', 16, 1);
            RETURN;
        END

        IF @startTime IS NULL OR @endTime IS NULL
        BEGIN
            SET @isValid = 0;
            RETURN;
        END

        -- Validar que los horarios estén dentro del rango del día de funcionamiento
        SELECT @isValid = CASE 
            WHEN @startTime >= StartTime 
                AND @endTime <= EndTime 
                AND @startTime < @endTime
                AND IsExcluded = 0
                AND IsActive = 1
            THEN 1 
            ELSE 0 
        END
        FROM SiteOperatingDays
        WHERE Id = @operatingDayId;

        -- Si no se encontró el día, retornar 0
        IF @isValid IS NULL
        BEGIN
            SET @isValid = 0;
        END

    END TRY
    BEGIN CATCH
        DECLARE @errorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        SET @isValid = 0;
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO

