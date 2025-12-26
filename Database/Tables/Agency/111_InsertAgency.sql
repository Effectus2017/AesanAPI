CREATE OR ALTER PROCEDURE [111_InsertAgency]
    @name nvarchar(255),
    @agencyStatusId int,
    @cityId int,
    @postalCityId int,
    @regionId int,
    @postalRegionId int,
    @uieNumber bigint,
    @einNumber int,
    @sdrNumber bigint,
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
    @isRecurrent bit,
    @id int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Declarar variables para mensajes de error
        DECLARE @uieErrorMsg NVARCHAR(500);
        DECLARE @einErrorMsg NVARCHAR(500);
        DECLARE @sdrErrorMsg NVARCHAR(500);
        
        -- =============================================
        -- Validar duplicados antes de INSERT
        -- =============================================
        
        -- Verificar si el UieNumber ya existe
        IF EXISTS (SELECT 1 FROM Agency WHERE UieNumber = @uieNumber)
        BEGIN
            ROLLBACK TRANSACTION;
            SET @uieErrorMsg = CONCAT('El Identificador Único de Entidad (IUE) ', CAST(@uieNumber AS NVARCHAR(20)), ' ya está registrado en el sistema.');
            THROW 50001, @uieErrorMsg, 1;
        END
        
        -- Verificar si el EinNumber ya existe
        IF EXISTS (SELECT 1 FROM Agency WHERE EinNumber = @einNumber)
        BEGIN
            ROLLBACK TRANSACTION;
            SET @einErrorMsg = CONCAT('El Número de Seguro Social Patronal (EIN) ', CAST(@einNumber AS NVARCHAR(20)), ' ya está registrado en el sistema.');
            THROW 50002, @einErrorMsg, 1;
        END
        
        -- Verificar si el SdrNumber ya existe
        IF EXISTS (SELECT 1 FROM Agency WHERE SdrNumber = @sdrNumber)
        BEGIN
            ROLLBACK TRANSACTION;
            SET @sdrErrorMsg = CONCAT('El Número de Registro del Departamento de Estado (SDR) ', CAST(@sdrNumber AS NVARCHAR(20)), ' ya está registrado en el sistema.');
            THROW 50003, @sdrErrorMsg, 1;
        END
        
        -- =============================================
        -- Insertar solo la agencia
        -- =============================================
        INSERT INTO Agency
        (
        Name, AgencyStatusId, CityId, PostalCityId,
        RegionId, PostalRegionId, UieNumber, EinNumber,
        SdrNumber, Address, ZipCode, PostalAddress,
        PostalZipCode, Phone, Email, Latitude,
        Longitude, ImageURL, IsActive, IsListable,
        AgencyCode, IsPropietary, IsRecurrent
        )
    VALUES
        (
            @name, @agencyStatusId, @cityId, @postalCityId,
            @regionId, @postalRegionId, @uieNumber, @einNumber,
            @sdrNumber, @address, @zipCode, @postalAddress,
            @postalZipCode, @phone, @email, @latitude,
            @longitude, @imageUrl, @isActive, @isListable,
            @agencyCode, @isPropietary, @isRecurrent
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