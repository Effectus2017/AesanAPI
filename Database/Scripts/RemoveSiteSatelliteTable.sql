-- =============================================
-- Script: Eliminar Tabla SiteSatellite
-- Descripción: Elimina completamente la tabla SiteSatellite y sus relaciones
-- Fecha: 2025-10-15
-- Versión: 1.0
-- =============================================

-- Eliminar foreign keys primero
IF EXISTS (SELECT *
FROM sys.foreign_keys
WHERE name = 'FK_SiteSatellite_MainSite')
    ALTER TABLE [SiteSatellite] DROP CONSTRAINT [FK_SiteSatellite_MainSite];

IF EXISTS (SELECT *
FROM sys.foreign_keys
WHERE name = 'FK_SiteSatellite_SatelliteSite')
    ALTER TABLE [SiteSatellite] DROP CONSTRAINT [FK_SiteSatellite_SatelliteSite];

-- Eliminar índices
IF EXISTS (SELECT *
FROM sys.indexes
WHERE name = 'IX_SiteSatellite_MainSiteId')
    DROP INDEX [IX_SiteSatellite_MainSiteId] ON [SiteSatellite];

IF EXISTS (SELECT *
FROM sys.indexes
WHERE name = 'IX_SiteSatellite_SatelliteSiteId')
    DROP INDEX [IX_SiteSatellite_SatelliteSiteId] ON [SiteSatellite];

IF EXISTS (SELECT *
FROM sys.indexes
WHERE name = 'IX_SiteSatellite_IsActive')
    DROP INDEX [IX_SiteSatellite_IsActive] ON [SiteSatellite];

IF EXISTS (SELECT *
FROM sys.indexes
WHERE name = 'IX_SiteSatellite_AssignmentDate')
    DROP INDEX [IX_SiteSatellite_AssignmentDate] ON [SiteSatellite];

-- Eliminar tabla
IF EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SiteSatellite')
    DROP TABLE [SiteSatellite];

PRINT 'Tabla SiteSatellite eliminada exitosamente';
