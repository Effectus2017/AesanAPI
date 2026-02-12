-- =============================================
-- Migración: Eliminar rol Monitor y migrar usuarios a rol NUTRE
-- Fecha: 2025-02-XX
-- Descripción: (1) Asigna rol Coordinador (o Coordinadora de Monitoría) a usuarios que tienen Monitor.
--              (2) Quita el rol Monitor de esos usuarios. (3) Elimina el rol Monitor de AspNetRoles.
--              El rol Monitor ya no existe; lista canónica NUTRE según PLAN_Usuarios_Roles_Cargos_NUTRE.
-- =============================================

SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AspNetRoles')
BEGIN
    PRINT 'Tabla AspNetRoles no existe. Saltando migración.';
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AspNetUserRoles')
BEGIN
    PRINT 'Tabla AspNetUserRoles no existe. Saltando migración.';
    RETURN;
END

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @MonitorRoleId NVARCHAR(450);
    DECLARE @TargetRoleId NVARCHAR(450);

    SELECT @MonitorRoleId = Id FROM AspNetRoles WHERE Name = N'Monitor' AND IsActive = 1;
    IF @MonitorRoleId IS NULL
    BEGIN
        PRINT 'Rol Monitor no existe o ya está inactivo. Nada que migrar.';
        COMMIT TRANSACTION;
        RETURN;
    END

    -- Rol destino NUTRE: Coordinadora de Monitoría o Coordinador (el que exista)
    SELECT TOP 1 @TargetRoleId = Id
    FROM AspNetRoles
    WHERE (Name = N'Coordinadora de Monitoría' OR Name = N'Coordinador') AND IsActive = 1;

    IF @TargetRoleId IS NOT NULL
    BEGIN
        -- Asignar rol destino a usuarios que tienen Monitor y aún no tienen el rol destino
        INSERT INTO AspNetUserRoles (UserId, RoleId, IsActive, CreatedAt)
        SELECT DISTINCT ur.UserId, @TargetRoleId, 1, GETUTCDATE()
        FROM AspNetUserRoles ur
        WHERE ur.RoleId = @MonitorRoleId AND ur.IsActive = 1
          AND NOT EXISTS (SELECT 1 FROM AspNetUserRoles a WHERE a.UserId = ur.UserId AND a.RoleId = @TargetRoleId AND a.IsActive = 1);

        PRINT 'Usuarios migrados de Monitor a rol NUTRE: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));
    END
    ELSE
        PRINT 'ADVERTENCIA: No existe rol Coordinadora de Monitoría ni Coordinador. Usuarios con Monitor no migrados.';

    -- Quitar asignación Monitor de todos los usuarios
    DELETE FROM AspNetUserRoles WHERE RoleId = @MonitorRoleId;

    -- Quitar Monitor de RoleAssignmentCategory si existe
    IF OBJECT_ID('RoleAssignmentCategory', 'U') IS NOT NULL
        DELETE FROM RoleAssignmentCategory WHERE RoleId = @MonitorRoleId;

    -- Eliminar el rol Monitor de AspNetRoles
    DELETE FROM AspNetRoles WHERE Id = @MonitorRoleId;
    PRINT 'Rol Monitor eliminado de AspNetRoles.';

    COMMIT TRANSACTION;
    PRINT 'Migración 118_RemoveMonitorRoleAndMigrateUsers completada.';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
    PRINT 'Error: ' + @Msg;
    THROW;
END CATCH;
GO
