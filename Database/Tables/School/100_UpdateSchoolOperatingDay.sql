-- Stored Procedure para actualizar un día de funcionamiento específico
-- Permite actualizar horarios, comentarios y estado de un día existente
CREATE OR ALTER PROCEDURE [dbo].[100_UpdateSchoolOperatingDay]
    @id INT,
    @startTime TIME = NULL,
    @endTime TIME = NULL,
    @isWeekendOverride BIT = NULL,
    @isExcluded BIT = NULL,
    @comment NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RowsAffected INT = 0;

    BEGIN TRY
        -- Validar parámetros
        IF @id IS NULL OR @id <= 0
        BEGIN
        RAISERROR('Id es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        -- Verificar que el registro existe
        IF NOT EXISTS (SELECT 1
    FROM SchoolOperatingDays
    WHERE Id = @id)
        BEGIN
        RAISERROR('El día de funcionamiento especificado no existe', 16, 1);
        RETURN;
    END

        -- Actualizar solo los campos proporcionados
        UPDATE SchoolOperatingDays
        SET 
            StartTime = CASE WHEN @startTime IS NOT NULL THEN @startTime ELSE StartTime END,
            EndTime = CASE WHEN @endTime IS NOT NULL THEN @endTime ELSE EndTime END,
            IsWeekendOverride = CASE WHEN @isWeekendOverride IS NOT NULL THEN @isWeekendOverride ELSE IsWeekendOverride END,
            IsExcluded = CASE WHEN @isExcluded IS NOT NULL THEN @isExcluded ELSE IsExcluded END,
            Comment = CASE WHEN @comment IS NOT NULL THEN @comment ELSE Comment END,
            UpdatedAt = GETDATE()
        WHERE Id = @id;

        SET @RowsAffected = @@ROWCOUNT;

        -- Mantener horarios siempre, incluso si está excluido
        -- Los horarios se mantienen para referencia y consistencia

        -- Si se desmarcó como excluido y no tiene horarios, establecer por defecto
        IF @isExcluded = 0 AND @RowsAffected > 0
        BEGIN
        UPDATE SchoolOperatingDays
            SET 
                StartTime = CASE WHEN StartTime IS NULL THEN '08:00:00' ELSE StartTime END,
                EndTime = CASE WHEN EndTime IS NULL THEN '16:00:00' ELSE EndTime END,
                UpdatedAt = GETDATE()
            WHERE Id = @id AND (StartTime IS NULL OR EndTime IS NULL);
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
