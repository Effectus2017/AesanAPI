CREATE OR ALTER PROCEDURE [110_GetAgencyById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
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
        a.IsActive,
        a.IsListable,
        a.CreatedAt,
        a.UpdatedAt,
        a.AgencyCode,
        a.IsPropietary,
        
        -- Campos de AgencyInscription
        ai.Id AS AgencyInscriptionId,
        ai.NonProfit,
        ai.FederalFundsDenied,
        ai.StateFundsDenied,
        ai.OrganizedAthleticPrograms,
        ai.AtRiskService,
        ai.BasicEducationRegistry,
        ai.ServiceTime,
        ai.TaxExemptionStatus,
        ai.TaxExemptionType,
        ai.RejectionJustification,
        ai.AppointmentCoordinated,
        ai.AppointmentDate,
        ai.Comments,

        -- Datos del usuario de la agencia (owner)
        au.UserId as UserId,
        u2.FirstName AS UserFirstName,
        u2.MiddleName AS UserMiddleName,
        u2.FatherLastName AS UserFatherLastName,
        u2.MotherLastName AS UserMotherLastName,
        u2.AdministrationTitle as UserAdministrationTitle,

        -- Datos del usuario monitor
        aua.UserId as MonitorId,
        u.FirstName AS MonitorFirstName,
        u.FatherLastName AS MonitorFatherLastName

    FROM Agency a
    INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
    INNER JOIN City c ON a.CityId = c.Id
    INNER JOIN Region r ON a.RegionId = r.Id
    LEFT JOIN City pc ON a.PostalCityId = pc.Id
    LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
    LEFT JOIN AgencyInscription ai ON a.Id = ai.AgencyId
    LEFT JOIN AgencyUsers aua ON a.Id = aua.AgencyId AND aua.IsActive = 1 AND aua.IsMonitor = 1
    LEFT JOIN AspNetUsers u ON aua.UserId = u.Id
    LEFT JOIN AgencyUsers au ON a.Id = au.AgencyId AND au.IsActive = 1 AND au.IsOwner = 1
    LEFT JOIN AspNetUsers u2 ON au.UserId = u2.Id
    WHERE a.Id = @Id AND a.IsActive = 1;

    -- Obtener los programas asociados a la agencia
    SELECT
        p.Id,
        p.Name,
        p.Description,
        p.IsActive,
        p.CreatedAt,
        p.UpdatedAt
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
    WHERE ap.AgencyId = @Id;
END;
GO 


exec [110_GetAgencyById] 1;