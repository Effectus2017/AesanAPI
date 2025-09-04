-- Obtener todas las agencias
-- 1.1.7 - Versión simplificada sin datos de usuarios
CREATE OR ALTER PROCEDURE [117_GetAgencies]
    @take INT = 10,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @programId INT = NULL,
    @statusId INT = NULL,
    @userId NVARCHAR(450) = NULL,
    @alls BIT = 0,
    @isPropietary BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- CTEs para mejorar la performance
    WITH
        AgencyProgramsCTE
        AS
        (
            SELECT DISTINCT AgencyId, ProgramId
            FROM AgencyProgram
            WHERE IsActive = 1
        ),
        AgencyOwnersCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId, CreatedAt
            FROM AgencyUsers
            WHERE IsOwner = 1 AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE IsMonitor = 1 AND IsActive = 1
        )
    SELECT DISTINCT
        a.Id,
        a.Name,
        a.AgencyStatusId as StatusId,
        ast.Name AS AgencyStatusName,
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        a.Address,
        a.ZipCode,
        a.CityId,
        c.Name AS CityName,
        a.RegionId,
        r.Name AS RegionName,
        a.PostalAddress,
        a.PostalZipCode,
        a.PostalCityId,
        pc.Name AS PostalCityName,
        a.PostalRegionId,
        pr.Name AS PostalRegionName,
        a.Latitude,
        a.Longitude,
        a.Phone,
        a.Email,
        a.ImageURL,
        a.IsPropietary,
        -- Campos de inscripción actualizados
        ai.BasicEducationRegistryId,
        ai.NonProfit,
        ai.FederalFundsDenied,
        ai.StateFundsDenied,
        ai.OrganizedAthleticPrograms,
        ai.AtRiskService,
        ai.ServiceTime,
        ai.TaxExemptionStatusId,
        ai.TaxExemptionTypeId,
        ai.TypeOfEntityId,
        ai.TypeOfApplicantId,
        ai.PublicAllianceContractId,
        ai.NationalYouthProgram,
        ai.IsDayCareHome,
        a.IsActive,
        a.IsListable,
        a.CreatedAt,
        a.UpdatedAt,
        a.AgencyCode,

        -- Comentarios de la asignación de programa
        ai.Comments as ProgramRejectionJustification,
        ai.AppointmentCoordinated AS ProgramAppointmentCoordinated,
        ai.AppointmentDate AS ProgramAppointmentDate

    FROM Agency a
        INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
        INNER JOIN City c ON a.CityId = c.Id
        INNER JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN AgencyInscription ai ON a.Id = ai.AgencyId
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
        LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
        LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
    WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
        AND (
    @alls = 1
        OR ((@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@programId IS NULL OR ap.ProgramId = @programId)
        AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
        AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId)
    )
)
    ORDER BY a.CreatedAt DESC, a.Name
OFFSET @skip ROWS
FETCH NEXT @take ROWS ONLY;

    -- Obtener programas asociados a las agencias filtradas
    WITH
        AgencyProgramsCTE
        AS
        (
            SELECT DISTINCT AgencyId, ProgramId
            FROM AgencyProgram
            WHERE IsActive = 1
        ),
        AgencyOwnersCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE IsOwner = 1 AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE IsMonitor = 1 AND IsActive = 1
        ),
        FilteredAgencies
        AS
        (
            SELECT DISTINCT a.Id
            FROM Agency a
                LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
                LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
                LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
            WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
                AND (
        @alls = 1
                OR (
            (@name IS NULL OR a.Name LIKE '%' + @name + '%')
                AND (@regionId IS NULL OR a.RegionId = @regionId)
                AND (@cityId IS NULL OR a.CityId = @cityId)
                AND (@programId IS NULL OR ap.ProgramId = @programId)
                AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
                AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId)
        )
    )
            ORDER BY a.Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY
        )
    SELECT DISTINCT
        p.Id,
        p.Name,
        p.NameEN,
        p.Description,
        p.DescriptionEN,
        p.IsActive,
        p.CreatedAt,
        p.UpdatedAt
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
        INNER JOIN FilteredAgencies fa ON ap.AgencyId = fa.Id
    WHERE ap.IsActive = 1;

    -- Obtener usuarios monitors asociados a las agencias filtradas
    WITH
        AgencyProgramsCTE
        AS
        (
            SELECT DISTINCT AgencyId, ProgramId
            FROM AgencyProgram
            WHERE IsActive = 1
        ),
        AgencyOwnersCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE IsOwner = 1 AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId, CreatedAt
            FROM AgencyUsers
            WHERE IsMonitor = 1 AND IsActive = 1
        ),
        FilteredAgencies
        AS
        (
            SELECT DISTINCT a.Id
            FROM Agency a
                LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
                LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
                LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
            WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
                AND (
        @alls = 1
                OR (
            (@name IS NULL OR a.Name LIKE '%' + @name + '%')
                AND (@regionId IS NULL OR a.RegionId = @regionId)
                AND (@cityId IS NULL OR a.CityId = @cityId)
                AND (@programId IS NULL OR ap.ProgramId = @programId)
                AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
                AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId)
        )
    )
            ORDER BY a.Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY
        ),
        AgencyMonitorsWithDataCTE
        AS
        (
            SELECT DISTINCT
                mon.AgencyId,
                mon.UserId,
                ROW_NUMBER() OVER (PARTITION BY mon.AgencyId ORDER BY mon.CreatedAt DESC) as rn
            FROM AgencyMonitorsCTE mon
                INNER JOIN FilteredAgencies fa ON mon.AgencyId = fa.Id
        )
    SELECT DISTINCT
        mon.AgencyId,
        mon.UserId,
        -- Datos del usuario desde Staff
        s.Id as Id,
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
        os_status.NameEN AS StatusNameEN
    FROM AgencyMonitorsWithDataCTE mon
        LEFT JOIN AspNetUsers u ON mon.UserId = u.Id
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os ON s.PositionId = os.Id
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
    WHERE mon.rn = 1;

    -- Obtener usuarios owners asociados a las agencias filtradas
    WITH
        AgencyProgramsCTE
        AS
        (
            SELECT DISTINCT AgencyId, ProgramId
            FROM AgencyProgram
            WHERE IsActive = 1
        ),
        AgencyOwnersCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId, CreatedAt
            FROM AgencyUsers
            WHERE IsOwner = 1 AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE IsMonitor = 1 AND IsActive = 1
        ),
        FilteredAgencies
        AS
        (
            SELECT DISTINCT a.Id
            FROM Agency a
                LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
                LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
                LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
            WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
                AND (
        @alls = 1
                OR (
            (@name IS NULL OR a.Name LIKE '%' + @name + '%')
                AND (@regionId IS NULL OR a.RegionId = @regionId)
                AND (@cityId IS NULL OR a.CityId = @cityId)
                AND (@programId IS NULL OR ap.ProgramId = @programId)
                AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
                AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId)
        )
            )
            ORDER BY a.Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY
        ),
        AgencyOwnersWithDataCTE
        AS
        (
            SELECT DISTINCT
                own.AgencyId,
                own.UserId,
                ROW_NUMBER() OVER (PARTITION BY own.AgencyId ORDER BY own.CreatedAt DESC) as rn
            FROM AgencyOwnersCTE own
                INNER JOIN FilteredAgencies fa ON own.AgencyId = fa.Id
        )
    SELECT DISTINCT
        own.AgencyId,
        own.UserId,
        -- Datos del usuario desde Staff
        s.Id as Id,
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
        os_status.NameEN AS StatusNameEN
    FROM AgencyOwnersWithDataCTE own
        LEFT JOIN AspNetUsers u ON own.UserId = u.Id
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os ON s.PositionId = os.Id
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
    WHERE own.rn = 1;

    -- Count query
    WITH
        AgencyProgramsCTE
        AS
        (
            SELECT DISTINCT AgencyId, ProgramId
            FROM AgencyProgram
            WHERE IsActive = 1
        ),
        AgencyOwnersCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE IsOwner = 1 AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE IsMonitor = 1 AND IsActive = 1
        )
    SELECT COUNT(DISTINCT a.Id)
    FROM Agency a
        LEFT JOIN AgencyInscription ai ON a.Id = ai.AgencyId
        LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
        LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
        LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
    WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
        AND (
    @alls = 1
        OR (
        (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@programId IS NULL OR ap.ProgramId = @programId)
        AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
        AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId)
        AND (a.IsListable = 1)
    )
        );
END;
GO

EXEC [117_GetAgencies]
    @take = 10,
    @skip = 0,
    @name = NULL,
    @regionId = NULL,
    @cityId = NULL,
    @programId = NULL,
    @statusId = NULL,
    @userId = NULL,
    @alls = 1,
    @isPropietary = NULL;
