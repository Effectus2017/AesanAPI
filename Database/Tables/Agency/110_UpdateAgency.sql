CREATE OR ALTER PROCEDURE [110_UpdateAgency]
    @Id int,
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
    @IsPropietary bit
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Actualizamos la agencia
        UPDATE Agency
        SET Name = @Name,
            AgencyStatusId = @AgencyStatusId,
            CityId = @CityId,
            PostalCityId = @PostalCityId,
            RegionId = @RegionId,
            PostalRegionId = @PostalRegionId,
            UieNumber = @UieNumber,
            EinNumber = @EinNumber,
            SdrNumber = @SdrNumber,
            Address = @Address,
            ZipCode = @ZipCode,
            PostalAddress = @PostalAddress,
            PostalZipCode = @PostalZipCode,
            Phone = @Phone,
            Email = @Email,
            Latitude = @Latitude,
            Longitude = @Longitude,
            ImageURL = @ImageURL,
            IsActive = @IsActive,
            IsListable = @IsListable,
            AgencyCode = @AgencyCode,
            IsPropietary = @IsPropietary,
            UpdatedAt = GETDATE()
        WHERE Id = @Id;

        -- Actualizamos la inscripción
        UPDATE ai
        SET NonProfit = @NonProfit,
            FederalFundsDenied = @FederalFundsDenied,
            StateFundsDenied = @StateFundsDenied,
            OrganizedAthleticPrograms = @OrganizedAthleticPrograms,
            AtRiskService = @AtRiskService,
            BasicEducationRegistry = @BasicEducationRegistry,
            ServiceTime = @ServiceTime,
            TaxExemptionStatus = @TaxExemptionStatus,
            TaxExemptionType = @TaxExemptionType
        FROM AgencyInscription ai
        INNER JOIN Agency a ON a.AgencyInscriptionId = ai.Id
        WHERE a.Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO 