-- =============================================
-- Stored Procedure: 101_GetUserAssignedAgencies
-- Fecha: 2025-01-XX
-- Descripción: Obtiene las agencias asignadas a un usuario con nueva lógica.
--              Retorna AgencyAssignmentType, RoleId y RoleName mediante JOIN.
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[101_GetUserAssignedAgencies]
    @userId NVARCHAR(450),
    @take INT,
    @skip INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener resultados paginados con nueva lógica
    SELECT 
        a.Id,
        a.Name,
        a.AgencyCode,
        a.Address,
        a.Phone,
        a.Email,
        a.ImageUrl,
        a.AgencyStatusId,
        a.CreatedAt,
        a.UpdatedAt,
        uaa.AssignedDate,
        uaa.AssignedBy,
        uaa.AgencyAssignmentType,
        r.Id AS RoleId,  -- Desde JOIN
        r.Name AS RoleName  -- Desde JOIN
    FROM Agency a
    INNER JOIN AgencyUsers uaa ON a.Id = uaa.AgencyId
    INNER JOIN AspNetUserRoles ur ON uaa.UserId = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE uaa.UserId = @userId AND uaa.IsActive = 1
    ORDER BY uaa.AssignedDate DESC
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Obtener total de registros
    SELECT COUNT(DISTINCT a.Id)
    FROM Agency a
    INNER JOIN AgencyUsers uaa ON a.Id = uaa.AgencyId
    WHERE uaa.UserId = @userId AND uaa.IsActive = 1;

END;
GO 