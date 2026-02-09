-- =============================================
-- Stored Procedure: 113_InsertUserRoles
-- Descripción: Inserta los roles de un usuario en AspNetUserRoles.
--              Usa IsActive=1 y CreatedAt para compatibilidad con el esquema extendido.
--              Reemplaza AddToRolesAsync en RegisterUser para asegurar persistencia correcta.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[113_InsertUserRoles]
    @userId NVARCHAR(450),
    @roleNames NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF @userId IS NULL OR LTRIM(RTRIM(@userId)) = ''
        RETURN;

    IF @roleNames IS NULL OR LTRIM(RTRIM(@roleNames)) = ''
        RETURN;

    INSERT INTO AspNetUserRoles
        (UserId, RoleId, IsActive, CreatedAt)
    SELECT @userId, r.Id, 1, GETDATE()
    FROM AspNetRoles r
    INNER JOIN STRING_SPLIT(@roleNames, ',') ss ON LTRIM(RTRIM(ss.value)) = r.Name;
END;
GO
