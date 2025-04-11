SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- Procedimiento para actualizar una agencia con ZipCode y PostalZipCode como NVARCHAR
CREATE OR ALTER PROCEDURE [110_UpdateAgency]
    @Id INT,
    @Name NVARCHAR(255),
    @AgencyStatusId INT,
    -- Datos de la agencia
    @SdrNumber INT,
    @UieNumber INT,
    @EinNumber INT,
    -- Dirección fisica
    @Address NVARCHAR(255),
    @ZipCode NVARCHAR(20),
    @CityId INT,
    @RegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    -- Dirección postal
    @PostalAddress NVARCHAR(255),
    @PostalZipCode NVARCHAR(20),
    @PostalCityId INT,
    @PostalRegionId INT,
    -- Teléfono
    @Phone NVARCHAR(20),
    -- Imagen
    @ImageURL NVARCHAR(MAX) = NULL,
    -- Datos de contacto
    @Email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Actualizamos la agencia
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
            -- Auditoría
            UpdatedAt = GETDATE()
        WHERE Id = @Id;

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