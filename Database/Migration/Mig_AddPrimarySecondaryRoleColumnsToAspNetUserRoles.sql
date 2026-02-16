-- =============================================
-- Migración: Agregar columnas rol principal/secundario y vigencia a AspNetUserRoles
-- Descripción: IsPrimary (1 = principal, 0 = secundario), ValidFrom/ValidTo para secundarios.
--              Un usuario tiene exactamente un rol principal (IsPrimary=1, ValidFrom/ValidTo NULL)
--              y puede tener N roles secundarios (IsPrimary=0, ValidFrom/ValidTo definidos).
-- =============================================

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AspNetUserRoles')
BEGIN
    PRINT 'Tabla AspNetUserRoles no existe. Saltando migración.';
    RETURN;
END

BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUserRoles') AND name = 'IsPrimary')
    BEGIN
        ALTER TABLE AspNetUserRoles ADD IsPrimary BIT NOT NULL DEFAULT 1;
        PRINT 'Columna IsPrimary agregada a AspNetUserRoles.';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUserRoles') AND name = 'ValidFrom')
    BEGIN
        ALTER TABLE AspNetUserRoles ADD ValidFrom DATE NULL;
        PRINT 'Columna ValidFrom agregada a AspNetUserRoles.';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUserRoles') AND name = 'ValidTo')
    BEGIN
        ALTER TABLE AspNetUserRoles ADD ValidTo DATE NULL;
        PRINT 'Columna ValidTo agregada a AspNetUserRoles.';
    END

    PRINT 'Migración Mig_AddPrimarySecondaryRoleColumnsToAspNetUserRoles completada.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH
GO
