CREATE OR ALTER PROCEDURE [113_GetAgencies]
    @take INT = 10,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @programId INT = NULL,
    @statusId INT = NULL,
    @userId NVARCHAR(450) = NULL,
    @alls BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- CTEs para mejorar la legibilidad y performance
    WITH AgencyProgramsCTE AS (
        SELECT DISTINCT AgencyId, ProgramId
        FROM AgencyProgram 
        WHERE IsActive = 1
    ),
    AgencyOwnersCTE AS (
        SELECT DISTINCT AgencyId, UserId, IsOwner
        FROM AgencyUsers
        WHERE IsOwner = 1 AND IsActive = 1
    ),
    AgencyMonitorsCTE AS (
        SELECT DISTINCT AgencyId, UserId
        FROM AgencyUsers
        WHERE IsMonitor = 1 AND IsActive = 1
    )
    SELECT DISTINCT
        a.Id,
        a.Name,
        a.AgencyStatusId,
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
        -- Campos de inscripción
        ai.BasicEducationRegistry,
        ai.NonProfit,
        ai.FederalFundsDenied,
        ai.StateFundsDenied,
        ai.OrganizedAthleticPrograms,
        ai.AtRiskService,
        ai.ServiceTime,
        ai.TaxExemptionStatus,
        ai.TaxExemptionType,
        a.IsActive,
        a.IsListable,
        a.CreatedAt,
        a.UpdatedAt,
        a.AgencyCode,
        own.IsOwner,

        -- Datos del usuario de la agencia
        u.Id as UserId,
        u.FirstName AS UserFirstName,
        u.FatherLastName AS UserFatherLastName,

        -- Datos del usuario monitor
        mon.UserId as MonitorId,
        mu.FirstName AS MonitorFirstName,
        mu.FatherLastName AS MonitorFatherLastName,

        -- Comentarios de la asignación de programa
        ai.Comments as ProgramRejectionJustification,
        ai.AppointmentCoordinated AS ProgramAppointmentCoordinated,
        ai.AppointmentDate AS ProgramAppointmentDate

    FROM Agency a
    INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    INNER JOIN City c ON a.CityId = c.Id
    INNER JOIN Region r ON a.RegionId = r.Id
    LEFT JOIN AgencyInscription ai ON a.AgencyInscriptionId = ai.Id
    LEFT JOIN City pc ON a.PostalCityId = pc.Id
    LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
    LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
    LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
    LEFT JOIN AspNetUsers u ON own.UserId = u.Id
    LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
    LEFT JOIN AspNetUsers mu ON mon.UserId = mu.Id
    WHERE a.IsPropietary = 0
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
    ORDER BY a.Name
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Obtener programas asociados a las agencias filtradas y al usuario
    WITH AgencyProgramsCTE AS (
        SELECT DISTINCT AgencyId, ProgramId
        FROM AgencyProgram 
        WHERE IsActive = 1
    ),
    AgencyOwnersCTE AS (
        SELECT DISTINCT AgencyId, UserId
        FROM AgencyUsers
        WHERE IsOwner = 1 AND IsActive = 1
    ),
    AgencyMonitorsCTE AS (
        SELECT DISTINCT AgencyId, UserId
        FROM AgencyUsers
        WHERE IsMonitor = 1 AND IsActive = 1
    ),
    FilteredAgencies AS (
        SELECT DISTINCT a.Id
        FROM Agency a
        LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
        LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
        LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
        WHERE a.IsPropietary = 0
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
        p.Description,
        ap.AgencyId,
        ap.Comments,
        ap.AppointmentCoordinated,
        ap.AppointmentDate,
        -- Usuario asignado al programa
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName
    FROM Program p
    INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
    INNER JOIN FilteredAgencies fa ON ap.AgencyId = fa.Id
    LEFT JOIN AspNetUsers u ON ap.UserId = u.Id
    WHERE ap.IsActive = 1;

    -- Count query
    WITH AgencyProgramsCTE AS (
        SELECT DISTINCT AgencyId, ProgramId
        FROM AgencyProgram 
        WHERE IsActive = 1
    ),
    AgencyOwnersCTE AS (
        SELECT DISTINCT AgencyId, UserId
        FROM AgencyUsers
        WHERE IsOwner = 1 AND IsActive = 1
    ),
    AgencyMonitorsCTE AS (
        SELECT DISTINCT AgencyId, UserId
        FROM AgencyUsers
        WHERE IsMonitor = 1 AND IsActive = 1
    )
    SELECT COUNT(DISTINCT a.Id)
    FROM Agency a
    LEFT JOIN AgencyInscription ai ON a.AgencyInscriptionId = ai.Id
    LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
    LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
    LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
    WHERE a.IsPropietary = 0
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