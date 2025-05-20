CREATE OR ALTER PROCEDURE [102_GetAgencies]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @programId INT = NULL,
    @statusId INT = NULL,
    @userId VARCHAR(36) = NULL
AS
BEGIN
    SELECT DISTINCT
        a.Id,
        a.Name,
        a.AgencyStatusId AS StatusId,
        -- Datos de la Agencia
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        -- Dirección y Teléfono
        a.Address,
        a.ZipCode,
        a.PostalAddress,
        a.Phone,
        -- Coordenadas
        a.Latitude,
        a.Longitude,
        -- Joins para las relaciones
        c.Id AS CityId,
        c.Name AS CityName,
        r.Id AS RegionId,
        r.Name AS RegionName,
        s.Id AS StatusId,
        s.Name AS StatusName,
        -- Datos del usuario
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        u.AdministrationTitle,
        u.Email,
        -- Auditoría
        a.CreatedAt,
        a.UpdatedAt,
        -- Imágen - Logo
        a.ImageURL,
        -- Justificación para rechazo
        a.RejectionJustification,
        -- Datos del programa
        ap.Comments,
        ap.AppointmentCoordinated,
        ap.AppointmentDate,
        -- Código de la agencia
        a.AgencyCode
    FROM Agency a
        INNER JOIN AgencyProgram ap ON a.Id = ap.AgencyId
        LEFT JOIN UserProgram up ON ap.ProgramId = up.ProgramId
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN AgencyStatus s ON a.AgencyStatusId = s.Id
        LEFT JOIN AspNetUsers u ON u.AgencyId = a.Id
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
        AND (@userId IS NULL OR up.UserId = @userId))
        AND a.IsListable = 1
    ORDER BY a.Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Obtener programas asociados a las agencias filtradas y al usuario
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
        INNER JOIN Agency a ON ap.AgencyId = a.Id
        INNER JOIN UserProgram up ON p.Id = up.ProgramId
        LEFT JOIN AspNetUsers u ON ap.UserId = u.Id
    WHERE 
        up.UserId = @userId
        AND (@alls = 1 OR ap.ProgramId = @programId)
        AND (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND a.IsListable = 1;

   -- Obtener el conteo total
   SELECT COUNT(DISTINCT a.Id) AS TotalCount
    FROM Agency a
     	LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId
        LEFT JOIN UserProgram up ON ap.ProgramId = up.ProgramId
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@statusId IS NULL OR a.AgencyStatusId = @statusId)
        AND (@userId IS NULL OR up.UserId = @userId)
    )
        AND a.IsListable = 1;
   
END;
GO 