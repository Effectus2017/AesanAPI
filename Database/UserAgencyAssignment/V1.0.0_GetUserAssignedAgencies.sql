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
        a.Code,
        a.Address,
        a.Phone,
        a.Email,
        a.Website,
        a.Logo,
        a.StatusId,
        a.RejectionJustification,
        a.CreatedAt,
        a.UpdatedAt,
        uaa.AssignedDate,
        uaa.AssignedBy
    FROM Agency a
    INNER JOIN UserAgencyAssignment uaa ON a.Id = uaa.AgencyId
    WHERE uaa.UserId = @userId AND uaa.IsActive = 1
    ORDER BY uaa.AssignedDate DESC
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Obtener total de registros
    SELECT COUNT(DISTINCT a.Id)
    FROM Agency a
    INNER JOIN UserAgencyAssignment uaa ON a.Id = uaa.AgencyId
    WHERE uaa.UserId = @userId AND uaa.IsActive = 1;

END; 