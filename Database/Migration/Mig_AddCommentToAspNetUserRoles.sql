-- =============================================
-- Migración: Agregar columna Comment a AspNetUserRoles
-- Descripción: Motivo por el cual se asigna un rol secundario (opcional).
-- =============================================

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AspNetUserRoles')
BEGIN
    PRINT 'Tabla AspNetUserRoles no existe. Saltando migración.';
    RETURN;
END

BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUserRoles') AND name = 'Comment')
    BEGIN
        ALTER TABLE AspNetUserRoles ADD Comment NVARCHAR(500) NULL;
        PRINT 'Columna Comment agregada a AspNetUserRoles.';
    END

    PRINT 'Migración Mig_AddCommentToAspNetUserRoles completada.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH
GO
