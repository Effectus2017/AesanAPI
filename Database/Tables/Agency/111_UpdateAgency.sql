SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para actualizar una agencia con ZipCode y PostalZipCode como NVARCHAR
-- 1.1.1
CREATE OR ALTER PROCEDURE [111_UpdateAgency]
    @id INT,
    @name NVARCHAR(255),
    @agencyStatusId INT,
    -- Datos de la agencia
    @sdrNumber INT,
    @uieNumber INT,
    @einNumber INT,
    -- Dirección fisica
    @address NVARCHAR(255),
    @zipCode NVARCHAR(20),
    @cityId INT,
    @regionId INT,
    @latitude FLOAT,
    @longitude FLOAT,
    -- Dirección postal
    @postalAddress NVARCHAR(255),
    @postalZipCode NVARCHAR(20),
    @postalCityId INT,
    @postalRegionId INT,
    -- Teléfono
    @phone NVARCHAR(20),
    -- Imagen
    @imageURL NVARCHAR(MAX) = NULL,
    -- Datos de contacto
    @email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Actualizamos la agencia
        UPDATE Agency
        SET Name = @name,
            AgencyStatusId = @agencyStatusId,
            -- Datos de la agencia
            SdrNumber = @sdrNumber,
            UieNumber = @uieNumber,
            EinNumber = @einNumber,
            -- Dirección fisica
            Address = @address,
            ZipCode = @zipCode,
            CityId = @cityId,
            RegionId = @regionId,
            Latitude = @latitude,
            Longitude = @longitude,
            -- Dirección postal
            PostalAddress = @postalAddress,
            PostalZipCode = @postalZipCode,
            PostalCityId = @postalCityId,
            PostalRegionId = @postalRegionId,
            -- Teléfono
            Phone = @phone,
            -- Imagen
            ImageURL = @imageURL,
            -- Datos de contacto
            Email = @email,
            -- Auditoría
            UpdatedAt = GETDATE()
        WHERE Id = @id;

        SET @rowsAffected = @@ROWCOUNT; 

        COMMIT TRANSACTION;

        RETURN @rowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO 