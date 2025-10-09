-- =============================================
-- Stored Procedure: 100_InsertSiteFacilities
-- Descripción: Inserta múltiples instalaciones para un sitio
-- Reemplaza: 100_InsertSchoolFacilities
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSiteFacilities]
    @siteId INT,
    @facilityTypeIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear tabla temporal para los IDs
    DECLARE @FacilityTypeTable TABLE (FacilityTypeId INT);

    -- Insertar IDs en la tabla temporal
    INSERT INTO @FacilityTypeTable
        (FacilityTypeId)
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@facilityTypeIds, ',')
    WHERE value IS NOT NULL AND value != '';

    -- Insertar instalaciones
    INSERT INTO SiteFacility
        (SiteId, FacilityTypeId, IsActive, CreatedAt)
    SELECT @siteId, FacilityTypeId, 1, GETDATE()
    FROM @FacilityTypeTable;
END;
