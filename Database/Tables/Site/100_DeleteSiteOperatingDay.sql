-- =============================================
-- Stored Procedure: 100_DeleteSiteOperatingDay
-- Descripción: Elimina un día de funcionamiento específico
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_DeleteSiteOperatingDay]
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

        -- Verificar que el día de funcionamiento existe
        IF NOT EXISTS (SELECT 1
    FROM SiteOperatingDays
    WHERE Id = @id)
        BEGIN
        RAISERROR('El día de funcionamiento especificado no existe', 16, 1);
        RETURN;
    END

        -- Obtener SiteId antes de eliminar
        DECLARE @siteIdForRecalc INT;
        SELECT @siteIdForRecalc = SiteId
    FROM SiteOperatingDays
    WHERE Id = @id;

        -- Eliminar el día de funcionamiento
        -- CASCADE DELETE eliminará automáticamente los servicios relacionados (SiteOperatingDayService)
        DELETE FROM SiteOperatingDays
        WHERE Id = @id;

        -- Recalcular días de funcionamiento
        IF @siteIdForRecalc IS NOT NULL
        BEGIN
        EXEC [dbo].[100_ReCalculateSiteOperatingDays] @siteIdForRecalc;
    END

        -- Retornar el número de filas afectadas
        SELECT @@ROWCOUNT AS RowsAffected;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

