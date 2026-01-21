-- Stored Procedure para insertar un día de funcionamiento específico
-- Crea un nuevo día de funcionamiento para un sitio
-- Maneja automáticamente fines de semana y feriados
CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteOperatingDay]
    @siteId INT,
    @operatingDate DATE,
    @startTime TIME = NULL,
    @endTime TIME = NULL,
    @isWeekend BIT = 0,
    @isHoliday BIT = 0,
    @comment NVARCHAR(500) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DayOfWeek INT;

    BEGIN TRY
        -- Validar parámetros
        IF @SiteId IS NULL OR @SiteId <= 0
        BEGIN
        RAISERROR('SiteId es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        IF @OperatingDate IS NULL
        BEGIN
        RAISERROR('OperatingDate es requerido', 16, 1);
        RETURN;
    END

        -- Verificar que el sitio existe
        IF NOT EXISTS (SELECT 1
    FROM Site
    WHERE Id = @SiteId)
        BEGIN
        RAISERROR('El sitio especificado no existe', 16, 1);
        RETURN;
    END

        -- Verificar que no existe ya un día para esta fecha y sitio
        IF EXISTS (SELECT 1
    FROM SiteOperatingDays
    WHERE SiteId = @SiteId AND OperatingDate = @OperatingDate)
        BEGIN
        RAISERROR('Ya existe un día de funcionamiento para esta fecha y sitio', 16, 1);
        RETURN;
    END

        -- Obtener día de la semana (1=Domingo, 2=Lunes, ..., 7=Sábado)
        SET @DayOfWeek = DATEPART(WEEKDAY, @operatingDate);
        -- Si no se proporciona @isWeekend, calcularlo automáticamente
        IF @isWeekend IS NULL
        BEGIN
        SET @isWeekend = CASE WHEN @DayOfWeek IN (1, 7) THEN 1 ELSE 0 END;
    END

        -- Establecer horarios por defecto si no se proporcionan
        IF @startTime IS NULL OR @endTime IS NULL
        BEGIN
        SET @startTime = ISNULL(@startTime, '08:00:00');
        SET @endTime = ISNULL(@endTime, '18:00:00');
    END

        -- Establecer comentario por defecto si no se proporciona
        IF @comment IS NULL
        BEGIN
        SET @comment = CASE 
                WHEN @isHoliday = 1 THEN 'Feriado - Funciona por excepción'
                WHEN @isWeekend = 1 THEN 'Fin de semana - Funciona por excepción'
                ELSE 'Día de funcionamiento'
            END
    END

        -- Insertar el día
        -- IsManuallyAdded = 1 porque este día es agregado manualmente desde el calendario
        INSERT INTO SiteOperatingDays
        (
        SiteId,
        OperatingDate,
        StartTime,
        EndTime,
        IsWeekend,
        IsHoliday,
        Comment,
        IsActive,
        IsManuallyAdded,
        CreatedAt,
        UpdatedAt
        )
    VALUES
        (
            @siteId,
            @operatingDate,
            @startTime,
            @endTime,
            @isWeekend,
            @isHoliday,
            @comment,
            1,
            1, -- IsManuallyAdded = 1 (día agregado manualmente en el calendario)
            GETDATE(),
            GETDATE()
        );

        -- Obtener el ID del día insertado
        SET @id = SCOPE_IDENTITY();

        -- Recalcular días de funcionamiento del sitio
        EXEC [dbo].[100_ReCalculateSiteOperatingDays] @siteId;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        SET @id = NULL;
    END CATCH
END

