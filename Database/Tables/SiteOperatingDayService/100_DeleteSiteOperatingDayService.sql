-- =============================================
-- Stored Procedure: 100_DeleteSiteOperatingDayService
-- Descripción: Elimina un servicio de alimentación de un día de funcionamiento
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteOperatingDayService]
    @id INT
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

        -- Verificar que el servicio existe
        IF NOT EXISTS (SELECT 1
    FROM SiteOperatingDayService
    WHERE Id = @id)
        BEGIN
        RAISERROR('El servicio especificado no existe', 16, 1);
        RETURN;
    END

        -- Eliminar el servicio
        DELETE FROM SiteOperatingDayService
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

