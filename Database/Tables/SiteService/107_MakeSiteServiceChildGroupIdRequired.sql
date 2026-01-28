-- =============================================
-- Script: 107_MakeSiteServiceChildGroupIdRequired
-- Descripción: Hace obligatorio el campo ChildGroupId en SiteService
-- Todos los servicios deben estar asociados a un grupo
-- Fecha: 2025-01-25
-- Versión: 1.0
-- =============================================

-- Primero, eliminar servicios que no tienen grupo asociado (si existen)
DELETE FROM SiteService WHERE ChildGroupId IS NULL;

-- Hacer el campo NOT NULL
ALTER TABLE [SiteService]
ALTER COLUMN [ChildGroupId] [int] NOT NULL;
