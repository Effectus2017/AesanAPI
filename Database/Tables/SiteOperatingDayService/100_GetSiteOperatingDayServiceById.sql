-- =============================================
-- Stored Procedure: 100_GetSiteOperatingDayServiceById
-- Descripción: Obtiene un servicio por su ID con información completa del día de funcionamiento
-- Fecha: 2025-03-14
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_GetSiteOperatingDayServiceById]
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

        -- Obtener servicio con información del tipo de servicio y día de funcionamiento
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
            sods.UpdatedAt,
            sod.OperatingDate,
            sod.StartTime AS DayStartTime,
            sod.EndTime AS DayEndTime
        FROM SiteOperatingDayService sods
            INNER JOIN SiteOperatingDays sod ON sods.OperatingDayId = sod.Id
            INNER JOIN ServiceType st ON sods.ServiceTypeId = st.Id
            LEFT JOIN SiteChildGroup scg ON sods.ChildGroupId = scg.Id
        WHERE sods.Id = @id;

    END TRY
    BEGIN CATCH
        DECLARE @errorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @errorSeverity INT = ERROR_SEVERITY();
        DECLARE @errorState INT = ERROR_STATE();
        
        RAISERROR(@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
GO

