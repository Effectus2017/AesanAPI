-- =============================================
-- Stored Procedure: 100_GetStaffByAgency
-- =============================================
-- Obtiene todos los miembros del staff de una agencia específica
-- Parámetros:
--   @agencyId: ID de la agencia (obligatorio)
--   @take: Número de registros a tomar
--   @skip: Número de registros a saltar
--   @name: Nombre para filtrar (busca en FirstName y FatherLastName)
--   @staffTypeId: ID del tipo de staff para filtrar

CREATE OR ALTER PROCEDURE [dbo].[100_GetStaffByAgency]
    @agencyId INT,
    @take INT = 15,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @staffTypeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar que se proporcione agencyId
    IF @agencyId IS NULL
    BEGIN
        RAISERROR ('El parámetro @agencyId es obligatorio', 16, 1);
        RETURN;
    END

    -- Retorna lista paginada con filtros
    SELECT
        s.Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        s.StatusId,
        os_status.Name AS StatusName,
        os_status.NameEN AS StatusNameEN,
        s.PositionId,
        ISNULL(
            (SELECT STRING_AGG(os_pos.Name, ', ') WITHIN GROUP (ORDER BY c.StaffClassificationId)
             FROM StaffContractByClassification c
             INNER JOIN OptionSelection os_pos ON c.PositionId = os_pos.Id
             WHERE c.StaffId = s.Id AND c.IsActive = 1),
            os_position.Name
        ) AS PositionName,
        os_position.NameEN AS PositionNameEN,
        s.StaffTypeId,
        st.Name AS StaffTypeName,
        st.NameEn AS StaffTypeNameEn,
        s.StaffClassificationId,
        sc.Name AS StaffClassificationName,
        sc.NameEn AS StaffClassificationNameEn,
        s.ContractStartDate,
        s.ContractEndDate,
        s.BirthDate,
        s.Email,
        s.PostalAddress,
        s.CityId,
        c.Name AS CityName,
        s.RegionId,
        r.Name AS RegionName,
        s.ZipCode,
        s.AgencyId,
        a.Name AS AgencyName,
        s.Comments,
        s.UserId,
        u.UserName,
        s.CreatedAt,
        s.UpdatedAt,
        s.IsActive
    FROM Staff s
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
        LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
        LEFT JOIN StaffType st ON s.StaffTypeId = st.Id
        LEFT JOIN StaffClassification sc ON s.StaffClassificationId = sc.Id
        LEFT JOIN City c ON s.CityId = c.Id
        LEFT JOIN Region r ON s.RegionId = r.Id
        LEFT JOIN Agency a ON s.AgencyId = a.Id
        LEFT JOIN AspNetUsers u ON s.UserId = u.Id
    WHERE s.IsActive = 1
        AND s.AgencyId = @agencyId
        AND (@name IS NULL OR LEN(@name) = 0 OR
        s.FirstName LIKE '%' + @name + '%' OR
        s.FatherLastName LIKE '%' + @name + '%' OR
        s.MiddleName LIKE '%' + @name + '%' OR
        s.MotherLastName LIKE '%' + @name + '%')
        AND (@staffTypeId IS NULL OR s.StaffTypeId = @staffTypeId)
    ORDER BY s.FirstName, s.FatherLastName
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Total de registros para paginación
    SELECT COUNT(*)
    FROM Staff s
    WHERE s.IsActive = 1
        AND s.AgencyId = @agencyId
        AND (@name IS NULL OR LEN(@name) = 0 OR
        s.FirstName LIKE '%' + @name + '%' OR
        s.FatherLastName LIKE '%' + @name + '%' OR
        s.MiddleName LIKE '%' + @name + '%' OR
        s.MotherLastName LIKE '%' + @name + '%')
        AND (@staffTypeId IS NULL OR s.StaffTypeId = @staffTypeId);
END
