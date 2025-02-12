-- Actualización de la tabla Agency
ALTER TABLE Agency
ADD AtRiskService BIT NULL DEFAULT 0;
GO
-- Stored Procedures de Agency
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

CREATE OR ALTER PROCEDURE [101_UpdateAgency]
    @Id INT,
    @Name NVARCHAR(255),
    @AgencyStatusId INT,
    -- Datos de la agencia
    @SdrNumber INT,
    @UieNumber INT,
    @EinNumber INT,
    -- Dirección fisica
    @Address NVARCHAR(255),
    @ZipCode INT,
    @CityId INT,
    @RegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    -- Dirección postal
    @PostalAddress NVARCHAR(255),
    @PostalZipCode INT,
    @PostalCityId INT,
    @PostalRegionId INT,
    -- Teléfono
    @Phone NVARCHAR(20),
    -- Imagen
    @ImageURL NVARCHAR(MAX) = NULL,
    -- Datos de contacto
    @Email NVARCHAR(255),
    -- Codigo de la agencia
    @AgencyCode NVARCHAR(50),
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Establecer el UserId en el contexto de la sesión
    EXEC sp_set_session_context @key = N'UserId', @value = @UserId;
    
    DECLARE @rowsAffected INT;

    UPDATE Agency
    SET Name = @Name,
        AgencyStatusId = @AgencyStatusId,
        -- Datos de la agencia
        SdrNumber = @SdrNumber,
        UieNumber = @UieNumber,
        EinNumber = @EinNumber,
        -- Dirección fisica
        Address = @Address,
        ZipCode = @ZipCode,
        CityId = @CityId,
        RegionId = @RegionId,
        Latitude = @Latitude,
        Longitude = @Longitude,
        -- Dirección postal
        PostalAddress = @PostalAddress,
        PostalZipCode = @PostalZipCode,
        PostalCityId = @PostalCityId,
        PostalRegionId = @PostalRegionId,
        -- Teléfono
        Phone = @Phone,
        -- Imagen
        ImageURL = @ImageURL,
        -- Datos de contacto
        Email = @Email,
        -- Codigo de la agencia
        AgencyCode = @AgencyCode,
        -- Auditoría
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE [102_InsertAgency]
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
    @PostalAddress NVARCHAR(255),
    @PostalZipCode INT,
    @PostalCityId INT,
    @PostalRegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    @NonProfit BIT,
    @FederalFundsDenied BIT,
    @StateFundsDenied BIT,
    @OrganizedAthleticPrograms BIT,
    @AtRiskService BIT,
    @RejectionJustification NVARCHAR(MAX) = NULL,
    @ImageURL NVARCHAR(MAX) = NULL,
    @Comment NVARCHAR(MAX) = NULL,
    @AppointmentCoordinated BIT = NULL,
    @AppointmentDate DATETIME = NULL,
    @IsListable BIT = 1,
    @AgencyCode NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Agency (
        Name, AgencyStatusId, SdrNumber, UieNumber, EinNumber,
        Address, ZipCode, Phone, Email, CityId, RegionId,
        PostalAddress, PostalZipCode, PostalCityId, PostalRegionId,
        Latitude, Longitude, NonProfit, FederalFundsDenied, StateFundsDenied,
        OrganizedAthleticPrograms, AtRiskService,
        RejectionJustification, ImageURL, Comment,
        AppointmentCoordinated, AppointmentDate, IsListable, AgencyCode
    )
    VALUES (
        @Name, @AgencyStatusId, @SdrNumber, @UieNumber, @EinNumber,
        @Address, @ZipCode, @Phone, @Email, @CityId, @RegionId,
        @PostalAddress, @PostalZipCode, @PostalCityId, @PostalRegionId,
        @Latitude, @Longitude, @NonProfit, @FederalFundsDenied, @StateFundsDenied,
        @OrganizedAthleticPrograms, @AtRiskService,
        @RejectionJustification, @ImageURL, @Comment,
        @AppointmentCoordinated, @AppointmentDate, @IsListable, @AgencyCode
    );

    SET @Id = SCOPE_IDENTITY();
    RETURN @Id;
END;
GO

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

-- Stored Procedure para inscripción de programa
CREATE OR ALTER PROCEDURE [100_InsertProgramInscription]
    @AgencyId INT,
    @ProgramId INT,
    @UserId NVARCHAR(450),
    @Comments NVARCHAR(MAX) = NULL,
    @AppointmentCoordinated BIT = NULL,
    @AppointmentDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si ya existe la inscripción
    IF EXISTS (
        SELECT 1 
        FROM AgencyProgram 
        WHERE AgencyId = @AgencyId 
        AND ProgramId = @ProgramId
    )
    BEGIN
        RAISERROR ('La agencia ya está inscrita en este programa.', 16, 1);
        RETURN -1;
    END

    -- Insertar la inscripción
    INSERT INTO AgencyProgram (
        AgencyId,
        ProgramId,
        UserId,
        Comments,
        AppointmentCoordinated,
        AppointmentDate,
        CreatedAt
    )
    VALUES (
        @AgencyId,
        @ProgramId,
        @UserId,
        @Comments,
        @AppointmentCoordinated,
        @AppointmentDate,
        GETDATE()
    );

    RETURN SCOPE_IDENTITY();
END;
GO 