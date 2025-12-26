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
    @sdrNumber BIGINT,
    @uieNumber BIGINT,
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
    @email NVARCHAR(255),
    -- Es recurrente
    @isRecurrent BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT = 0;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Declarar variables para mensajes de error
        DECLARE @uieErrorMsg NVARCHAR(500);
        DECLARE @einErrorMsg NVARCHAR(500);
        DECLARE @sdrErrorMsg NVARCHAR(500);
        
        -- =============================================
        -- Validar duplicados antes de UPDATE
        -- (excluyendo el registro actual)
        -- =============================================
        
        -- Verificar si el UieNumber ya existe en otra agencia
        IF EXISTS (SELECT 1 FROM Agency WHERE UieNumber = @uieNumber AND Id != @id)
        BEGIN
            ROLLBACK TRANSACTION;
            SET @uieErrorMsg = CONCAT('El Identificador Único de Entidad (IUE) ', CAST(@uieNumber AS NVARCHAR(20)), ' ya está registrado en otra agencia.');
            THROW 50001, @uieErrorMsg, 1;
        END
        
        -- Verificar si el EinNumber ya existe en otra agencia
        IF EXISTS (SELECT 1 FROM Agency WHERE EinNumber = @einNumber AND Id != @id)
        BEGIN
            ROLLBACK TRANSACTION;
            SET @einErrorMsg = CONCAT('El Número de Seguro Social Patronal (EIN) ', CAST(@einNumber AS NVARCHAR(20)), ' ya está registrado en otra agencia.');
            THROW 50002, @einErrorMsg, 1;
        END
        
        -- Verificar si el SdrNumber ya existe en otra agencia
        IF EXISTS (SELECT 1 FROM Agency WHERE SdrNumber = @sdrNumber AND Id != @id)
        BEGIN
            ROLLBACK TRANSACTION;
            SET @sdrErrorMsg = CONCAT('El Número de Registro del Departamento de Estado (SDR) ', CAST(@sdrNumber AS NVARCHAR(20)), ' ya está registrado en otra agencia.');
            THROW 50003, @sdrErrorMsg, 1;
        END
        
        -- =============================================
        -- Actualizamos la agencia
        -- =============================================
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
            -- Es recurrente
            IsRecurrent = @isRecurrent,
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