-- Procedimiento para obtener todas las agencias
CREATE OR ALTER PROCEDURE [101_GetAgencies]
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
        -- Datos del usuario administrador
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
        ap.AppointmentDate
    FROM Agency a
        INNER JOIN AgencyProgram ap ON a.Id = ap.AgencyId
        INNER JOIN UserProgram up ON ap.ProgramId = up.ProgramId
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

-- Procedimiento para obtener una agencia por ID
CREATE OR ALTER PROCEDURE [101_GetAgencyById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        a.Id,
        a.Name,
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
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
        -- Estado de la agencia
        a.AgencyStatusId AS StatusId,
        ast.Name AS StatusName,
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
        a.CreatedAt,
        a.UpdatedAt,
        -- Coordenadas
        a.Latitude,
        a.Longitude,
        a.AgencyCode
    FROM Agency a
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        LEFT JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
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

-- Procedimiento para actualizar una agencia
CREATE OR ALTER PROCEDURE [100_UpdateAgency]
    @Id INT,
    @Name NVARCHAR(255),
    @AgencyStatusId INT,
    @SdrNumber INT,
    @UieNumber INT,
    @EinNumber INT,
    @Address NVARCHAR(255),
    @ZipCode INT,
    @Phone NVARCHAR(20),
    @Email NVARCHAR(255),
    @CityId INT,
    @RegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    @PostalAddress NVARCHAR(255),
    @PostalZipCode INT,
    @PostalCityId INT,
    @PostalRegionId INT,
    @NonProfit BIT,
    @FederalFundsDenied BIT,
    @StateFundsDenied BIT,
    @OrganizedAthleticPrograms BIT,
    @RejectionJustification NVARCHAR(MAX) = NULL,
    @ImageURL NVARCHAR(MAX) = NULL,
    @Comment NVARCHAR(MAX) = NULL,
    @AppointmentCoordinated BIT = NULL,
    @AppointmentDate DATETIME = NULL,
    @AgencyCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Agency
    SET Name = @Name,
        AgencyStatusId = @AgencyStatusId,
        SdrNumber = @SdrNumber,
        UieNumber = @UieNumber,
        EinNumber = @EinNumber,
        Address = @Address,
        ZipCode = @ZipCode,
        Phone = @Phone,
        Email = @Email,
        CityId = @CityId,
        RegionId = @RegionId,
        Latitude = @Latitude,
        Longitude = @Longitude,
        PostalAddress = @PostalAddress,
        PostalZipCode = @PostalZipCode,
        PostalCityId = @PostalCityId,
        PostalRegionId = @PostalRegionId,
        NonProfit = @NonProfit,
        FederalFundsDenied = @FederalFundsDenied,
        StateFundsDenied = @StateFundsDenied,
        OrganizedAthleticPrograms = @OrganizedAthleticPrograms,
        RejectionJustification = @RejectionJustification,
        ImageURL = @ImageURL,
        Comment = @Comment,
        AppointmentCoordinated = @AppointmentCoordinated,
        AppointmentDate = @AppointmentDate,
        AgencyCode = @AgencyCode,
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO

-- Procedimiento para eliminar una agencia (soft delete)
CREATE OR ALTER PROCEDURE [100_DeleteAgency]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Agency
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 

-- Procedimiento almacenado para obtener una agencia por su ID para la visita preoperacional
CREATE OR ALTER PROCEDURE [dbo].[101_GetAgencyByIdAndUserId]
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
        ap.Comment,
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
        LEFT JOIN AgencyStatus s ON a.StatusId = s.Id
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