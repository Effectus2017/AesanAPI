-- =============================================
-- Migration: 307_RemoveAdministratorAuthorizedNameFromSiteDayCareHome
-- Descripción: Elimina la columna AdministratorAuthorizedName de la tabla SiteDayCareHome
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- Eliminar la columna AdministratorAuthorizedName de SiteDayCareHome
-- Esta columna debe estar únicamente en la tabla Site
IF EXISTS (SELECT *
FROM sys.columns
WHERE object_id = OBJECT_ID('SiteDayCareHome') AND name = 'AdministratorAuthorizedName')
BEGIN
    ALTER TABLE SiteDayCareHome DROP COLUMN AdministratorAuthorizedName;
    PRINT 'Columna AdministratorAuthorizedName eliminada de SiteDayCareHome';
END
ELSE
BEGIN
    PRINT 'La columna AdministratorAuthorizedName no existe en SiteDayCareHome';
END
