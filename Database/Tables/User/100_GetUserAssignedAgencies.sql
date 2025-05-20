-- Stored Procedure para obtener las agencias asignadas a un usuario
CREATE OR ALTER PROCEDURE [dbo].[100_GetUserAssignedAgencies]
    @userId NVARCHAR(450),
    @take INT,
    @skip INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @totalRecords INT;

    WITH
        CTE_Results
        AS
        (
            SELECT
                a.*,
                uaa.AssignedDate,
                uaa.AssignedBy
            FROM Agency a
                INNER JOIN UserAgencyAssignment uaa ON a.Id = uaa.AgencyId
            WHERE uaa.UserId = @userId AND uaa.IsActive = 1
        )
    SELECT @totalRecords = COUNT(*)
    FROM CTE_Results;

    SELECT
        *,
        @totalRecords AS TotalRecords
    FROM CTE_Results
    ORDER BY AssignedDate DESC
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;
END; 