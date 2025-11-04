-- =============================================
-- Stored Procedure: 100_ToggleSiteOperatingDayService
-- Descripción: Habilita o deshabilita un servicio de alimentación de un día
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_ToggleSiteOperatingDayService]
    @id INT,
    @isEnabled BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar parámetros
        IF @id IS NULL OR @id <= 0
        BEGIN
        RAISERROR('Id es requerido y debe ser mayor a 0', 16, 1);
        RETURN;
    END

        IF @isEnabled IS NULL
        BEGIN
        RAISERROR('IsEnabled es requerido', 16, 1);
        RETURN;
    END

        -- Verificar que el servicio existe
        IF NOT EXISTS (SELECT 1
    FROM SiteOperatingDayService
    WHERE Id = @id)
        BEGIN
        RAISERROR('El servicio especificado no existe', 16, 1);
        RETURN;
    END

        -- Actualizar el estado
        UPDATE SiteOperatingDayService
        SET 
            IsEnabled = @isEnabled,
            UpdatedAt = GETDATE()
        WHERE Id = @id;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

