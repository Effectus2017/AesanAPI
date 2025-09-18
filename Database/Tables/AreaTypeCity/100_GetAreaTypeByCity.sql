CREATE OR ALTER PROCEDURE [100_GetAreaTypeByCity]
    @cityId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT at.Id,
        at.Name,
        at.NameEN,
        at.IsActive,
        at.DisplayOrder,
        at.CreatedAt,
        at.UpdatedAt
    FROM AreaType at
        INNER JOIN AreaTypeCity atc ON at.Id = atc.AreaTypeId
    WHERE atc.CityId = @cityId
        AND at.IsActive = 1
        AND atc.IsActive = 1
    ORDER BY at.DisplayOrder, at.Name;
END;
GO
