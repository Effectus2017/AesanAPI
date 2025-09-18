CREATE OR ALTER PROCEDURE [100_GetCenterTypesByProgram]
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ct.Id,
        ct.Name,
        ct.NameEN,
        ct.IsActive,
        ct.DisplayOrder,
        ct.CreatedAt,
        ct.UpdatedAt
    FROM CenterType ct
        INNER JOIN CenterTypeProgram ctp ON ct.Id = ctp.CenterTypeId
    WHERE ctp.ProgramId = @programId
        AND ct.IsActive = 1
        AND ctp.IsActive = 1
    ORDER BY ct.DisplayOrder, ct.Name;
END;
GO
