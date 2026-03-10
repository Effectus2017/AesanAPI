-- =============================================
-- Script: 105_DropDeleteSiteChildGroupsBySiteId
-- Descripción: Elimina el SP 100_DeleteSiteChildGroupsBySiteId del esquema.
--              El flujo de actualización de grupos usa merge (100_UpdateSiteChildGroup,
--              100_DeleteSiteChildGroupById, etc.) y ya no usa este SP.
-- Versión: 1.0
-- =============================================

IF OBJECT_ID('[dbo].[100_DeleteSiteChildGroupsBySiteId]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[100_DeleteSiteChildGroupsBySiteId];
