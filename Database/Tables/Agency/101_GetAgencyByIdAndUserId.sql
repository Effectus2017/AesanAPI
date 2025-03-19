
-- Procedimiento almacenado para obtener una agencia por su ID para la visita preoperacional
ALTER PROCEDURE [dbo].[101_GetAgencyByIdAndUserId]
    @agencyId INT,
    @userId NVARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;

    -- Primera consulta: Obtener los datos de la agencia
    SELECT
        a.*,
        -- Ciudad y Región
        c.Id AS CityId,
        c.Name AS CityName,
        r.Id AS RegionId,
        r.Name AS RegionName,
        -- Ciudad y Región Postal
        pc.Id AS PostalCityId,
        pc.Name AS PostalCityName,
        pr.Id AS PostalRegionId,
        pr.Name AS PostalRegionName,
        -- Estado
        s.Id AS StatusId,
        s.Name AS StatusName,
        -- Usuario
        u.Id AS UserId,
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        u.AdministrationTitle,
        -- Comments
        ap.Comments,
        ap.AppointmentCoordinated AS AppointmentCoordinated,
        ap.AppointmentDate AS AppointmentDate
    FROM Agency a
        -- Joins para Ciudad y Región
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        -- Joins para Ciudad y Región Postal
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        -- Join para Estado
        LEFT JOIN AgencyStatus s ON a.AgencyStatusId = s.Id
        -- Join para Usuario
        LEFT JOIN AspNetUsers u ON a.Id = u.AgencyId
        -- Join para Programas
        LEFT JOIN AgencyProgram ap ON ap.AgencyId = a.id
        LEFT JOIN UserProgram up ON up.ProgramId = ap.ProgramId
    WHERE a.Id = @agencyId AND up.UserId = @userId;

    -- Segunda consulta: Obtener los programas asociados al usuario y la agencia
    SELECT DISTINCT
        p.Id,
        p.Name,
        p.Description,
        ap.AgencyId
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
        INNER JOIN UserProgram up ON p.Id = up.ProgramId
    WHERE ap.AgencyId = @agencyId AND up.UserId = @userId;

    -- Obtener el usuario que hizo el appointment
    SELECT
        u.Id,
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName
    FROM AspNetUsers u
        INNER JOIN AgencyProgram ap ON ap.UserId = u.Id AND ap.AgencyId = @agencyId;
END;
GO