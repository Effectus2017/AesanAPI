-- Obtener usuarios owners de las agencias
-- 1.1.7 - Stored procedure para obtener owners de agencias
CREATE OR ALTER PROCEDURE [118_GetAgencyOwners]
    @agencyId INT = NULL,
    @userId NVARCHAR(450) = NULL,
    @take INT = 10,
    @skip INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    WITH
        AgencyOwnersCTE
        AS
        (
            SELECT DISTINCT
                AgencyId,
                UserId,
                IsOwner,
                ROW_NUMBER() OVER (PARTITION BY AgencyId ORDER BY CreatedAt DESC) as rn
            FROM AgencyUsers
            WHERE IsOwner = 1 AND IsActive = 1
        )
    SELECT
        own.AgencyId,
        a.Name AS AgencyName,
        own.UserId,
        own.IsOwner,
        -- Datos del usuario desde Staff
        s.Id as StaffId,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        -- Campos de posición del usuario
        s.PositionId,
        os.Name AS PositionName,
        os.NameEN AS PositionNameEN,
        os.OptionKey AS PositionOptionKey,
        -- Campos de contrato del usuario
        s.ContractStartDate,
        s.ContractEndDate,
        -- Información adicional del usuario
        s.Email,
        s.BirthDate,
        s.StatusId,
        os_status.Name AS StatusName,
        os_status.NameEN AS StatusNameEN,
        own.CreatedAt AS AssignmentDate,
        own.UpdatedAt AS AssignmentUpdatedAt
    FROM AgencyOwnersCTE own
        INNER JOIN Agency a ON own.AgencyId = a.Id
        LEFT JOIN AspNetUsers u ON own.UserId = u.Id
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os ON s.PositionId = os.Id
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
    WHERE own.rn = 1
        AND (@agencyId IS NULL OR own.AgencyId = @agencyId)
        AND (@userId IS NULL OR own.UserId = @userId)
    ORDER BY a.Name, s.FirstName, s.FatherLastName
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Count query
    WITH
        AgencyOwnersCTE
        AS
        (
            SELECT DISTINCT
                AgencyId,
                UserId,
                ROW_NUMBER() OVER (PARTITION BY AgencyId ORDER BY CreatedAt DESC) as rn
            FROM AgencyUsers
            WHERE IsOwner = 1 AND IsActive = 1
        )
    SELECT COUNT(*)
    FROM AgencyOwnersCTE own
    WHERE own.rn = 1
        AND (@agencyId IS NULL OR own.AgencyId = @agencyId)
        AND (@userId IS NULL OR own.UserId = @userId);
END;
GO

-- Ejemplos de uso:
-- EXEC [118_GetAgencyOwners] @agencyId = 1;
-- EXEC [118_GetAgencyOwners] @userId = 'user-guid-here';
-- EXEC [118_GetAgencyOwners] @take = 20, @skip = 0;
