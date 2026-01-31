-- =============================================
-- Stored Procedure: 100_GetSiteOperatingDayServicesBatch
-- Descripción: Obtiene servicios para múltiples días de funcionamiento en una sola consulta
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteOperatingDayServicesBatch]
    @operatingDayIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar parámetros
        IF @operatingDayIds IS NULL OR @operatingDayIds = ''
        BEGIN
        SELECT
            NULL as Id,
            NULL as OperatingDayId,
            NULL as ServiceTypeId,
            NULL as ServiceTypeName,
            NULL as ServiceTypeNameEN,
            NULL as ChildGroupId,
            NULL as ChildGroupName,
            NULL as StartTime,
            NULL as EndTime,
            NULL as IsEnabled,
            NULL as Comment,
            NULL as CreatedAt,
            NULL as UpdatedAt
        WHERE 1 = 0;
        -- Retornar estructura vacía
        RETURN;
    END

        -- Crear tabla temporal para los IDs
        DECLARE @OperatingDayTable TABLE (OperatingDayId INT);

        -- Insertar IDs en la tabla temporal
        INSERT INTO @OperatingDayTable
        (OperatingDayId)
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@operatingDayIds, ',')
    WHERE value IS NOT NULL AND value != '';

        -- Obtener servicios con información del tipo de servicio y grupo
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
        INNER JOIN SiteChildGroup scg ON sods.ChildGroupId = scg.Id
        INNER JOIN @OperatingDayTable ids ON sods.OperatingDayId = ids.OperatingDayId
    WHERE sods.ChildGroupId IS NOT NULL
    ORDER BY sods.OperatingDayId, st.DisplayOrder, sods.StartTime;

    END TRY
    BEGIN CATCH
        DECLARE @errorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO

