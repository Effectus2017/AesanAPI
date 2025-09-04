-- Obtener usuarios monitors de las agencias
-- 1.1.7 - Stored procedure para obtener monitors de agencias
CREATE OR ALTER PROCEDURE [119_GetAgencyMonitors]
    @agencyId INT = NULL,
    @userId NVARCHAR(450) = NULL,
    @take INT = 10,
    @skip INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    WITH
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT
                AgencyId,
                UserId,
                ROW_NUMBER() OVER (PARTITION BY AgencyId ORDER BY CreatedAt DESC) as rn
            FROM AgencyUsers
            WHERE IsMonitor = 1 AND IsActive = 1
        )
    SELECT
        mon.AgencyId,
        a.Name AS AgencyName,
        mon.UserId,
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
        mon.CreatedAt AS AssignmentDate,
        mon.UpdatedAt AS AssignmentUpdatedAt
    FROM AgencyMonitorsCTE mon
        INNER JOIN Agency a ON mon.AgencyId = a.Id
        LEFT JOIN AspNetUsers u ON mon.UserId = u.Id
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os ON s.PositionId = os.Id
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
    WHERE mon.rn = 1
        AND (@agencyId IS NULL OR mon.AgencyId = @agencyId)
        AND (@userId IS NULL OR mon.UserId = @userId)
    ORDER BY a.Name, s.FirstName, s.FatherLastName
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Count query
    WITH
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT
                AgencyId,
                UserId,
                ROW_NUMBER() OVER (PARTITION BY AgencyId ORDER BY CreatedAt DESC) as rn
            FROM AgencyUsers
            WHERE IsMonitor = 1 AND IsActive = 1
        )
    SELECT COUNT(*)
    FROM AgencyMonitorsCTE mon
    WHERE mon.rn = 1
        AND (@agencyId IS NULL OR mon.AgencyId = @agencyId)
        AND (@userId IS NULL OR mon.UserId = @userId);
END;
GO

-- Ejemplos de uso:
-- EXEC [119_GetAgencyMonitors] @agencyId = 1;
-- EXEC [119_GetAgencyMonitors] @userId = 'user-guid-here';
-- EXEC [119_GetAgencyMonitors] @take = 20, @skip = 0;
