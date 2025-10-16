-- Stored Procedure para alternar el estado de funcionamiento de un día específico
-- Inserta o actualiza un día de funcionamiento para una escuela
-- Maneja automáticamente fines de semana y días excluidos
CREATE OR ALTER PROCEDURE [dbo].[100_ToggleSchoolOperatingDay]
    @schoolId INT,
    @operatingDate DATE,
    @startTime TIME = NULL,
    @endTime TIME = NULL,
    @isWeekendOverride BIT = 0,
    @isExcluded BIT = 0,
    @comment NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DayOfWeek INT;
    DECLARE @IsWeekend BIT;
    DECLARE @ExistingId INT;
    DECLARE @RowsAffected INT = 0;

    BEGIN TRY
        -- Validar parámetros
        IF @SchoolId IS NULL OR @SchoolId <= 0
        BEGIN
        RAISERROR('SchoolId es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        IF @OperatingDate IS NULL
        BEGIN
        RAISERROR('OperatingDate es requerido', 16, 1);
        RETURN;
    END

        -- Verificar que el sitio existe
        IF NOT EXISTS (SELECT 1
    FROM School
    WHERE Id = @SchoolId)
        BEGIN
        RAISERROR('El sitio especificado no existe', 16, 1);
        RETURN;
    END

        -- Obtener día de la semana (1=Domingo, 2=Lunes, ..., 7=Sábado)
        SET @DayOfWeek = DATEPART(WEEKDAY, @operatingDate);
        SET @IsWeekend = CASE WHEN @DayOfWeek IN (1, 7) THEN 1 ELSE 0 END;

        -- Verificar si ya existe un registro para esta fecha
        SELECT @ExistingId = Id
    FROM SchoolOperatingDays
    WHERE SchoolId = @schoolId AND OperatingDate = @operatingDate;

        -- Si no está excluido, establecer horarios por defecto si no se proporcionan
        IF @isExcluded = 0 AND (@startTime IS NULL OR @endTime IS NULL)
        BEGIN
        SET @startTime = ISNULL(@startTime, '08:00:00');
        SET @endTime = ISNULL(@endTime, '16:00:00');
    END

        -- Mantener horarios siempre, incluso si está excluido
        -- Los horarios se mantienen para referencia y consistencia

        -- Establecer comentario por defecto si no se proporciona
        IF @comment IS NULL
        BEGIN
        IF @isExcluded = 1
                SET @comment = CASE 
                    WHEN @IsWeekend = 1 THEN 'Fin de semana - No funciona'
                    ELSE 'Día excluido - No funciona'
                END
            ELSE
                SET @comment = CASE 
                    WHEN @isWeekendOverride = 1 THEN 'Fin de semana - Funciona por excepción'
                    ELSE 'Día de funcionamiento'
                END
    END

        -- Si existe, actualizar; si no, insertar
        IF @ExistingId IS NOT NULL
        BEGIN
        UPDATE SchoolOperatingDays
            SET 
                StartTime = @startTime,
                EndTime = @endTime,
                IsWeekendOverride = @isWeekendOverride,
                IsExcluded = @isExcluded,
                Comment = @comment,
                UpdatedAt = GETDATE()
            WHERE Id = @ExistingId;

        SET @RowsAffected = @@ROWCOUNT;
    END
        ELSE
        BEGIN
        INSERT INTO SchoolOperatingDays
            (
            SchoolId,
            OperatingDate,
            StartTime,
            EndTime,
            IsWeekendOverride,
            IsExcluded,
            Comment,
            CreatedAt,
            UpdatedAt
            )
        VALUES
            (
                @SchoolId,
                @OperatingDate,
                @startTime,
                @endTime,
                @isWeekendOverride,
                @isExcluded,
                @comment,
                GETDATE(),
                GETDATE()
            );

        SET @RowsAffected = @@ROWCOUNT;
    END

        -- Retornar el número de filas afectadas
        SELECT @RowsAffected as RowsAffected;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
