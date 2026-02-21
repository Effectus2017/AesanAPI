-- =============================================
-- Stored Procedure: 118_GetAgencies
-- Fecha: 2025-01-XX
-- Descripción: Obtener todas las agencias con nueva lógica de acceso simplificada.
--              Reemplaza 117_GetAgencies con nueva lógica de acceso.
--              Solo roles super_administrator y administrator ven todas las agencias.
--              Todos los demás roles solo ven agencias asignadas en AgencyUsers.
-- =============================================

CREATE OR ALTER PROCEDURE [118_GetAgencies]
    @take INT = 10000000,
    @skip INT = 0,
    @name NVARCHAR(255) = NULL,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @programId INT = NULL,
    @statusId INT = NULL,
    @userId NVARCHAR(450) = NULL,
    @alls BIT = 0,
    @isPropietary BIT = NULL,
    @userFirstName NVARCHAR(255) = NULL,
    @statusName NVARCHAR(255) = NULL,
    @monitorFirstName NVARCHAR(255) = NULL,
    @createdAtFrom DATETIME2 = NULL,
    @createdAtTo DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- =============================================
    -- CTEs para mejorar la performance
    -- =============================================
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
            WHERE AgencyAssignmentType = 'AGENCY_OWNER' AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId, CreatedAt
            FROM AgencyUsers
            WHERE AgencyAssignmentType LIKE 'NUTRE_%' AND IsActive = 1
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
        ai.BasicEducationRegistry,
        ai.ExtendedHours,
        ai.ServicesOfferedSince,
        ai.NonProfit,
        ai.FederalFundsDenied,
        ai.FederalFundsDeniedReason,
        ai.StateFundsDenied,
        ai.StateFundsDeniedReason,
        ai.TaxExemptionStatusId,
        ai.TaxExemptionTypeId,
        ai.TypeOfEntityId,
        ai.TypeOfApplicantId,
        ai.PublicAllianceContractId,
        ai.NationalYouthProgram,
        ai.IsDayCareHomeId,
        ai.DeadlineToCompleteRegistration,
        ai.CompletedRegistrationDate,
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
        LEFT JOIN Staff ownerStaff ON ownerStaff.UserId = own.UserId
        LEFT JOIN Staff monitorStaff ON monitorStaff.UserId = mon.UserId
    WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
        AND (@name IS NULL OR @name = '' OR a.Name LIKE '%' + @name + '%')
        -- NUEVA LÓGICA DE ACCESO: Filtrar por AgencyUsers si no es SuperAdmin/Admin
        AND (
            @userId IS NULL
            OR EXISTS (
                SELECT 1 
                FROM AspNetUserRoles ur
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE ur.UserId = @userId
                    AND r.Name IN ('super_administrator', 'administrator')
            )
            OR EXISTS (
                SELECT 1 FROM AgencyUsers au 
                WHERE au.UserId = @userId 
                AND au.AgencyId = a.Id 
                AND au.IsActive = 1
            )
        )
        AND (
            @alls = 1
            OR (
                (@regionId IS NULL OR a.RegionId = @regionId)
                AND (@cityId IS NULL OR a.CityId = @cityId)
                AND (@programId IS NULL OR ap.ProgramId = @programId)
                AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
                AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId)
            )
        )
        AND (@userFirstName IS NULL OR @userFirstName = '' OR ownerStaff.FirstName LIKE '%' + @userFirstName + '%')
        AND (@statusName IS NULL OR @statusName = '' OR ast.Name LIKE '%' + @statusName + '%')
        AND (@monitorFirstName IS NULL OR @monitorFirstName = '' OR monitorStaff.FirstName LIKE '%' + @monitorFirstName + '%')
        AND (@createdAtFrom IS NULL OR a.CreatedAt >= @createdAtFrom)
        AND (@createdAtTo IS NULL OR a.CreatedAt <= @createdAtTo)
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
            WHERE AgencyAssignmentType = 'AGENCY_OWNER' AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE AgencyAssignmentType LIKE 'NUTRE_%' AND IsActive = 1
        ),
        FilteredAgencies
        AS
        (
            SELECT DISTINCT a.Id
            FROM Agency a
                INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
                LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
                LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
                LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
                LEFT JOIN Staff ownerStaff ON ownerStaff.UserId = own.UserId
                LEFT JOIN Staff monitorStaff ON monitorStaff.UserId = mon.UserId
            WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
                AND (@name IS NULL OR @name = '' OR a.Name LIKE '%' + @name + '%')
                -- NUEVA LÓGICA DE ACCESO
                AND (
                    @userId IS NULL
                    OR EXISTS (
                        SELECT 1 
                        FROM AspNetUserRoles ur
                        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                        WHERE ur.UserId = @userId
                            AND r.Name IN ('super_administrator', 'administrator')
                    )
                    OR EXISTS (
                        SELECT 1 FROM AgencyUsers au 
                        WHERE au.UserId = @userId 
                        AND au.AgencyId = a.Id 
                        AND au.IsActive = 1
                    )
                )
                AND (
                    @alls = 1
                    OR (
                        (@regionId IS NULL OR a.RegionId = @regionId)
                        AND (@cityId IS NULL OR a.CityId = @cityId)
                        AND (@programId IS NULL OR ap.ProgramId = @programId)
                        AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
                        AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId)
                    )
                )
                AND (@userFirstName IS NULL OR @userFirstName = '' OR ownerStaff.FirstName LIKE '%' + @userFirstName + '%')
                AND (@statusName IS NULL OR @statusName = '' OR ast.Name LIKE '%' + @statusName + '%')
                AND (@monitorFirstName IS NULL OR @monitorFirstName = '' OR monitorStaff.FirstName LIKE '%' + @monitorFirstName + '%')
                AND (@createdAtFrom IS NULL OR a.CreatedAt >= @createdAtFrom)
                AND (@createdAtTo IS NULL OR a.CreatedAt <= @createdAtTo)
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
            WHERE AgencyAssignmentType = 'AGENCY_OWNER' AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId, CreatedAt
            FROM AgencyUsers
            WHERE AgencyAssignmentType LIKE 'NUTRE_%' AND IsActive = 1
        ),
        FilteredAgencies
        AS
        (
            SELECT DISTINCT a.Id
            FROM Agency a
                INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
                LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
                LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
                LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
                LEFT JOIN Staff ownerStaff ON ownerStaff.UserId = own.UserId
                LEFT JOIN Staff monitorStaff ON monitorStaff.UserId = mon.UserId
            WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
                AND (@name IS NULL OR @name = '' OR a.Name LIKE '%' + @name + '%')
                -- NUEVA LÓGICA DE ACCESO
                AND (
                    @userId IS NULL
                    OR EXISTS (
                        SELECT 1 
                        FROM AspNetUserRoles ur
                        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                        WHERE ur.UserId = @userId
                            AND r.Name IN ('super_administrator', 'administrator')
                    )
                    OR EXISTS (
                        SELECT 1 FROM AgencyUsers au 
                        WHERE au.UserId = @userId 
                        AND au.AgencyId = a.Id 
                        AND au.IsActive = 1
                    )
                )
                AND (
                    @alls = 1
                    OR (
                        (@regionId IS NULL OR a.RegionId = @regionId)
                        AND (@cityId IS NULL OR a.CityId = @cityId)
                        AND (@programId IS NULL OR ap.ProgramId = @programId)
                        AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
                        AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId)
                    )
                )
                AND (@userFirstName IS NULL OR @userFirstName = '' OR ownerStaff.FirstName LIKE '%' + @userFirstName + '%')
                AND (@statusName IS NULL OR @statusName = '' OR ast.Name LIKE '%' + @statusName + '%')
                AND (@monitorFirstName IS NULL OR @monitorFirstName = '' OR monitorStaff.FirstName LIKE '%' + @monitorFirstName + '%')
                AND (@createdAtFrom IS NULL OR a.CreatedAt >= @createdAtFrom)
                AND (@createdAtTo IS NULL OR a.CreatedAt <= @createdAtTo)
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
        s.Id as Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        s.PositionId,
        os.Name AS PositionName,
        os.NameEN AS PositionNameEN,
        os.OptionKey AS PositionOptionKey,
        s.ContractStartDate,
        s.ContractEndDate,
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
            WHERE AgencyAssignmentType = 'AGENCY_OWNER' AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE AgencyAssignmentType LIKE 'NUTRE_%' AND IsActive = 1
        ),
        FilteredAgencies
        AS
        (
            SELECT DISTINCT a.Id
            FROM Agency a
                INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
                LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
                LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
                LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
                LEFT JOIN Staff ownerStaff ON ownerStaff.UserId = own.UserId
                LEFT JOIN Staff monitorStaff ON monitorStaff.UserId = mon.UserId
            WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
                AND (@name IS NULL OR @name = '' OR a.Name LIKE '%' + @name + '%')
                -- NUEVA LÓGICA DE ACCESO
                AND (
                    @userId IS NULL
                    OR EXISTS (
                        SELECT 1 
                        FROM AspNetUserRoles ur
                        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                        WHERE ur.UserId = @userId
                            AND r.Name IN ('super_administrator', 'administrator')
                    )
                    OR EXISTS (
                        SELECT 1 FROM AgencyUsers au 
                        WHERE au.UserId = @userId 
                        AND au.AgencyId = a.Id 
                        AND au.IsActive = 1
                    )
                )
                AND (
                    @alls = 1
                    OR (
                        (@regionId IS NULL OR a.RegionId = @regionId)
                        AND (@cityId IS NULL OR a.CityId = @cityId)
                        AND (@programId IS NULL OR ap.ProgramId = @programId)
                        AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
                        AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId)
                    )
                )
                AND (@userFirstName IS NULL OR @userFirstName = '' OR ownerStaff.FirstName LIKE '%' + @userFirstName + '%')
                AND (@statusName IS NULL OR @statusName = '' OR ast.Name LIKE '%' + @statusName + '%')
                AND (@monitorFirstName IS NULL OR @monitorFirstName = '' OR monitorStaff.FirstName LIKE '%' + @monitorFirstName + '%')
                AND (@createdAtFrom IS NULL OR a.CreatedAt >= @createdAtFrom)
                AND (@createdAtTo IS NULL OR a.CreatedAt <= @createdAtTo)
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
        s.Id as Id,
        s.FirstName,
        s.MiddleName,
        s.FatherLastName,
        s.MotherLastName,
        s.PositionId,
        os.Name AS PositionName,
        os.NameEN AS PositionNameEN,
        os.OptionKey AS PositionOptionKey,
        s.ContractStartDate,
        s.ContractEndDate,
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
            WHERE AgencyAssignmentType = 'AGENCY_OWNER' AND IsActive = 1
        ),
        AgencyMonitorsCTE
        AS
        (
            SELECT DISTINCT AgencyId, UserId
            FROM AgencyUsers
            WHERE AgencyAssignmentType LIKE 'NUTRE_%' AND IsActive = 1
        )
    SELECT COUNT(DISTINCT a.Id)
    FROM Agency a
        INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
        LEFT JOIN AgencyInscription ai ON a.Id = ai.AgencyId
        LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
        LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
        LEFT JOIN AgencyMonitorsCTE mon ON a.Id = mon.AgencyId
        LEFT JOIN Staff ownerStaff ON ownerStaff.UserId = own.UserId
        LEFT JOIN Staff monitorStaff ON monitorStaff.UserId = mon.UserId
    WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
        AND (@name IS NULL OR @name = '' OR a.Name LIKE '%' + @name + '%')
        -- NUEVA LÓGICA DE ACCESO
        AND (
            @userId IS NULL
            OR EXISTS (
                SELECT 1 
                FROM AspNetUserRoles ur
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE ur.UserId = @userId
                    AND r.Name IN ('super_administrator', 'administrator')
            )
            OR EXISTS (
                SELECT 1 FROM AgencyUsers au 
                WHERE au.UserId = @userId 
                AND au.AgencyId = a.Id 
                AND au.IsActive = 1
            )
        )
        AND (
            @alls = 1
            OR (
                (@regionId IS NULL OR a.RegionId = @regionId)
                AND (@cityId IS NULL OR a.CityId = @cityId)
                AND (@programId IS NULL OR ap.ProgramId = @programId)
                AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
                AND (@userId IS NULL OR own.UserId = @userId OR mon.UserId = @userId)
                AND (a.IsListable = 1)
            )
        )
        AND (@userFirstName IS NULL OR @userFirstName = '' OR ownerStaff.FirstName LIKE '%' + @userFirstName + '%')
        AND (@statusName IS NULL OR @statusName = '' OR ast.Name LIKE '%' + @statusName + '%')
        AND (@monitorFirstName IS NULL OR @monitorFirstName = '' OR monitorStaff.FirstName LIKE '%' + @monitorFirstName + '%')
        AND (@createdAtFrom IS NULL OR a.CreatedAt >= @createdAtFrom)
        AND (@createdAtTo IS NULL OR a.CreatedAt <= @createdAtTo);
END;
GO
