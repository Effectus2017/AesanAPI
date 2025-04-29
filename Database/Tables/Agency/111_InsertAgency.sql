CREATE OR ALTER PROCEDURE [111_InsertAgency]
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
    @IsPropietary bit,
    @Id int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Insertar solo la agencia
        INSERT INTO Agency (
            Name, AgencyStatusId, CityId, PostalCityId,
            RegionId, PostalRegionId, UieNumber, EinNumber,
            SdrNumber, Address, ZipCode, PostalAddress,
            PostalZipCode, Phone, Email, Latitude,
            Longitude, ImageURL, IsActive, IsListable,
            AgencyCode, IsPropietary
        )
        VALUES (
            @Name, @AgencyStatusId, @CityId, @PostalCityId,
            @RegionId, @PostalRegionId, @UieNumber, @EinNumber,
            @SdrNumber, @Address, @ZipCode, @PostalAddress,
            @PostalZipCode, @Phone, @Email, @Latitude,
            @Longitude, @ImageURL, @IsActive, @IsListable,
            @AgencyCode, @IsPropietary
        );

        SET @Id = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO 