-- Procedimiento para actualizar una agencia con el nuevo campo BasicEducationRegistry
CREATE OR ALTER PROCEDURE [109_UpdateAgency]
    @Id INT,
    @Name NVARCHAR(255),
    @AgencyStatusId INT,
    @SdrNumber INT,
    @UieNumber INT,
    @EinNumber INT,
    @Address NVARCHAR(255),
    @ZipCode NVARCHAR(20),
    @Phone NVARCHAR(20),
    @Email NVARCHAR(255),
    @CityId INT,
    @RegionId INT,
    @PostalAddress NVARCHAR(255),
    @PostalZipCode NVARCHAR(20),
    @PostalCityId INT,
    @PostalRegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    @NonProfit BIT,
    @BasicEducationRegistry INT,
    @FederalFundsDenied BIT,
    @StateFundsDenied BIT,
    @OrganizedAthleticPrograms BIT,
    @AtRiskService BIT,
    @ServiceTime DATETIME,
    @TaxExemptionStatus INT,
    @TaxExemptionType INT,
    @RejectionJustification NVARCHAR(MAX) = NULL,
    @ImageURL NVARCHAR(MAX) = NULL,
    @IsListable BIT = 1,
    @AgencyCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

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
        PostalAddress = @PostalAddress,
        PostalZipCode = @PostalZipCode,
        PostalCityId = @PostalCityId,
        PostalRegionId = @PostalRegionId,
        Latitude = @Latitude,
        Longitude = @Longitude,
        NonProfit = @NonProfit,
        BasicEducationRegistry = @BasicEducationRegistry,
        FederalFundsDenied = @FederalFundsDenied,
        StateFundsDenied = @StateFundsDenied,
        OrganizedAthleticPrograms = @OrganizedAthleticPrograms,
        AtRiskService = @AtRiskService,
        ServiceTime = @ServiceTime,
        TaxExemptionStatus = @TaxExemptionStatus,
        TaxExemptionType = @TaxExemptionType,
        RejectionJustification = @RejectionJustification,
        ImageURL = @ImageURL,
        IsListable = @IsListable,
        AgencyCode = @AgencyCode,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;

    RETURN 0;
END;
GO 