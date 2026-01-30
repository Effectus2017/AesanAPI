-- =============================================
-- Script: 108_DropSiteService
-- Descripción: Elimina la tabla SiteService y todos los stored procedures asociados.
-- La funcionalidad fue migrada a SiteChildGroupService (servicios por grupo).
-- Fecha: 2025-01-27
-- Versión: 1.0
-- =============================================

-- Eliminar stored procedures (orden irrelevante)
-- Se usa EXEC dinámico porque los nombres que empiezan por número dan error de sintaxis en DROP PROCEDURE directo.
IF OBJECT_ID(N'[dbo].[100_GetSiteServicesBySiteId]', N'P') IS NOT NULL
    EXEC('DROP PROCEDURE [dbo].[100_GetSiteServicesBySiteId]');

IF OBJECT_ID(N'[dbo].[100_InsertSiteService]', N'P') IS NOT NULL
    EXEC('DROP PROCEDURE [dbo].[100_InsertSiteService]');

IF OBJECT_ID(N'[dbo].[100_UpdateSiteService]', N'P') IS NOT NULL
    EXEC('DROP PROCEDURE [dbo].[100_UpdateSiteService]');

IF OBJECT_ID(N'[dbo].[100_DeleteSiteServiceById]', N'P') IS NOT NULL
    EXEC('DROP PROCEDURE [dbo].[100_DeleteSiteServiceById]');

IF OBJECT_ID(N'[dbo].[100_DeleteSiteServicesByChildGroupId]', N'P') IS NOT NULL
    EXEC('DROP PROCEDURE [dbo].[100_DeleteSiteServicesByChildGroupId]');

IF OBJECT_ID(N'[dbo].[100_DeleteSiteServicesBySiteId]', N'P') IS NOT NULL
    EXEC('DROP PROCEDURE [dbo].[100_DeleteSiteServicesBySiteId]');

-- Eliminar la tabla (las FK se eliminan automáticamente)
IF OBJECT_ID(N'[dbo].[SiteService]', N'U') IS NOT NULL
    DROP TABLE [dbo].[SiteService];
