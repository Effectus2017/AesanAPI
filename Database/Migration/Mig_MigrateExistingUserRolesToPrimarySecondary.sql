-- =============================================
-- Migración: Marcar rol principal y secundarios en AspNetUserRoles (datos existentes)
-- Descripción: Para cada usuario con uno o más roles:
--   - Un solo rol: se deja como principal (IsPrimary=1, ValidFrom/ValidTo NULL).
--   - Varios roles: el primero por CreatedAt/RoleId se marca como principal;
--     el resto como secundarios (IsPrimary=0, ValidFrom=CreatedAt, ValidTo=9999-12-31).
-- Ejecutar después de Mig_AddPrimarySecondaryRoleColumnsToAspNetUserRoles.sql
-- =============================================

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AspNetUserRoles')
BEGIN
    PRINT 'Tabla AspNetUserRoles no existe. Saltando migración.';
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUserRoles') AND name = 'IsPrimary')
BEGIN
    PRINT 'Columna IsPrimary no existe. Ejecute primero Mig_AddPrimarySecondaryRoleColumnsToAspNetUserRoles.sql';
    RETURN;
END

BEGIN TRY
    BEGIN TRANSACTION;

    -- Marcar como secundarios todos los roles que NO son el "primero" por usuario (CreatedAt, RoleId)
    ;WITH ranked AS (
        SELECT UserId, RoleId,
               ROW_NUMBER() OVER (PARTITION BY UserId ORDER BY CreatedAt, RoleId) AS rn
        FROM AspNetUserRoles
    )
    UPDATE ur
    SET ur.IsPrimary = 0,
        ur.ValidFrom = CAST(ur.CreatedAt AS DATE),
        ur.ValidTo = CAST('9999-12-31' AS DATE),
        ur.UpdatedAt = GETDATE()
    FROM AspNetUserRoles ur
    INNER JOIN ranked r ON r.UserId = ur.UserId AND r.RoleId = ur.RoleId
    WHERE r.rn > 1;

    DECLARE @secondaryCount INT = @@ROWCOUNT;
    PRINT 'Roles marcados como secundarios: ' + CAST(@secondaryCount AS NVARCHAR(10));

    -- Asegurar que el rol "primero" por usuario tenga IsPrimary=1 y fechas NULL
    ;WITH ranked AS (
        SELECT UserId, RoleId,
               ROW_NUMBER() OVER (PARTITION BY UserId ORDER BY CreatedAt, RoleId) AS rn
        FROM AspNetUserRoles
    )
    UPDATE ur
    SET ur.IsPrimary = 1,
        ur.ValidFrom = NULL,
        ur.ValidTo = NULL,
        ur.UpdatedAt = GETDATE()
    FROM AspNetUserRoles ur
    INNER JOIN ranked r ON r.UserId = ur.UserId AND r.RoleId = ur.RoleId
    WHERE r.rn = 1;

    PRINT 'Roles marcados como principales: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));
    PRINT 'Migración Mig_MigrateExistingUserRolesToPrimarySecondary completada.';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
    PRINT 'Error: ' + @Msg;
    THROW;
END CATCH;
GO
