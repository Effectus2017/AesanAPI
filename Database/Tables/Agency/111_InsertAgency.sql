CREATE OR ALTER PROCEDURE [111_InsertAgency]
    @name nvarchar(255),
    @agencyStatusId int,
    @cityId int,
    @postalCityId int,
    @regionId int,
    @postalRegionId int,
    @uieNumber int,
    @einNumber int,
    @sdrNumber int,
    @address nvarchar(255),
    @zipCode nvarchar(20),
    @postalAddress nvarchar(255),
    @postalZipCode nvarchar(20),
    @phone nvarchar(20),
    @email nvarchar(255),
    @latitude real,
    @longitude real,
    @imageUrl nvarchar(max),
    @isActive bit,
    @isListable bit,
    @agencyCode nvarchar(50),
    @isPropietary bit,
    @id int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Insertar solo la agencia
        INSERT INTO Agency
        (
        Name, AgencyStatusId, CityId, PostalCityId,
        RegionId, PostalRegionId, UieNumber, EinNumber,
        SdrNumber, Address, ZipCode, PostalAddress,
        PostalZipCode, Phone, Email, Latitude,
        Longitude, ImageURL, IsActive, IsListable,
        AgencyCode, IsPropietary
        )
    VALUES
        (
            @name, @agencyStatusId, @cityId, @postalCityId,
            @regionId, @postalRegionId, @uieNumber, @einNumber,
            @sdrNumber, @address, @zipCode, @postalAddress,
            @postalZipCode, @phone, @email, @latitude,
            @longitude, @imageUrl, @isActive, @isListable,
            @agencyCode, @isPropietary
        );

        SET @id = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO 