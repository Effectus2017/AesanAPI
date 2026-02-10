CREATE OR ALTER PROCEDURE [100_GetKitchenTypesByGroupType]
    @groupTypeId INT,
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id = kt.Id,
        name = kt.Name,
        nameen = kt.NameEN,
        isactive = kt.IsActive,
        displayorder = kt.DisplayOrder,
        createdat = kt.CreatedAt,
        updatedat = kt.UpdatedAt
    FROM KitchenType kt
        INNER JOIN KitchenTypeGroupType ktgt ON kt.Id = ktgt.KitchenTypeId
        INNER JOIN KitchenTypeProgram ktp ON kt.Id = ktp.KitchenTypeId AND ktp.ProgramId = @programId AND ktp.IsActive = 1
    WHERE ktgt.GroupTypeId = @groupTypeId
        AND kt.IsActive = 1
        AND ktgt.IsActive = 1
    ORDER BY kt.DisplayOrder, kt.Name;
END;
GO
