-- =============================================
-- Stored Procedure: 100_GetStaff
-- =============================================
-- Obtiene todos los miembros del staff con paginación y filtros
-- Parámetros:
--   @take: Número de registros a tomar
--   @skip: Número de registros a saltar
--   @name: Nombre para filtrar (busca en FirstName y FatherLastName)
--   @alls: Si es true, retorna solo lista simple sin paginación

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaff]
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
            s.Id,
            s.FirstName,
            s.MiddleName,
            s.FatherLastName,
            s.MotherLastName,
            s.StatusId,
            os_status.Name AS StatusName,
            s.PositionId,
            os_position.Name AS PositionName,
            s.StaffTypeId,
            st.Name AS StaffTypeName,
            st.NameEn AS StaffTypeNameEn,
            s.BirthDate,
            s.Email,
            s.PostalAddress,
            s.CityId,
            c.Name AS CityName,
            s.RegionId,
            r.Name AS RegionName,
            s.AreaCode,
            s.Comments,
            s.UserId,
            u.FirstName + ' ' + u.FatherLastName AS UserName,
            s.CreatedAt,
            s.UpdatedAt,
            s.IsActive
        FROM Staff s
            LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
            LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
            LEFT JOIN StaffType st ON s.StaffTypeId = st.Id
            LEFT JOIN City c ON s.CityId = c.Id
            LEFT JOIN Region r ON s.RegionId = r.Id
            LEFT JOIN AspNetUsers u ON s.UserId = u.Id
        WHERE s.IsActive = 1
        ORDER BY s.FirstName, s.FatherLastName;
    END
    ELSE
    BEGIN
        -- Retorna lista paginada con filtros
        SELECT
            s.Id,
            s.FirstName,
            s.MiddleName,
            s.FatherLastName,
            s.MotherLastName,
            s.StatusId,
            os_status.Name AS StatusName,
            s.PositionId,
            os_position.Name AS PositionName,
            s.StaffTypeId,
            st.Name AS StaffTypeName,
            st.NameEn AS StaffTypeNameEn,
            s.BirthDate,
            s.Email,
            s.PostalAddress,
            s.CityId,
            c.Name AS CityName,
            s.RegionId,
            r.Name AS RegionName,
            s.AreaCode,
            s.Comments,
            s.UserId,
            u.FirstName + ' ' + u.FatherLastName AS UserName,
            s.CreatedAt,
            s.UpdatedAt,
            s.IsActive
        FROM Staff s
            LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
            LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
            LEFT JOIN StaffType st ON s.StaffTypeId = st.Id
            LEFT JOIN City c ON s.CityId = c.Id
            LEFT JOIN Region r ON s.RegionId = r.Id
            LEFT JOIN AspNetUsers u ON s.UserId = u.Id
        WHERE s.IsActive = 1
            AND (
                @alls = 1
            OR (@name IS NULL OR
            s.FirstName LIKE '%' + @name + '%' OR
            s.FatherLastName LIKE '%' + @name + '%' OR
            s.MiddleName LIKE '%' + @name + '%' OR
            s.MotherLastName LIKE '%' + @name + '%')
            )
        ORDER BY s.FirstName, s.FatherLastName
        OFFSET @skip ROWS
        FETCH NEXT @take ROWS ONLY;

        -- Total de registros para paginación
        SELECT COUNT(*)
        FROM Staff s
        WHERE s.IsActive = 1
            AND (
                @alls = 1
            OR (@name IS NULL OR
            s.FirstName LIKE '%' + @name + '%' OR
            s.FatherLastName LIKE '%' + @name + '%' OR
            s.MiddleName LIKE '%' + @name + '%' OR
            s.MotherLastName LIKE '%' + @name + '%')
            );
    END
END