-- =============================================
-- Script: Insert School Permissions
-- Descripción: Inserta los permisos CRUD para escuelas
-- Fecha: 2025-01-15
-- Versión: 1.0
-- =============================================

-- Insertar permisos para escuelas (solo si no existen)
IF NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'school.view')
BEGIN
    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), 'school.view', 'Ver escuelas', 'View schools', 1, GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'school.create')
BEGIN
    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), 'school.create', 'Crear escuelas', 'Create schools', 1, GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'school.edit')
BEGIN
    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), 'school.edit', 'Editar escuelas', 'Edit schools', 1, GETDATE(), GETDATE());
END

IF NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'school.delete')
BEGIN
    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), 'school.delete', 'Eliminar escuelas', 'Delete schools', 1, GETDATE(), GETDATE());
END

PRINT 'School permissions inserted successfully';
