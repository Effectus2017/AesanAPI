-- =============================================
-- Stored Procedure: 100_InsertSiteOperatingDayService
-- Descripción: Inserta un servicio de alimentación para un día de funcionamiento específico
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteOperatingDayService]
    @operatingDayId INT,
    @serviceTypeId INT,
    @childGroupId INT = NULL,
    @startTime TIME,
    @endTime TIME,
    @isEnabled BIT = 1,
    @comment NVARCHAR(500) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @dayStartTime TIME;
    DECLARE @dayEndTime TIME;
    DECLARE @dayIsHoliday BIT;
    DECLARE @dayIsActive BIT;
    DECLARE @errorMessage NVARCHAR(4000);

    BEGIN TRY
        -- Validar parámetros
        IF @operatingDayId IS NULL OR @operatingDayId <= 0
        BEGIN
        RAISERROR('OperatingDayId es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        IF @serviceTypeId IS NULL OR @serviceTypeId <= 0
        BEGIN
        RAISERROR('ServiceTypeId es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        IF @startTime IS NULL OR @endTime IS NULL
        BEGIN
        RAISERROR('StartTime y EndTime son requeridos', 16, 1);
        RETURN;
    END

        IF @startTime >= @endTime
        BEGIN
        RAISERROR('StartTime debe ser menor que EndTime', 16, 1);
        RETURN;
    END

        -- Verificar que el día de funcionamiento existe y obtener sus horarios
        SELECT
        @dayStartTime = StartTime,
        @dayEndTime = EndTime,
        @dayIsHoliday = IsHoliday,
        @dayIsActive = IsActive
    FROM SiteOperatingDays
    WHERE Id = @operatingDayId;

        IF @dayStartTime IS NULL
        BEGIN
        RAISERROR('El día de funcionamiento especificado no existe', 16, 1);
        RETURN;
    END

        -- Validar que el día no es feriado o inactivo
        IF @dayIsHoliday = 1 OR @dayIsActive = 0
        BEGIN
        RAISERROR('No se pueden crear servicios para días feriados o inactivos', 16, 1);
        RETURN;
    END

        -- Validar que los horarios del servicio estén dentro del rango del día
        IF @startTime < @dayStartTime OR @endTime > @dayEndTime
        BEGIN
        SET @errorMessage = 'Los horarios del servicio deben estar dentro del rango del día de funcionamiento (' + 
                CONVERT(VARCHAR(8), @dayStartTime) + ' - ' + CONVERT(VARCHAR(8), @dayEndTime) + ')';
        RAISERROR(@errorMessage, 16, 1);
        RETURN;
    END

        -- Validar que ServiceTypeId existe en ServiceType
        IF NOT EXISTS (SELECT 1
    FROM ServiceType
    WHERE Id = @serviceTypeId AND IsActive = 1)
        BEGIN
        RAISERROR('El ServiceTypeId especificado no existe o no está activo', 16, 1);
        RETURN;
    END

        -- Validar que no existe ya este servicio para este día (unique constraint)
        IF EXISTS (
            SELECT 1
    FROM SiteOperatingDayService
    WHERE OperatingDayId = @operatingDayId
        AND ServiceTypeId = @serviceTypeId
        AND (@childGroupId IS NULL AND ChildGroupId IS NULL OR ChildGroupId = @childGroupId)
        )
        BEGIN
        RAISERROR('Ya existe este servicio para este día y grupo', 16, 1);
        RETURN;
    END

        -- Insertar el servicio
        INSERT INTO SiteOperatingDayService
        (
        OperatingDayId,
        ServiceTypeId,
        ChildGroupId,
        StartTime,
        EndTime,
        IsEnabled,
        Comment,
        CreatedAt
        )
    VALUES
        (
            @operatingDayId,
            @serviceTypeId,
            @childGroupId,
            @startTime,
            @endTime,
            @isEnabled,
            @comment,
            GETDATE()
            );

        SET @id = SCOPE_IDENTITY();

    END TRY
    BEGIN CATCH
        SET @errorMessage = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO

