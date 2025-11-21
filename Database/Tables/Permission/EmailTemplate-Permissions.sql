/*
===========================================
Permisos para Email Templates
===========================================
Script para crear los permisos necesarios para gestionar templates de email.

Versión: 1.0
Fecha: 2025-01-15

Permisos creados:
- email.template.view: Ver templates de email
- email.template.edit: Editar templates de email
*/

-- Verificar que la tabla Permission exista
IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'Permission')
BEGIN
    RAISERROR('La tabla Permission no existe. Ejecute primero el script de creación de permisos.', 16, 1);
    RETURN;
END

PRINT 'Iniciando creación de permisos para Email Templates...';

BEGIN TRY
    BEGIN TRANSACTION;

    -- Permiso para ver templates de email
    IF NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'email.template.view')
    BEGIN
    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), 'email.template.view', 'Ver templates de email', 'View email templates', 1, GETDATE(), GETDATE());
    PRINT 'Permiso creado: email.template.view';
END
    ELSE
    BEGIN
    PRINT 'Permiso ya existe: email.template.view';
END

    -- Permiso para editar templates de email
    IF NOT EXISTS (SELECT 1
FROM Permission
WHERE ValueKey = 'email.template.edit')
    BEGIN
    INSERT INTO Permission
        (Id, ValueKey, Name, NameEn, IsActive, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), 'email.template.edit', 'Editar templates de email', 'Edit email templates', 1, GETDATE(), GETDATE());
    PRINT 'Permiso creado: email.template.edit';
END
    ELSE
    BEGIN
    PRINT 'Permiso ya existe: email.template.edit';
END

    COMMIT TRANSACTION;
    PRINT 'Permisos para Email Templates creados exitosamente.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error al crear permisos para Email Templates:';
    PRINT ERROR_MESSAGE();
    THROW;
END CATCH;
GO

