-- =============================================
-- Script de Migración: AddIsDayCareHomeIdToSite
-- Descripción: Agrega columna IsDayCareHomeId a la tabla Site y migra datos existentes
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- Paso 1: Agregar columna IsDayCareHomeId a la tabla Site
IF NOT EXISTS (SELECT *
FROM sys.columns
WHERE object_id = OBJECT_ID(N'[dbo].[Site]') AND name = 'IsDayCareHomeId')
BEGIN
    ALTER TABLE [dbo].[Site]
    ADD [IsDayCareHomeId] [int] NULL;

    PRINT 'Columna IsDayCareHomeId agregada a la tabla Site';
END
ELSE
BEGIN
    PRINT 'La columna IsDayCareHomeId ya existe en la tabla Site';
END
GO

-- Paso 2: Agregar Foreign Key si no existe
IF NOT EXISTS (SELECT *
FROM sys.foreign_keys
WHERE name = 'FK_Site_IsDayCareHome')
BEGIN
    ALTER TABLE [dbo].[Site]
    ADD CONSTRAINT [FK_Site_IsDayCareHome] FOREIGN KEY([IsDayCareHomeId]) REFERENCES [OptionSelection]([Id]);

    PRINT 'Foreign Key FK_Site_IsDayCareHome agregada';
END
ELSE
BEGIN
    PRINT 'La Foreign Key FK_Site_IsDayCareHome ya existe';
END
GO

-- Paso 3: Agregar índice si no existe
IF NOT EXISTS (SELECT *
FROM sys.indexes
WHERE name = 'IX_Site_IsDayCareHomeId' AND object_id = OBJECT_ID(N'[dbo].[Site]'))
BEGIN
    CREATE INDEX [IX_Site_IsDayCareHomeId] ON [Site]([IsDayCareHomeId]);

    PRINT 'Índice IX_Site_IsDayCareHomeId creado';
END
ELSE
BEGIN
    PRINT 'El índice IX_Site_IsDayCareHomeId ya existe';
END
GO

-- Paso 4: Migrar datos existentes
-- Obtener IDs de OptionSelection para isDayCareHome
DECLARE @NoOptionId INT;
DECLARE @SiOptionId INT;
DECLARE @AmbosOptionId INT;

SELECT @NoOptionId = Id
FROM OptionSelection
WHERE OptionKey = 'isDayCareHome' AND BooleanValue = 0 AND Name = 'No';
SELECT @SiOptionId = Id
FROM OptionSelection
WHERE OptionKey = 'isDayCareHome' AND BooleanValue = 1 AND Name = 'Sí';
SELECT @AmbosOptionId = Id
FROM OptionSelection
WHERE OptionKey = 'isDayCareHome' AND BooleanValue IS NULL AND Name = 'Ambos';

PRINT 'IDs de OptionSelection:';
PRINT '  No: ' + ISNULL(CAST(@NoOptionId AS VARCHAR), 'NULL');
PRINT '  Sí: ' + ISNULL(CAST(@SiOptionId AS VARCHAR), 'NULL');
PRINT '  Ambos: ' + ISNULL(CAST(@AmbosOptionId AS VARCHAR), 'NULL');

-- Migrar datos basándose en AgencyInscription y SiteDayCareHome
UPDATE s
SET s.IsDayCareHomeId = CASE
    -- Si la agencia tiene IsDayCareHomeId = "No", asignar "No" a todos sus sitios
    WHEN ai.IsDayCareHomeId = @NoOptionId THEN @NoOptionId
    -- Si la agencia tiene IsDayCareHomeId = "Sí", asignar "Sí" a todos sus sitios
    WHEN ai.IsDayCareHomeId = @SiOptionId THEN @SiOptionId
    -- Si la agencia tiene IsDayCareHomeId = "Ambos":
    --   - Si existe SiteDayCareHome → asignar "Sí"
    --   - Si no existe → asignar "No"
    WHEN ai.IsDayCareHomeId = @AmbosOptionId THEN
        CASE
            WHEN sdch.Id IS NOT NULL THEN @SiOptionId
            ELSE @NoOptionId
        END
    -- Si no hay información de la agencia, dejar NULL
    ELSE NULL
END
FROM Site s
    LEFT JOIN Agency a ON s.AgencyId = a.Id
    LEFT JOIN AgencyInscription ai ON a.Id = ai.AgencyId
    LEFT JOIN SiteDayCareHome sdch ON s.Id = sdch.SiteId
WHERE s.IsDayCareHomeId IS NULL;
-- Solo actualizar sitios que aún no tienen valor

DECLARE @RowsAffected INT = @@ROWCOUNT;
PRINT 'Migración completada. Filas actualizadas: ' + CAST(@RowsAffected AS VARCHAR);

-- Verificar resultados
SELECT
    ai.IsDayCareHomeId AS AgencyIsDayCareHomeId,
    os_agency.Name AS AgencyIsDayCareHomeName,
    COUNT(*) AS TotalSites,
    SUM(CASE WHEN s.IsDayCareHomeId = @NoOptionId THEN 1 ELSE 0 END) AS SitesWithNo,
    SUM(CASE WHEN s.IsDayCareHomeId = @SiOptionId THEN 1 ELSE 0 END) AS SitesWithSi,
    SUM(CASE WHEN s.IsDayCareHomeId IS NULL THEN 1 ELSE 0 END) AS SitesWithNull
FROM Site s
    LEFT JOIN Agency a ON s.AgencyId = a.Id
    LEFT JOIN AgencyInscription ai ON a.Id = ai.AgencyId
    LEFT JOIN OptionSelection os_agency ON ai.IsDayCareHomeId = os_agency.Id
WHERE s.IsActive = 1
GROUP BY ai.IsDayCareHomeId, os_agency.Name
ORDER BY ai.IsDayCareHomeId;

PRINT 'Script de migración completado exitosamente';
GO

