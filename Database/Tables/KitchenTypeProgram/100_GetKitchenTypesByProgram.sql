CREATE OR ALTER PROCEDURE [100_GetKitchenTypesByProgram]
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        id = kt.Id,
        name = kt.Name,
        nameen = kt.NameEN,
        isactive = kt.IsActive,
        displayorder = kt.DisplayOrder,
        createdat = kt.CreatedAt,
        updatedat = kt.UpdatedAt
    FROM KitchenType kt
        INNER JOIN KitchenTypeProgram ktp ON kt.Id = ktp.KitchenTypeId
    WHERE ktp.ProgramId = @programId
        AND kt.IsActive = 1
        AND ktp.IsActive = 1
    ORDER BY kt.DisplayOrder, kt.Name;
END;
GO
