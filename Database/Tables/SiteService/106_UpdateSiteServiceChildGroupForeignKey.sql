-- =============================================
-- Script: 106_UpdateSiteServiceChildGroupForeignKey
-- Descripción: Actualiza la foreign key de SiteService.ChildGroupId para referenciar SiteChildGroup en lugar de OptionSelection
-- Fecha: 2025-01-25
-- Versión: 1.0
-- =============================================

-- Eliminar la constraint existente
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SiteService_ChildGroup')
BEGIN
    ALTER TABLE [SiteService] DROP CONSTRAINT [FK_SiteService_ChildGroup];
END;

-- Crear la nueva constraint que referencia a SiteChildGroup
ALTER TABLE [SiteService] 
ADD CONSTRAINT [FK_SiteService_ChildGroup] 
FOREIGN KEY([ChildGroupId]) 
REFERENCES [SiteChildGroup]([Id]) 
ON DELETE CASCADE;
