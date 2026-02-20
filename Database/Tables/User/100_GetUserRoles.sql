-- 1.0.0
-- Obtiene todos los roles asignados a un usuario específico (primario + secundarios activos).
-- 
-- Convención de campos de rol:
-- - name: Clave técnica del rol (ej: "administrator"). Se usa como identificador en lógica de negocio.
-- - displayname: Nombre legible en español para mostrar al usuario (ej: "Administrador").
-- - displaynameen: Nombre legible en inglés para mostrar al usuario (ej: "Administrator").
CREATE OR ALTER PROCEDURE [100_GetUserRoles]
    @userid NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener rol primario y roles secundarios activos del usuario
    SELECT DISTINCT
        id = r.Id,
        name = r.Name,
        displayname = r.DisplayName,
        displaynameen = r.DisplayNameEN
    FROM AspNetRoles r
    INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
    WHERE ur.UserId = @userid
    ORDER BY r.DisplayName;

END
GO
