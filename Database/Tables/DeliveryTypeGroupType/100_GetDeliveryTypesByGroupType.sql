CREATE OR ALTER PROCEDURE [100_GetDeliveryTypesByGroupType]
    @groupTypeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT dt.Id,
        dt.Name,
        dt.NameEN,
        dt.IsActive,
        dt.DisplayOrder,
        dtg.RequiresPermission,
        dt.CreatedAt,
        dt.UpdatedAt
    FROM DeliveryType dt
        INNER JOIN DeliveryTypeGroupType dtg ON dt.Id = dtg.DeliveryTypeId
    WHERE dtg.GroupTypeId = @groupTypeId
        AND dt.IsActive = 1
        AND dtg.IsActive = 1
    ORDER BY dt.DisplayOrder, dt.Name;
END;
GO

