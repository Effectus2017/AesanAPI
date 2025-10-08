-- Obtener todas las agencias
-- 1.1.6
CREATE OR ALTER PROCEDURE [116_GetAgencies]
    @take INT = 10,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @programId INT = NULL,
    @statusId INT = NULL,
    @userId NVARCHAR(450) = NULL,
    @alls BIT = 0,
    @isList BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Si @isList = 1, retornar todas las agencias sin filtros
    IF @isList = 1
    BEGIN
        SELECT *
        FROM Agency
        ORDER BY CreatedAt DESC, Name;

        RETURN;
    END
    ELSE
    BEGIN
        -- CTEs para mejorar la performance (solo cuando @isList = 0)
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
                SELECT DISTINCT AgencyId, UserId, IsOwner
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
            -- Campos de inscripción actualizados
            ai.BasicEducationRegistry,
            ai.NonProfit,
            ai.FederalFundsDenied,
            ai.StateFundsDenied,
            ai.StateFundsDeniedReason,
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
            own.IsOwner,

            -- Datos del usuario de la agencia - desde Staff
            s.Id as UserId,
            s.FirstName AS UserFirstName,
            s.FatherLastName AS UserFatherLastName,

            -- Datos del usuario monitor - desde Staff
            s_monitor.Id as MonitorId,
            s_monitor.FirstName AS MonitorFirstName,
            s_monitor.FatherLastName AS MonitorFatherLastName,

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
            LEFT JOIN AspNetUsers u ON own.UserId = u.Id
            LEFT JOIN Staff s ON u.Id = s.UserId
            LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
            LEFT JOIN AspNetUsers mu ON mon.UserId = mu.Id
            LEFT JOIN Staff s_monitor ON mu.Id = s_monitor.UserId
        WHERE a.IsPropietary = 0
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

        -- Obtener programas asociados a las agencias filtradas y al usuario
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
            -- Usuario asignado al programa - desde Staff
            s_program.FirstName,
            s_program.MiddleName,
            s_program.FatherLastName,
            s_program.MotherLastName
        FROM Program p
            INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
            INNER JOIN FilteredAgencies fa ON ap.AgencyId = fa.Id
            LEFT JOIN AspNetUsers u ON ap.UserId = u.Id
            LEFT JOIN Staff s_program ON u.Id = s_program.UserId
        WHERE ap.IsActive = 1;

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
    END
END;
GO


EXEC [116_GetAgencies]
    @take = 10,
    @skip = 0,
    @name = NULL,
    @regionId = NULL,
    @cityId = NULL,
    @programId = NULL,
    @statusId = NULL,
    @userId = NULL,
    @alls = 1,
    @isList = 1;