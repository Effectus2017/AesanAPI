SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para actualizar una agencia con las nuevas propiedades
CREATE OR ALTER PROCEDURE [103_UpdateAgency]
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
    -- Campos de elegibilidad
    @NonProfit BIT,
    @FederalFundsDenied BIT,
    @StateFundsDenied BIT,
    @OrganizedAthleticPrograms BIT,
    @AtRiskService BIT
    -- Nuevas propiedades
    -- @ServiceTime DATETIME,
    -- @TaxExemptionStatus INT,
    -- @TaxExemptionType INT
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
        -- Campos de elegibilidad
        NonProfit = @NonProfit,
        FederalFundsDenied = @FederalFundsDenied,
        StateFundsDenied = @StateFundsDenied,
        OrganizedAthleticPrograms = @OrganizedAthleticPrograms,
        AtRiskService = @AtRiskService,
        -- Nuevas propiedades
        -- ServiceTime = @ServiceTime,
        -- TaxExemptionStatus = @TaxExemptionStatus,
        -- TaxExemptionType = @TaxExemptionType,
        -- Auditoría
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND IsActive = 1;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END;
GO 