CREATE OR ALTER PROCEDURE [100_GetKitchenTypesByGroupType]
    @groupTypeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT kt.Id,
        kt.Name,
        kt.NameEN,
        kt.IsActive,
        kt.DisplayOrder,
        kt.CreatedAt,
        kt.UpdatedAt
    FROM KitchenType kt
        INNER JOIN KitchenTypeGroupType ktgt ON kt.Id = ktgt.KitchenTypeId
    WHERE ktgt.GroupTypeId = @groupTypeId
        AND kt.IsActive = 1
        AND ktgt.IsActive = 1
    ORDER BY kt.DisplayOrder, kt.Name;
END;
GO
