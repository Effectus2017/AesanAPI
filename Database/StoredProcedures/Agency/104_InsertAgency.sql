SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para insertar una agencia con ZipCode y PostalZipCode como NVARCHAR
CREATE OR ALTER PROCEDURE [104_InsertAgency]
    @Name NVARCHAR(255),
    @AgencyStatusId INT,
    @SdrNumber INT,
    @UieNumber INT,
    @EinNumber INT,
    @Address NVARCHAR(255),
    @ZipCode NVARCHAR(20),  -- Cambiado de INT a NVARCHAR(20)
    @Phone NVARCHAR(20),
    @Email NVARCHAR(255),
    @CityId INT,
    @RegionId INT,
    @PostalAddress NVARCHAR(255),
    @PostalZipCode NVARCHAR(20),  -- Cambiado de INT a NVARCHAR(20)
    @PostalCityId INT,
    @PostalRegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    @NonProfit BIT,
    @FederalFundsDenied BIT,
    @StateFundsDenied BIT,
    @OrganizedAthleticPrograms BIT,
    @AtRiskService BIT,
    @ServiceTime DATETIME,
    @TaxExemptionStatus INT,
    @TaxExemptionType INT,
    @ImageURL NVARCHAR(MAX) = NULL,
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
        ServiceTime, TaxExemptionStatus, TaxExemptionType,
        ImageURL, 
        IsListable, AgencyCode, IsPropietary
    )
    VALUES (
        @Name, @AgencyStatusId, @SdrNumber, @UieNumber, @EinNumber,
        @Address, @ZipCode, @Phone, @Email, @CityId, @RegionId,
        @PostalAddress, @PostalZipCode, @PostalCityId, @PostalRegionId,
        @Latitude, @Longitude, @NonProfit, @FederalFundsDenied, @StateFundsDenied,
        @OrganizedAthleticPrograms, @AtRiskService, 
        @ServiceTime, @TaxExemptionStatus, @TaxExemptionType,
        @ImageURL, 
        @IsListable, @AgencyCode, 0
    );

    SET @Id = SCOPE_IDENTITY();
    RETURN @Id;
END;
GO 