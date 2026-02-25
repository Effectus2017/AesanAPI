-- =============================================
-- Stored Procedure: 119_GetAgencies
-- Fecha: 2026-02-25
-- Descripción: Reemplaza 118_GetAgencies.
--              - Incluye AgencyId en resultado de programas (para mapear por agencia).
--              - Devuelve todos los usuarios asignados a cada agencia (AgencyUsers, por etapa/asignación; pueden ser varios).
--              - No usa concepto "monitors": la agencia tiene usuarios asignados por etapa (AgencyUsers).
--              Acceso: admin o cualquier usuario con asignación en AgencyUsers para esa agencia.
-- =============================================

CREATE OR ALTER PROCEDURE [119_GetAgencies]
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
    @createdatfrom DATETIME2 = NULL,
    @createdatto DATETIME2 = NULL,
    @uieNumber BIGINT = NULL,
    @einNumber INT = NULL,
    @sdrNumber BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- =============================================
    -- CTEs (owners para filtro; acceso por AgencyUsers, cualquier asignación)
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
        LEFT JOIN Staff ownerStaff ON ownerStaff.UserId = own.UserId
    WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
        AND (@name IS NULL OR @name = '' OR a.Name LIKE '%' + @name + '%')
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
                AND (@userId IS NULL OR own.UserId = @userId OR EXISTS (SELECT 1 FROM AgencyUsers au2 WHERE au2.UserId = @userId AND au2.AgencyId = a.Id AND au2.IsActive = 1))
            )
        )
        AND (@userFirstName IS NULL OR @userFirstName = '' OR ownerStaff.FirstName LIKE '%' + @userFirstName + '%')
        AND (@statusName IS NULL OR @statusName = '' OR ast.Name LIKE '%' + @statusName + '%')
        AND (@createdatfrom IS NULL OR a.CreatedAt >= @createdatfrom)
        AND (@createdatto IS NULL OR a.CreatedAt <= @createdatto)
        AND (@uieNumber IS NULL OR CAST(a.UieNumber AS NVARCHAR(20)) LIKE CAST(@uieNumber AS NVARCHAR(20)) + N'%')
        AND (@einNumber IS NULL OR CAST(a.EinNumber AS NVARCHAR(20)) LIKE CAST(@einNumber AS NVARCHAR(20)) + N'%')
        AND (@sdrNumber IS NULL OR CAST(a.SdrNumber AS NVARCHAR(20)) LIKE CAST(@sdrNumber AS NVARCHAR(20)) + N'%')
    ORDER BY a.CreatedAt DESC, a.Name
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Programas (con AgencyId para mapear por agencia)
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
        FilteredAgencies
        AS
        (
            SELECT DISTINCT a.Id
            FROM Agency a
                INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
                LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
                LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
                LEFT JOIN Staff ownerStaff ON ownerStaff.UserId = own.UserId
            WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
                AND (@name IS NULL OR @name = '' OR a.Name LIKE '%' + @name + '%')
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
                        AND (@userId IS NULL OR own.UserId = @userId OR EXISTS (SELECT 1 FROM AgencyUsers au2 WHERE au2.UserId = @userId AND au2.AgencyId = a.Id AND au2.IsActive = 1))
                    )
                )
                AND (@userFirstName IS NULL OR @userFirstName = '' OR ownerStaff.FirstName LIKE '%' + @userFirstName + '%')
                AND (@statusName IS NULL OR @statusName = '' OR ast.Name LIKE '%' + @statusName + '%')
                AND (@createdatfrom IS NULL OR a.CreatedAt >= @createdatfrom)
                AND (@createdatto IS NULL OR a.CreatedAt <= @createdatto)
                AND (@uieNumber IS NULL OR CAST(a.UieNumber AS NVARCHAR(20)) LIKE CAST(@uieNumber AS NVARCHAR(20)) + N'%')
                AND (@einNumber IS NULL OR CAST(a.EinNumber AS NVARCHAR(20)) LIKE CAST(@einNumber AS NVARCHAR(20)) + N'%')
                AND (@sdrNumber IS NULL OR CAST(a.SdrNumber AS NVARCHAR(20)) LIKE CAST(@sdrNumber AS NVARCHAR(20)) + N'%')
            ORDER BY a.Id
            OFFSET @skip ROWS
            FETCH NEXT @take ROWS ONLY
        )
    SELECT DISTINCT
        ap.AgencyId,
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

    -- Owners
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
        FilteredAgencies
        AS
        (
            SELECT DISTINCT a.Id
            FROM Agency a
                INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
                LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
                LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
                LEFT JOIN Staff ownerStaff ON ownerStaff.UserId = own.UserId
            WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
                AND (@name IS NULL OR @name = '' OR a.Name LIKE '%' + @name + '%')
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
                        AND (@userId IS NULL OR own.UserId = @userId OR EXISTS (SELECT 1 FROM AgencyUsers au2 WHERE au2.UserId = @userId AND au2.AgencyId = a.Id AND au2.IsActive = 1))
                    )
                )
                AND (@userFirstName IS NULL OR @userFirstName = '' OR ownerStaff.FirstName LIKE '%' + @userFirstName + '%')
                AND (@statusName IS NULL OR @statusName = '' OR ast.Name LIKE '%' + @statusName + '%')
                AND (@createdatfrom IS NULL OR a.CreatedAt >= @createdatfrom)
                AND (@createdatto IS NULL OR a.CreatedAt <= @createdatto)
                AND (@uieNumber IS NULL OR CAST(a.UieNumber AS NVARCHAR(20)) LIKE CAST(@uieNumber AS NVARCHAR(20)) + N'%')
                AND (@einNumber IS NULL OR CAST(a.EinNumber AS NVARCHAR(20)) LIKE CAST(@einNumber AS NVARCHAR(20)) + N'%')
                AND (@sdrNumber IS NULL OR CAST(a.SdrNumber AS NVARCHAR(20)) LIKE CAST(@sdrNumber AS NVARCHAR(20)) + N'%')
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

    -- Usuarios asignados a la agencia (todos los AgencyUsers por etapa/asignación; pueden ser varios por agencia)
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
        FilteredAgencies
        AS
        (
            SELECT DISTINCT a.Id
            FROM Agency a
                INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
                LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
                LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
                LEFT JOIN Staff ownerStaff ON ownerStaff.UserId = own.UserId
            WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
                AND (@name IS NULL OR @name = '' OR a.Name LIKE '%' + @name + '%')
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
                        AND (@userId IS NULL OR own.UserId = @userId OR EXISTS (SELECT 1 FROM AgencyUsers au2 WHERE au2.UserId = @userId AND au2.AgencyId = a.Id AND au2.IsActive = 1))
                    )
                )
                AND (@userFirstName IS NULL OR @userFirstName = '' OR ownerStaff.FirstName LIKE '%' + @userFirstName + '%')
                AND (@statusName IS NULL OR @statusName = '' OR ast.Name LIKE '%' + @statusName + '%')
                AND (@createdatfrom IS NULL OR a.CreatedAt >= @createdatfrom)
                AND (@createdatto IS NULL OR a.CreatedAt <= @createdatto)
                AND (@uieNumber IS NULL OR CAST(a.UieNumber AS NVARCHAR(20)) LIKE CAST(@uieNumber AS NVARCHAR(20)) + N'%')
                AND (@einNumber IS NULL OR CAST(a.EinNumber AS NVARCHAR(20)) LIKE CAST(@einNumber AS NVARCHAR(20)) + N'%')
                AND (@sdrNumber IS NULL OR CAST(a.SdrNumber AS NVARCHAR(20)) LIKE CAST(@sdrNumber AS NVARCHAR(20)) + N'%')
            ORDER BY a.Id
            OFFSET @skip ROWS
            FETCH NEXT @take ROWS ONLY
        )
    SELECT DISTINCT
        au.AgencyId,
        au.UserId,
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
    FROM AgencyUsers au
        INNER JOIN FilteredAgencies fa ON au.AgencyId = fa.Id
        LEFT JOIN AspNetUsers u ON au.UserId = u.Id
        LEFT JOIN Staff s ON u.Id = s.UserId
        LEFT JOIN OptionSelection os ON s.PositionId = os.Id
        LEFT JOIN OptionSelection os_status ON s.StatusId = os_status.Id
    WHERE au.IsActive = 1
    ORDER BY au.AgencyId, au.UserId;

    -- Count
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
        )
    SELECT COUNT(DISTINCT a.Id)
    FROM Agency a
        INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
        LEFT JOIN AgencyInscription ai ON a.Id = ai.AgencyId
        LEFT JOIN AgencyProgramsCTE ap ON a.Id = ap.AgencyId
        LEFT JOIN AgencyOwnersCTE own ON a.Id = own.AgencyId
        LEFT JOIN Staff ownerStaff ON ownerStaff.UserId = own.UserId
    WHERE (@isPropietary IS NULL OR a.IsPropietary = @isPropietary)
        AND (@name IS NULL OR @name = '' OR a.Name LIKE '%' + @name + '%')
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
                AND (@userId IS NULL OR own.UserId = @userId OR EXISTS (SELECT 1 FROM AgencyUsers au2 WHERE au2.UserId = @userId AND au2.AgencyId = a.Id AND au2.IsActive = 1))
                AND (a.IsListable = 1)
            )
        )
        AND (@userFirstName IS NULL OR @userFirstName = '' OR ownerStaff.FirstName LIKE '%' + @userFirstName + '%')
        AND (@statusName IS NULL OR @statusName = '' OR ast.Name LIKE '%' + @statusName + '%')
        AND (@createdatfrom IS NULL OR a.CreatedAt >= @createdatfrom)
        AND (@createdatto IS NULL OR a.CreatedAt <= @createdatto)
        AND (@uieNumber IS NULL OR CAST(a.UieNumber AS NVARCHAR(20)) LIKE CAST(@uieNumber AS NVARCHAR(20)) + N'%')
        AND (@einNumber IS NULL OR CAST(a.EinNumber AS NVARCHAR(20)) LIKE CAST(@einNumber AS NVARCHAR(20)) + N'%')
        AND (@sdrNumber IS NULL OR CAST(a.SdrNumber AS NVARCHAR(20)) LIKE CAST(@sdrNumber AS NVARCHAR(20)) + N'%');
END;
GO
