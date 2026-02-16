-- =============================================
-- Stored Procedure: 100_UpdateUserRolesPrimarySecondary
-- Descripción: Asigna rol principal (uno) y roles secundarios (N, con ValidFrom/ValidTo).
--              Mueve secundarios actuales a UserSecondaryRoleHistory antes de reemplazar.
-- Parámetros: lowercase; salida alias lowercase; columnas en cuerpo CapitalCase.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateUserRolesPrimarySecondary]
    @userId NVARCHAR(450),
    @primaryRoleName NVARCHAR(256),
    @secondaryRolesJson NVARCHAR(MAX) = NULL,
    @assignedBy NVARCHAR(450) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @userId IS NULL OR LTRIM(RTRIM(@userId)) = ''
        RETURN;
    IF @primaryRoleName IS NULL OR LTRIM(RTRIM(@primaryRoleName)) = ''
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Mover roles secundarios actuales a historial antes de borrar
        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserSecondaryRoleHistory')
        BEGIN
            INSERT INTO UserSecondaryRoleHistory (UserId, RoleId, ValidFrom, ValidTo, CreatedBy, EndedAt)
            SELECT ur.UserId, ur.RoleId, ur.ValidFrom, ur.ValidTo, @assignedBy, GETUTCDATE()
            FROM AspNetUserRoles ur
            WHERE ur.UserId = @userId
                AND ur.IsPrimary = 0
                AND ur.IsActive = 1
                AND ur.ValidFrom IS NOT NULL
                AND ur.ValidTo IS NOT NULL;
        END

        -- 2. Eliminar todos los roles del usuario
        DELETE FROM AspNetUserRoles WHERE UserId = @userId;

        -- 3. Insertar rol principal (IsPrimary=1, ValidFrom/ValidTo NULL)
        INSERT INTO AspNetUserRoles (UserId, RoleId, IsActive, CreatedAt, IsPrimary, ValidFrom, ValidTo)
        SELECT @userId, r.Id, 1, GETDATE(), 1, NULL, NULL
        FROM AspNetRoles r
        WHERE r.Name = LTRIM(RTRIM(@primaryRoleName));

        -- 4. Insertar roles secundarios desde JSON (IsPrimary=0, ValidFrom, ValidTo)
        IF @secondaryRolesJson IS NOT NULL AND LTRIM(RTRIM(@secondaryRolesJson)) <> ''
        BEGIN
            INSERT INTO AspNetUserRoles (UserId, RoleId, IsActive, CreatedAt, IsPrimary, ValidFrom, ValidTo)
            SELECT @userId, r.Id, 1, GETDATE(), 0, CAST(j.validFrom AS DATE), CAST(j.validTo AS DATE)
            FROM OPENJSON(@secondaryRolesJson) WITH (
                roleName NVARCHAR(256) N'$.roleName',
                validFrom DATE N'$.validFrom',
                validTo DATE N'$.validTo'
            ) j
            INNER JOIN AspNetRoles r ON r.Name = LTRIM(RTRIM(j.roleName))
            WHERE j.validFrom IS NOT NULL AND j.validTo IS NOT NULL;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
