CREATE OR ALTER PROCEDURE [dbo].[100_GetEvaluatorsByProgramId]
    @programId INT,
    @evaluatorRoleId NVARCHAR(450)
-- Recibir el RoleId desde C# (sin usar nombre del rol)
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener todos los evaluadores asignados a un programa
    -- Filtrar solo usuarios con el RoleId especificado (sin usar nombre del rol)
    SELECT DISTINCT
        u.Id AS UserId,
        u.Email,
        u.UserName,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        up.ProgramId,
        up.IsActive,
        up.CreatedAt,
        up.UpdatedAt
    FROM UserProgram up
        INNER JOIN AspNetUsers u ON up.UserId = u.Id
        INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN Staff s ON u.Id = s.UserId
    WHERE up.ProgramId = @programId
        AND up.IsActive = 1
        AND ur.RoleId = @evaluatorRoleId
        AND ur.IsActive = 1
    ORDER BY up.CreatedAt DESC;
END
GO

