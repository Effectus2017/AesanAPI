CREATE OR ALTER PROCEDURE [dbo].[100_GetUserAssignedAgencies]
    @userId NVARCHAR(450),
    @take INT,
    @skip INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener resultados paginados
    SELECT 
        a.Id,
        a.Name,
        a.Address,
        a.Phone,
        a.Email,
        a.RejectionJustification,
        a.CreatedAt,
        a.UpdatedAt,
        au.AssignedDate,
        au.AssignedBy
    FROM Agency a
    INNER JOIN AgencyUsers au ON a.Id = au.AgencyId
    WHERE au.UserId = @userId AND au.IsActive = 1
    ORDER BY au.AssignedDate DESC
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Obtener total de registros
    SELECT COUNT(DISTINCT a.Id)
    FROM Agency a
    INNER JOIN AgencyUsers au ON a.Id = au.AgencyId
    WHERE au.UserId = @userId AND au.IsActive = 1;

END; 