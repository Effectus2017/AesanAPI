CREATE OR ALTER PROCEDURE [dbo].[100_GetAgencyProgramsByUserId]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        p.*
    FROM AspNetUsers u
        JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        JOIN AspNetRoles r ON ur.RoleId = r.Id
        JOIN UserProgram up ON u.Id = up.UserId
        JOIN Program p ON up.ProgramId = p.Id
        JOIN AgencyProgram ap ON p.Id = ap.ProgramId
        JOIN Agency a ON ap.AgencyId = a.Id
        JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    WHERE 
	    r.Name = 'Monitor'
        AND u.Id = @userId -- Parámetro para el ID del usuario monitor
        AND a.IsActive = 1;

END
GO

EXEC [100_GetAgencyProgramsByUserId] @userId = '1db1104b-6c97-4f64-93e1-929296dea7bf';