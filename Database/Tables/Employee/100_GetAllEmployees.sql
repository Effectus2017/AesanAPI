-- =============================================
-- Stored Procedure: 100_GetEmployees
-- =============================================
-- Obtiene todos los empleados con paginación y filtros
-- Parámetros:
--   @take: Número de registros a tomar
--   @skip: Número de registros a saltar
--   @name: Nombre para filtrar (busca en FirstName y FatherLastName)
--   @alls: Si es true, retorna solo lista simple sin paginación

CREATE OR ALTER PROCEDURE [dbo].[100_GetEmployees]
    @take INT = 15,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF @alls = 1
    BEGIN
        -- Retorna lista simple para dropdowns
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
        WHERE e.IsActive = 1
        ORDER BY e.FirstName, e.FatherLastName;
    END
    ELSE
    BEGIN
        -- Retorna lista paginada con filtros
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
        WHERE e.IsActive = 1
            AND (
                @alls = 1
            OR (@name IS NULL OR
            e.FirstName LIKE '%' + @name + '%' OR
            e.FatherLastName LIKE '%' + @name + '%' OR
            e.MiddleName LIKE '%' + @name + '%' OR
            e.MotherLastName LIKE '%' + @name + '%')
            )
        ORDER BY e.FirstName, e.FatherLastName
        OFFSET @skip ROWS
        FETCH NEXT @take ROWS ONLY;

        -- Total de registros para paginación
        SELECT COUNT(*)
        FROM Employee e
        WHERE e.IsActive = 1
            AND (
                @alls = 1
            OR (@name IS NULL OR
            e.FirstName LIKE '%' + @name + '%' OR
            e.FatherLastName LIKE '%' + @name + '%' OR
            e.MiddleName LIKE '%' + @name + '%' OR
            e.MotherLastName LIKE '%' + @name + '%')
            );
    END
END 