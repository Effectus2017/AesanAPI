-- Procedimiento para insertar una agencia con la nueva propiedad AtRiskService
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
    @AtRiskService BIT, -- Añadido
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
        OrganizedAthleticPrograms, AtRiskService, -- Añadido
        RejectionJustification, ImageURL, Comment,
        AppointmentCoordinated, AppointmentDate, IsListable, AgencyCode, IsPropietary
    )
    VALUES (
        @Name, @AgencyStatusId, @SdrNumber, @UieNumber, @EinNumber,
        @Address, @ZipCode, @Phone, @Email, @CityId, @RegionId,
        @PostalAddress, @PostalZipCode, @PostalCityId, @PostalRegionId,
        @Latitude, @Longitude, @NonProfit, @FederalFundsDenied, @StateFundsDenied,
        @OrganizedAthleticPrograms, @AtRiskService, -- Añadido
        @RejectionJustification, @ImageURL, @Comment,
        @AppointmentCoordinated, @AppointmentDate, @IsListable, @AgencyCode, false
    );

    SET @Id = SCOPE_IDENTITY();
    RETURN @Id;
END;
GO 