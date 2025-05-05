CREATE OR ALTER PROCEDURE [dbo].[101_GetHouseholdById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT h.Id, h.Street, h.Apartment, h.CityId, c.Name AS CityName, h.RegionId, r.Name AS RegionName, h.ZipCode, h.Phone, h.Email, h.CompletedBy, h.CompletedDate, h.IsActive, h.CreatedAt, h.UpdatedAt
    FROM Household h
    LEFT JOIN City c ON h.CityId = c.Id
    LEFT JOIN Region r ON h.RegionId = r.Id
    WHERE h.Id = @Id;
END; 