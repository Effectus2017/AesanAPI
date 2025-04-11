CREATE OR ALTER PROCEDURE [110_InsertAgency]
    @Name nvarchar(255),
    @AgencyStatusId int,
    @CityId int,
    @PostalCityId int,
    @RegionId int,
    @PostalRegionId int,
    @UieNumber int,
    @EinNumber int,
    @SdrNumber int,
    @Address nvarchar(255),
    @ZipCode nvarchar(20),
    @PostalAddress nvarchar(255),
    @PostalZipCode nvarchar(20),
    @Phone nvarchar(20),
    @Email nvarchar(255),
    @Latitude real,
    @Longitude real,
    @ImageURL nvarchar(max),
    @IsActive bit,
    @IsListable bit,
    @AgencyCode nvarchar(50),
    -- Campos para AgencyInscription
    @NonProfit bit,
    @FederalFundsDenied bit,
    @StateFundsDenied bit,
    @OrganizedAthleticPrograms bit,
    @AtRiskService bit,
    @BasicEducationRegistry int,
    @ServiceTime datetime,
    @TaxExemptionStatus int,
    @TaxExemptionType int,
    @IsPropietary bit,
    @Id int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Primero insertamos la inscripción
        DECLARE @AgencyInscriptionId int;
        
        INSERT INTO AgencyInscription (
            NonProfit, FederalFundsDenied, StateFundsDenied,
            OrganizedAthleticPrograms, AtRiskService,
            BasicEducationRegistry, ServiceTime,
            TaxExemptionStatus, TaxExemptionType
        )
        VALUES (
            @NonProfit, @FederalFundsDenied, @StateFundsDenied,
            @OrganizedAthleticPrograms, @AtRiskService,
            @BasicEducationRegistry, @ServiceTime,
            @TaxExemptionStatus, @TaxExemptionType
        );
        
        SET @AgencyInscriptionId = SCOPE_IDENTITY();

        -- Luego insertamos la agencia
        INSERT INTO Agency (
            Name, AgencyStatusId, CityId, PostalCityId,
            RegionId, PostalRegionId, UieNumber, EinNumber,
            SdrNumber, Address, ZipCode, PostalAddress,
            PostalZipCode, Phone, Email, Latitude,
            Longitude, ImageURL, IsActive, IsListable,
            AgencyCode, AgencyInscriptionId, IsPropietary
        )
        VALUES (
            @Name, @AgencyStatusId, @CityId, @PostalCityId,
            @RegionId, @PostalRegionId, @UieNumber, @EinNumber,
            @SdrNumber, @Address, @ZipCode, @PostalAddress,
            @PostalZipCode, @Phone, @Email, @Latitude,
            @Longitude, @ImageURL, @IsActive, @IsListable,
            @AgencyCode, @AgencyInscriptionId, @IsPropietary
        );

        SET @Id = SCOPE_IDENTITY();
        
        -- Actualizamos el AgencyId en AgencyInscription
        UPDATE AgencyInscription
        SET AgencyId = @Id
        WHERE Id = @AgencyInscriptionId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO