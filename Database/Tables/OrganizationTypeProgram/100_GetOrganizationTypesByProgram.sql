CREATE OR ALTER PROCEDURE [100_GetOrganizationTypesByProgram]
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ot.Id,
        ot.Name,
        ot.NameEN,
        ot.IsActive,
        ot.DisplayOrder,
        ot.RequiresCenterType
    FROM OrganizationType ot
        INNER JOIN OrganizationTypeProgram otp ON ot.Id = otp.OrganizationTypeId
    WHERE otp.ProgramId = @programId
        AND ot.IsActive = 1
        AND otp.IsActive = 1
    ORDER BY ot.DisplayOrder, ot.Name;
END;
GO

