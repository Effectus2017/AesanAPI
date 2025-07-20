-- =============================================
-- Stored Procedure: 100_GetEmployeeById
-- =============================================
-- Obtiene un empleado específico por su ID

CREATE OR ALTER PROCEDURE [dbo].[100_GetEmployeeById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.Id,
        e.FirstName,
        e.MiddleName,
        e.FatherLastName,
        e.MotherLastName,
        e.StatusId,
        os_status.Name AS StatusName,
        e.TitleId,
        os_title.Name AS TitleName,
        e.BirthDate,
        e.Email,
        e.PostalAddress,
        e.CityId,
        c.Name AS CityName,
        e.RegionId,
        r.Name AS RegionName,
        e.AreaCode,
        e.Comments,
        e.UserId,
        u.FirstName + ' ' + u.FatherLastName AS UserName,
        e.CreatedAt,
        e.UpdatedAt,
        e.IsActive
    FROM Employee e
        LEFT JOIN OptionSelection os_status ON e.StatusId = os_status.Id
        LEFT JOIN OptionSelection os_title ON e.TitleId = os_title.Id
        LEFT JOIN City c ON e.CityId = c.Id
        LEFT JOIN Region r ON e.RegionId = r.Id
        LEFT JOIN AspNetUsers u ON e.UserId = u.Id
    WHERE e.Id = @id;
END 