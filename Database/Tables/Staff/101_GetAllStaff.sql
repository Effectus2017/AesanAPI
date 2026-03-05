-- =============================================
-- Stored Procedure: 101_GetAllStaff
-- =============================================
-- Obtiene todos los miembros del staff con paginación y filtros
-- Parámetros:
--   @take: Número de registros a tomar
--   @skip: Número de registros a saltar
--   @name: Nombre para filtrar (busca en FirstName y FatherLastName)
--   @alls: Si es true, retorna solo lista simple sin paginación
--   @staffTypeId: ID del tipo de staff para filtrar
--   @agencyId: ID de la agencia para filtrar
--   @excludeRelated: Si es true, excluye staff ya relacionados en StaffRelationship (usado en modal Add)
--
-- v101: Fix - Los filtros @agencyId y @staffTypeId ahora se aplican siempre,
--       independientemente del valor de @alls

CREATE OR ALTER PROCEDURE [dbo].[101_GetAllStaff]
    @take INT = 15,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @alls BIT = 0,
    @staffTypeId INT = NULL,
    @agencyId INT = NULL,
    @excludeRelated BIT = 0
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
            s.TenureDuration,
            s.TenureDurationUnitId,
            os_tenure_unit.Name AS TenureDurationUnitName,
            os_tenure_unit.NameEN AS TenureDurationUnitNameEN,
            s.ReceivesProgramSalaryId,
            os_salary.Name AS ReceivesProgramSalaryName,
            os_salary.NameEN AS ReceivesProgramSalaryNameEN,
            os_admin_position.Name AS AdministrativePositionName,
            os_admin_position.NameEN AS AdministrativePositionNameEN,
            os_oper_position.Name AS OperationalPositionName,
            os_oper_position.NameEN AS OperationalPositionNameEN,
            s.CreatedAt,
            s.UpdatedAt,
            s.IsActive,
            CAST(
                CASE 
                    WHEN EXISTS (
                        SELECT 1
                        FROM StaffRelationship sr
                        WHERE (sr.StaffId = s.Id OR sr.RelatedStaffId = s.Id)
                            AND sr.IsActive = 1
                    ) THEN 1 
                    ELSE 0 
                END AS BIT
            ) AS HasRelationships,
            CAST(
                CASE 
                    WHEN (SELECT TOP 1 r.Name
                          FROM AspNetUserRoles ur
                          INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                          WHERE ur.UserId = s.UserId
                              AND ur.IsActive = 1
                              AND r.Name = 'agency_administrator') = 'agency_administrator' 
                    THEN 1 
                    ELSE 0 
                END AS BIT
            ) AS isSiteAdmin
        FROM Staff s
            LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
            LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
            LEFT JOIN StaffType st ON s.StaffTypeId = st.Id
            LEFT JOIN StaffClassification sc ON s.StaffClassificationId = sc.Id
            LEFT JOIN City c ON s.CityId = c.Id
            LEFT JOIN Region r ON s.RegionId = r.Id
            LEFT JOIN Agency a ON s.AgencyId = a.Id
            LEFT JOIN AspNetUsers u ON s.UserId = u.Id
            LEFT JOIN OptionSelection os_tenure_unit ON s.TenureDurationUnitId = os_tenure_unit.Id
            LEFT JOIN OptionSelection os_salary ON s.ReceivesProgramSalaryId = os_salary.Id
            LEFT JOIN StaffContractByClassification scbc_admin ON s.Id = scbc_admin.StaffId AND scbc_admin.StaffClassificationId = 1 AND scbc_admin.IsActive = 1
            LEFT JOIN OptionSelection os_admin_position ON scbc_admin.PositionId = os_admin_position.Id
            LEFT JOIN StaffContractByClassification scbc_oper ON s.Id = scbc_oper.StaffId AND scbc_oper.StaffClassificationId = 2 AND scbc_oper.IsActive = 1
            LEFT JOIN OptionSelection os_oper_position ON scbc_oper.PositionId = os_oper_position.Id
        WHERE s.IsActive = 1
            AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
            AND (@staffTypeId IS NULL OR s.StaffTypeId = @staffTypeId)
            AND (@excludeRelated = 0 OR s.Id NOT IN (
                SELECT DISTINCT sr.StaffId
                FROM StaffRelationship sr
                WHERE sr.IsActive = 1
                UNION
                SELECT DISTINCT sr.RelatedStaffId
                FROM StaffRelationship sr
                WHERE sr.IsActive = 1))
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
            s.TenureDuration,
            s.TenureDurationUnitId,
            os_tenure_unit.Name AS TenureDurationUnitName,
            os_tenure_unit.NameEN AS TenureDurationUnitNameEN,
            s.ReceivesProgramSalaryId,
            os_salary.Name AS ReceivesProgramSalaryName,
            os_salary.NameEN AS ReceivesProgramSalaryNameEN,
            os_admin_position.Name AS AdministrativePositionName,
            os_admin_position.NameEN AS AdministrativePositionNameEN,
            os_oper_position.Name AS OperationalPositionName,
            os_oper_position.NameEN AS OperationalPositionNameEN,
            s.CreatedAt,
            s.UpdatedAt,
            s.IsActive,
            CAST(
                CASE 
                    WHEN EXISTS (
                        SELECT 1
                        FROM StaffRelationship sr
                        WHERE (sr.StaffId = s.Id OR sr.RelatedStaffId = s.Id)
                            AND sr.IsActive = 1
                    ) THEN 1 
                    ELSE 0 
                END AS BIT
            ) AS HasRelationships,
            CAST(
                CASE 
                    WHEN (SELECT TOP 1 r.Name
                          FROM AspNetUserRoles ur
                          INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                          WHERE ur.UserId = s.UserId
                              AND ur.IsActive = 1
                              AND r.Name = 'agency_administrator') = 'agency_administrator' 
                    THEN 1 
                    ELSE 0 
                END AS BIT
            ) AS IsSiteAdmin
        FROM Staff s
            LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
            LEFT JOIN OptionSelection os_position ON s.PositionId = os_position.Id
            LEFT JOIN StaffType st ON s.StaffTypeId = st.Id
            LEFT JOIN StaffClassification sc ON s.StaffClassificationId = sc.Id
            LEFT JOIN City c ON s.CityId = c.Id
            LEFT JOIN Region r ON s.RegionId = r.Id
            LEFT JOIN AspNetUsers u ON s.UserId = u.Id
            LEFT JOIN Agency a ON s.AgencyId = a.Id
            LEFT JOIN OptionSelection os_tenure_unit ON s.TenureDurationUnitId = os_tenure_unit.Id
            LEFT JOIN OptionSelection os_salary ON s.ReceivesProgramSalaryId = os_salary.Id
            LEFT JOIN StaffContractByClassification scbc_admin ON s.Id = scbc_admin.StaffId AND scbc_admin.StaffClassificationId = 1 AND scbc_admin.IsActive = 1
            LEFT JOIN OptionSelection os_admin_position ON scbc_admin.PositionId = os_admin_position.Id
            LEFT JOIN StaffContractByClassification scbc_oper ON s.Id = scbc_oper.StaffId AND scbc_oper.StaffClassificationId = 2 AND scbc_oper.IsActive = 1
            LEFT JOIN OptionSelection os_oper_position ON scbc_oper.PositionId = os_oper_position.Id
        WHERE s.IsActive = 1
            AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
            AND (@staffTypeId IS NULL OR s.StaffTypeId = @staffTypeId)
            AND (@name IS NULL OR
                s.FirstName LIKE '%' + @name + '%' OR
                s.FatherLastName LIKE '%' + @name + '%' OR
                s.MiddleName LIKE '%' + @name + '%' OR
                s.MotherLastName LIKE '%' + @name + '%')
            AND (@excludeRelated = 0 OR s.Id NOT IN (
                SELECT DISTINCT sr.StaffId
                FROM StaffRelationship sr
                WHERE sr.IsActive = 1
                UNION
                SELECT DISTINCT sr.RelatedStaffId
                FROM StaffRelationship sr
                WHERE sr.IsActive = 1))
        ORDER BY s.FirstName, s.FatherLastName
        OFFSET @skip ROWS
        FETCH NEXT @take ROWS ONLY;

        -- Total de registros para paginación
        SELECT COUNT(*)
        FROM Staff s
        WHERE s.IsActive = 1
            AND (@agencyId IS NULL OR s.AgencyId = @agencyId)
            AND (@staffTypeId IS NULL OR s.StaffTypeId = @staffTypeId)
            AND (@name IS NULL OR
                s.FirstName LIKE '%' + @name + '%' OR
                s.FatherLastName LIKE '%' + @name + '%' OR
                s.MiddleName LIKE '%' + @name + '%' OR
                s.MotherLastName LIKE '%' + @name + '%')
            AND (@excludeRelated = 0 OR s.Id NOT IN (
                SELECT DISTINCT sr.StaffId
                FROM StaffRelationship sr
                WHERE sr.IsActive = 1
                UNION
                SELECT DISTINCT sr.RelatedStaffId
                FROM StaffRelationship sr
                WHERE sr.IsActive = 1));
    END
END


--EXEC [100_GetAllStaff] @excludeRelated = 1, @alls = 1, @name = 'Juan', @staffTypeId = 2, @agencyId = 3, @take = 10, @skip = 0;
