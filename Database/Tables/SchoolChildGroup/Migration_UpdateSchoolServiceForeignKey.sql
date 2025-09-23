-- =============================================
-- Script de migración: Actualizar foreign key de SchoolService
-- Descripción: Cambiar ChildGroupId de OptionSelection a SchoolChildGroup
-- Fecha: 2025-01-15
-- =============================================

-- Paso 1: Eliminar la foreign key existente
IF EXISTS (SELECT 1
FROM sys.foreign_keys
WHERE name = 'FK_SchoolService_ChildGroupId')
BEGIN
    ALTER TABLE SchoolService DROP CONSTRAINT FK_SchoolService_ChildGroupId;
    PRINT 'Foreign key FK_SchoolService_ChildGroupId eliminada.';
END

-- Paso 2: Crear la nueva foreign key que apunta a SchoolChildGroup
ALTER TABLE SchoolService 
ADD CONSTRAINT FK_SchoolService_ChildGroupId 
FOREIGN KEY (ChildGroupId) REFERENCES SchoolChildGroup(Id) ON DELETE CASCADE;

PRINT 'Foreign key FK_SchoolService_ChildGroupId creada apuntando a SchoolChildGroup.';

-- Paso 3: Actualizar el comentario en la tabla
EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del grupo de niños específico. NULL = servicio general, NOT NULL = servicio específico por grupo', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SchoolService', 
    @level2type = N'COLUMN', @level2name = N'ChildGroupId';

PRINT 'Comentario actualizado para ChildGroupId.';
