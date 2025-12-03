CREATE OR ALTER PROCEDURE [dbo].[100_AddEvaluatorToAllActivePrograms]
    @userId NVARCHAR(450),
    @assignedBy NVARCHAR(450),
    @evaluatorRoleId NVARCHAR(450)
-- Recibir el RoleId desde C# (sin usar nombre del rol)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @userRoleId NVARCHAR(450);

    -- Verificar que el usuario tenga el rol Evaluador (solo por RoleId, sin usar nombre)
    SELECT TOP 1
        @userRoleId = ur.RoleId
    FROM AspNetUserRoles ur
    WHERE ur.UserId = @userId
        AND ur.RoleId = @evaluatorRoleId
        AND ur.IsActive = 1;

    -- Si el usuario no tiene el rol especificado, retornar 0
    IF @userRoleId IS NULL
    BEGIN
        SELECT 0 AS ProgramsAssigned;
        RETURN;
    END

    -- Insertar el usuario en UserProgram para todos los programas activos
    -- Solo si no existe ya la asignación
    INSERT INTO UserProgram
        (UserId, ProgramId, IsActive, CreatedAt)
    SELECT
        @userId,
        p.Id,
        1,
        GETUTCDATE()
    FROM Program p
    WHERE p.IsActive = 1
        AND NOT EXISTS (
            SELECT 1
        FROM UserProgram up
        WHERE up.UserId = @userId
            AND up.ProgramId = p.Id
            AND up.IsActive = 1
        );

    -- Retornar el número de programas a los que se agregó (solo los que se insertaron en esta ejecución)
    SELECT @@ROWCOUNT AS ProgramsAssigned;
END
GO

