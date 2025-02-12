-- Procedimiento para actualizar una agencia
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
    @AgencyCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
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
