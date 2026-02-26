-- 1.0.0
-- Obtiene roles AESAN disponibles para asignar como roles secundarios.
-- Excluye el rol primario y roles secundarios ya asignados.
-- 
-- Convención de campos de rol:
-- - name: Clave técnica del rol (ej: "administrator"). Se usa como identificador en lógica de negocio.
-- - displayname: Nombre legible en español para mostrar al usuario (ej: "Administrador").
-- - displaynameen: Nombre legible en inglés para mostrar al usuario (ej: "Administrator").
CREATE OR ALTER PROCEDURE [100_GetAvailableSecondaryRoles]
    @primaryroleid NVARCHAR(450) = NULL,
    @excluderoleids NVARCHAR(MAX) = NULL  -- Lista de IDs separados por coma
AS
BEGIN
    SET NOCOUNT ON;

    -- Tabla temporal para los IDs a excluir
    DECLARE @ExcludeTable TABLE (RoleId NVARCHAR(450));

    -- Parsear la lista de IDs excluidos
    IF @excluderoleids IS NOT NULL AND LEN(@excluderoleids) > 0
    BEGIN
        INSERT INTO @ExcludeTable (RoleId)
        SELECT TRIM(value)
        FROM STRING_SPLIT(@excluderoleids, ',')
        WHERE TRIM(value) <> '';
    END

    -- Agregar el rol primario a la lista de exclusión
    IF @primaryroleid IS NOT NULL AND LEN(@primaryroleid) > 0
    BEGIN
        INSERT INTO @ExcludeTable (RoleId)
        VALUES (@primaryroleid);
    END

    -- Obtener roles AESAN que no estén en la lista de exclusión
    SELECT 
        id = r.Id,
        name = r.Name,
        displayname = r.DisplayName,
        displaynameen = r.DisplayNameEN
    FROM AspNetRoles r
    WHERE r.Name IN (
        'super_administrator',
        'administrator',
        'coordinator',
        'specialist'
    )
    AND r.Id NOT IN (SELECT RoleId FROM @ExcludeTable)
    ORDER BY r.DisplayName;

END
GO
