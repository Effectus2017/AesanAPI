CREATE OR ALTER PROCEDURE [100_GetDeliveryTypesByProgram]
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT dt.Id,
        dt.Name,
        dt.NameEN,
        dt.IsActive,
        dt.DisplayOrder,
        dt.SelectionNotification,
        dt.CreatedAt,
        dt.UpdatedAt
    FROM DeliveryType dt
        INNER JOIN DeliveryTypeProgram dtp ON dt.Id = dtp.DeliveryTypeId
    WHERE dtp.ProgramId = @programId
        AND dt.IsActive = 1
        AND dtp.IsActive = 1
    ORDER BY dt.DisplayOrder, dt.Name;
END;
GO

