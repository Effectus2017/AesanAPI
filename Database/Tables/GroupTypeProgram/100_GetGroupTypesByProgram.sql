CREATE OR ALTER PROCEDURE [100_GetGroupTypesByProgram]
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id = gt.Id,
        name = gt.Name,
        nameen = gt.NameEN,
        isactive = gt.IsActive,
        displayorder = gt.DisplayOrder,
        createdat = gt.CreatedAt,
        updatedat = gt.UpdatedAt
    FROM GroupType gt
        INNER JOIN GroupTypeProgram gtp ON gt.Id = gtp.GroupTypeId
    WHERE gtp.ProgramId = @programId
        AND gt.IsActive = 1
        AND gtp.IsActive = 1
    ORDER BY gt.DisplayOrder, gt.Name;
END;
GO

