
-- Procedimiento para obtener una agencia por ID
CREATE OR ALTER PROCEDURE [101_GetAgencyById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        a.Id,
        a.Name,
         -- Estado de la agencia
        a.AgencyStatusId AS StatusId,
        -- Estado de la agencia
        ast.Name AS StatusName,
        -- Datos de la Agencia
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        -- Dirección y Teléfono
        a.Address,
        a.Phone,
        a.Email,
        -- Ciudad y Región física
        a.CityId,
        c.Name AS CityName,
        a.RegionId,
        r.Name AS RegionName,
        a.ZipCode,
        -- Dirección postal
        a.PostalAddress,
        a.PostalCityId,
        pc.Name AS PostalCityName,
        a.PostalRegionId,
        pr.Name AS PostalRegionName,
        a.PostalZipCode,
        -- Campos adicionales
        a.NonProfit,
        a.FederalFundsDenied,
        a.StateFundsDenied,
        a.OrganizedAthleticPrograms,
        a.RejectionJustification,
        a.ImageURL,
        a.Comment,
        a.AppointmentCoordinated,
        a.AppointmentDate,
        a.IsActive,
        a.IsListable,
         -- Auditoría
        a.CreatedAt,
        a.UpdatedAt,
        -- Coordenadas
        a.Latitude,
        a.Longitude,
        -- Código de la agencia
        a.AgencyCode,
        -- Datos del usuario
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        u.AdministrationTitle,
        u.Email
    FROM Agency a
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        LEFT JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
        LEFT JOIN AspNetUsers u ON u.AgencyId = a.Id
    WHERE a.Id = @Id;
    
    -- Get associated programs
    SELECT
        p.Id,
        p.Name,
        p.Description
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
    WHERE ap.AgencyId = @Id;
END;
GO