-- =============================================
-- DEPRECATED: Este stored procedure está deprecado. Use Staff en su lugar.
-- Será eliminado en una versión futura.
-- =============================================
-- Stored Procedure: 100_GetAllEmployees
-- =============================================
-- Obtiene todos los empleados de la base de datos
-- Parámetros:
--   @take: Número de registros a obtener
--   @skip: Número de registros a saltar
--   @name: Nombre del empleado a buscar (opcional)
--   @alls: Si se deben obtener todos los empleados
--   @isList: Si es para lista
-- =============================================

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
            e.PositionId,
            os_position.Name AS PositionName,
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
            LEFT JOIN OptionSelection os_position ON e.PositionId = os_position.Id
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
            e.PositionId,
            os_position.Name AS PositionName,
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
            LEFT JOIN OptionSelection os_position ON e.PositionId = os_position.Id
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