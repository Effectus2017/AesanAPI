-- Procedimiento almacenado para obtener todas las ciudades
CREATE PROCEDURE [100_GetCities]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT
AS
BEGIN
    SELECT *
    FROM City
    WHERE (@alls = 1 OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;


    SELECT COUNT(*) AS TotalCount
    FROM City
    WHERE (@alls = 1 OR Name LIKE '%' + @name + '%');
END;
GO
-- Procedimiento almacenado para obtener todas las regiones
CREATE PROCEDURE [100_GetRegions]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT
AS
BEGIN
    SELECT *
    FROM Region
    WHERE (@alls = 1 OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM Region
    WHERE (@alls = 1 OR Name LIKE '%' + @name + '%');
END;
GO
-- Procedimiento almacenado para obtener regiones por ID de ciudad
CREATE PROCEDURE [100_GetRegionsByCityId] @cityId INT
AS
BEGIN
    SELECT *
    FROM Region
    WHERE CityId = @cityId;

    SELECT COUNT(*) AS TotalCount
    FROM Region
    WHERE CityId = @cityId;
END;
GO

-- Procedimiento almacenado para obtener una región por su ID
CREATE PROCEDURE [100_GetRegionById] @regionId INT
AS
BEGIN
    SELECT *
    FROM Region
    WHERE Id = @regionId;
END;
-- 
GO
CREATE OR ALTER PROCEDURE [100_InsertAgency]
    @Name NVARCHAR(MAX),
    @StatusId INT,
    @ProgramId INT,
    -- Datos de la Agencia
    @SdrNumber NVARCHAR(255),
    @UieNumber NVARCHAR(255),
    @EinNumber NVARCHAR(255),
    -- Datos de la Ciudad y Región
    @CityId INT,
    @RegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    -- Dirección y Teléfono
    @Address NVARCHAR(255),
    @ZipCode INT,
    @PostalAddress NVARCHAR(255),
    @Phone NVARCHAR(50),
    -- Datos del Contacto
    @Email NVARCHAR(255),
    @NonProfit BIT,
    @FederalFundsDenied BIT,
    @StateFundsDenied BIT,
    -- Justificación para rechazo
    @RejectionJustification NVARCHAR(MAX),
    -- Imágen - Logo
    @ImageURL NVARCHAR(MAX),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Agency (Name, StatusId, ProgramId, SdrNumber, UieNumber, EinNumber, CityId, RegionId, Latitude, Longitude, Address, ZipCode, PostalAddress, Phone, Email, NonProfit, FederalFundsDenied, StateFundsDenied, RejectionJustification, ImageURL)
    VALUES (@Name, @StatusId, @ProgramId, @SdrNumber, @UieNumber, @EinNumber, @CityId, @RegionId, @Latitude, @Longitude, @Address, @ZipCode, @PostalAddress, @Phone, @Email, @NonProfit, @FederalFundsDenied, @StateFundsDenied, @RejectionJustification, @ImageURL);

    SET @Id = SCOPE_IDENTITY(); -- Obtener el ID de la agencia insertada
END
GO

CREATE OR ALTER PROCEDURE [100_GetAgencies]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @programId INT = NULL,
    @statusId INT = NULL
AS
BEGIN
    SELECT 
        a.Id, 
        a.Name, 
        a.StatusId, 
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
        p.Id AS ProgramId,
        p.Name AS ProgramName,
        -- Datos del usuario administrador
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        -- Datos del usuario administrador
        u.AdministrationTitle,
        u.Email,
        -- Auditoría
        a.CreatedAt,
        a.UpdatedAt,
        -- Imágen - Logo
        a.ImageURL,
        -- Justificación para rechazo
        a.RejectionJustification
    FROM Agency a
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN AgencyStatus s ON a.StatusId = s.Id
        LEFT JOIN Program p ON a.ProgramId = p.Id
        LEFT JOIN AspNetUsers u ON u.AgencyId = a.Id
    WHERE (@alls = 1) 
    OR (
        (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@programId IS NULL OR a.ProgramId = @programId)
        AND (@statusId IS NULL OR a.StatusId = @statusId)
    )
    ORDER BY a.Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Actualizar también la consulta del conteo para incluir los mismos filtros
    SELECT COUNT(*) AS TotalCount
    FROM Agency a
    WHERE (@alls = 1) 
    OR (
        (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@programId IS NULL OR a.ProgramId = @programId)
        AND (@statusId IS NULL OR a.StatusId = @statusId)
    );
END;
GO

CREATE OR ALTER PROCEDURE [100_GetAgencyById] 
    @Id INT
AS
BEGIN
    SELECT 
        a.Id, 
        a.Name, 
        a.StatusId, 
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
        p.Id AS ProgramId,
        p.Name AS ProgramName,
        -- Datos del usuario administrador
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        -- Datos del usuario administrador
        u.AdministrationTitle,
        u.Email,
        -- Auditoría
        a.CreatedAt,
        a.UpdatedAt,
        -- Imágen - Logo
        a.ImageURL,
        -- Justificación para rechazo
        a.RejectionJustification
    FROM Agency a
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN AgencyStatus s ON a.StatusId = s.Id
        LEFT JOIN Program p ON a.ProgramId = p.Id
        LEFT JOIN AspNetUsers u ON u.AgencyId = a.Id
    WHERE a.Id = @Id;
END;
GO

CREATE PROCEDURE [100_GetAllAgencyStatus]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT
AS
BEGIN
    SELECT Id, Name
    FROM AgencyStatus
    WHERE (@alls = 1) 
    OR (
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    )
    ORDER BY Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM AgencyStatus
    WHERE (@alls = 1) 
    OR (
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    );
END;
GO
ALTER PROCEDURE [100_GetPrograms]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT
AS
BEGIN
    SELECT Id, Name, Description
    FROM Program
    WHERE (@alls = 1) 
    OR (
        (@name IS NULL OR Name LIKE '%' + @name + '%')
        AND Name <> 'AESAN'
    )
    ORDER BY Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM Program
    WHERE (@alls = 1) 
    OR (
        (@name IS NULL OR Name LIKE '%' + @name + '%')
        AND Name <> 'AESAN'
    );
END;
GO

CREATE OR ALTER PROCEDURE [100_InsertTemporaryPassword]
    @UserId NVARCHAR(450),
    @TemporaryPassword NVARCHAR(256)
AS
BEGIN
    INSERT INTO TemporaryPasswords (UserId, TemporaryPassword, CreatedAt)
    VALUES (@UserId, @TemporaryPassword, GETDATE());
END;
GO

CREATE OR ALTER PROCEDURE [100_DeleteTemporaryPassword]
    @UserId NVARCHAR(450)
AS
BEGIN
    DELETE FROM TemporaryPasswords
    WHERE UserId = @UserId;
END;
GO

CREATE OR ALTER PROCEDURE [100_UpdateAgency]
    @AgencyId INT,
    @Name NVARCHAR(MAX),
    @StatusId INT,
    @ProgramId INT,
    -- Datos de la Agencia
    @SdrNumber NVARCHAR(255),
    @UieNumber NVARCHAR(255),
    @EinNumber NVARCHAR(255),
    -- Datos de la Ciudad y Región
    @CityId INT,
    @RegionId INT,
    @Latitude FLOAT = NULL,
    @Longitude FLOAT = NULL,
    -- Dirección y Teléfono
    @Address NVARCHAR(255),
    @ZipCode INT,
    @PostalAddress NVARCHAR(255),
    @Phone NVARCHAR(50),
    -- Datos del Contacto
    @Email NVARCHAR(255),
    -- Imágen - Logo
    @ImageURL NVARCHAR(MAX) = NULL,
    -- Justificación para rechazo
    @RejectionJustification NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Agency
    SET 
        Name = @Name,
        StatusId = @StatusId,
        ProgramId = @ProgramId,
        SdrNumber = @SdrNumber,
        UieNumber = @UieNumber,
        EinNumber = @EinNumber,
        CityId = @CityId,
        RegionId = @RegionId,
        Latitude = CASE WHEN @Latitude IS NOT NULL THEN @Latitude ELSE Latitude END,
        Longitude = CASE WHEN @Longitude IS NOT NULL THEN @Longitude ELSE Longitude END,
        Address = @Address,
        ZipCode = @ZipCode,
        PostalAddress = @PostalAddress,
        Phone = @Phone,
        Email = @Email,
        ImageURL = CASE WHEN @ImageURL IS NOT NULL THEN @ImageURL ELSE ImageURL END,
        -- Justificación para rechazo
        RejectionJustification = CASE WHEN @RejectionJustification IS NOT NULL THEN @RejectionJustification ELSE RejectionJustification END
    WHERE Id = @AgencyId;

     -- Obtiene el número de filas afectadas
    SET @rowsAffected = @@ROWCOUNT;
    -- Retorna 1 si se actualizó al menos una fila, 0 si no
    RETURN CASE WHEN @rowsAffected > 0 THEN 1 ELSE 0 END;
END;
GO

CREATE OR ALTER PROCEDURE [100_UpdateAgencyLogo]
    @AgencyId INT,
    @ImageUrl NVARCHAR(MAX) -- Nueva URL de la imagen
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Actualizar la URL de la imagen
    UPDATE Agency
    SET 
        ImageURL = @ImageUrl,
        UpdatedAt = GETDATE()
    WHERE Id = @AgencyId;

    -- Obtiene el número de filas afectadas
    SET @rowsAffected = @@ROWCOUNT;

    -- Retorna 1 si se actualizó al menos una fila, 0 si no
    RETURN CASE WHEN @rowsAffected > 0 THEN 1 ELSE 0 END;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateAgencyStatus]
    @agencyId INT,
    @statusId INT,
    @rejectionJustification NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Actualiza el estado de la agencia
    UPDATE Agency
    SET 
        StatusId = @statusId,
        RejectionJustification = @rejectionJustification,
        UpdatedAt = GETDATE()
    WHERE Id = @agencyId;

    -- Obtiene el número de filas afectadas
    SET @rowsAffected = @@ROWCOUNT;

    -- Retorna 1 si se actualizó al menos una fila, 0 si no
    RETURN CASE WHEN @rowsAffected > 0 THEN 1 ELSE 0 END;
END;
