-- =============================================
-- Stored Procedure: 100_GetSiteOperatingDayServices
-- Descripción: Obtiene todos los servicios de un día de funcionamiento
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteOperatingDayServices]
    @operatingDayId INT
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

        -- Obtener servicios con información del tipo de servicio
        SELECT
        sods.Id,
        sods.OperatingDayId,
        sods.ServiceTypeId,
        st.Name AS ServiceTypeName,
        st.NameEN AS ServiceTypeNameEN,
        sods.ChildGroupId,
        scg.GroupName AS ChildGroupName,
        sods.StartTime,
        sods.EndTime,
        sods.IsEnabled,
        sods.Comment,
        sods.CreatedAt,
        sods.UpdatedAt
    FROM SiteOperatingDayService sods
        INNER JOIN ServiceType st ON sods.ServiceTypeId = st.Id
        LEFT JOIN SiteChildGroup scg ON sods.ChildGroupId = scg.Id
    WHERE sods.OperatingDayId = @operatingDayId
    ORDER BY st.DisplayOrder, sods.StartTime;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

