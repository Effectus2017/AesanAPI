-- Procedimiento almacenado para obtener todas las ciudades
CREATE PROCEDURE [100_GetCities]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT
AS
BEGIN
    SELECT *
    FROM City
    WHERE (@alls = 1 OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;


    SELECT COUNT(*) AS TotalCount
    FROM City
    WHERE (@alls = 1 OR Name LIKE '%' + @name + '%');
END;
GO
-- Procedimiento almacenado para obtener todas las regiones
CREATE PROCEDURE [100_GetRegions]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT
AS
BEGIN
    SELECT *
    FROM Region
    WHERE (@alls = 1 OR Name LIKE '%' + @name + '%')
    ORDER BY Name
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM Region
    WHERE (@alls = 1 OR Name LIKE '%' + @name + '%');
END;
GO
-- Procedimiento almacenado para obtener regiones por ID de ciudad
CREATE PROCEDURE [100_GetRegionsByCityId]
    @cityId INT
AS
BEGIN
    SELECT *
    FROM Region
    WHERE CityId = @cityId;

    SELECT COUNT(*) AS TotalCount
    FROM Region
    WHERE CityId = @cityId;
END;
GO

-- Procedimiento almacenado para obtener una región por su ID
CREATE PROCEDURE [100_GetRegionById]
    @regionId INT
AS
BEGIN
    SELECT *
    FROM Region
    WHERE Id = @regionId;
END;
GO

-- Procedimiento almacenado para insertar una agencia
CREATE OR ALTER PROCEDURE [100_InsertAgency]
    @Name NVARCHAR(MAX),
    @StatusId INT,
    -- Datos de la Agencia
    @SdrNumber NVARCHAR(255),
    @UieNumber NVARCHAR(255),
    @EinNumber NVARCHAR(255),
    -- Dirección Física
    @Address NVARCHAR(255),
    @ZipCode INT,
    @CityId INT,
    @RegionId INT,
    @Latitude FLOAT,
    @Longitude FLOAT,
    -- Dirección Postal
    @PostalAddress NVARCHAR(255),
    @PostalZipCode INT,
    @PostalCityId INT,
    @PostalRegionId INT,
    -- Teléfono
    @Phone NVARCHAR(50),
    -- Datos del Contacto
    @Email NVARCHAR(255),
    @NonProfit BIT,
    @FederalFundsDenied BIT,
    @StateFundsDenied BIT,
    @OrganizedAthleticPrograms BIT,
    -- Justificación para rechazo
    @RejectionJustification NVARCHAR(MAX) = NULL,
    -- Imágen - Logo
    @ImageURL NVARCHAR(MAX) = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Agency
        (
        Name,
        StatusId,
        SdrNumber,
        UieNumber,
        EinNumber,
        -- Dirección Física
        Address,
        ZipCode,
        CityId,
        RegionId,
        Latitude,
        Longitude,
        -- Dirección Postal
        PostalAddress,
        PostalZipCode,
        PostalCityId,
        PostalRegionId,
        -- Teléfono
        Phone,
        -- Datos del Contacto
        Email,
        NonProfit,
        FederalFundsDenied,
        StateFundsDenied,
        RejectionJustification,
        ImageURL,
        OrganizedAthleticPrograms
        )
    VALUES
        (
            @Name,
            @StatusId,
            @SdrNumber,
            @UieNumber,
            @EinNumber,
            -- Dirección Física
            @Address,
            @ZipCode,
            @CityId,
            @RegionId,
            @Latitude,
            @Longitude,
            -- Dirección Postal
            @PostalAddress,
            @PostalZipCode,
            @PostalCityId,
            @PostalRegionId,
            -- Teléfono
            @Phone,
            -- Datos del Contacto
            @Email,
            @NonProfit,
            @FederalFundsDenied,
            @StateFundsDenied,
            @RejectionJustification,
            @ImageURL,
            @OrganizedAthleticPrograms
    );

    SET @Id = SCOPE_IDENTITY();
-- Obtener el ID de la agencia insertada
END
GO

-- Procedimiento almacenado para insertar un programa de agencia
CREATE OR ALTER PROCEDURE [100_InsertAgencyProgram]
    @agencyId INT,
    @programId INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AgencyProgram
        (AgencyId, ProgramId)
    VALUES
        (@agencyId, @programId);
END
GO

-- Procedimiento almacenado para obtener todas las agencias
CREATE OR ALTER PROCEDURE [100_GetAgencies]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @programId INT = NULL,
    @statusId INT = NULL
AS
BEGIN
    SELECT
        a.Id,
        a.Name,
        a.StatusId,
        -- Datos de la Agencia
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        -- Dirección y Teléfono
        a.Address,
        a.ZipCode,
        a.PostalAddress,
        a.Phone,
        -- Coordenadas
        a.Latitude,
        a.Longitude,
        -- Joins para las relaciones
        c.Id AS CityId,
        c.Name AS CityName,
        r.Id AS RegionId,
        r.Name AS RegionName,
        s.Id AS StatusId,
        s.Name AS StatusName,
        -- Datos del usuario administrador
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        -- Datos del usuario administrador
        u.AdministrationTitle,
        u.Email,
        -- Auditoría
        a.CreatedAt,
        a.UpdatedAt,
        -- Imágen - Logo
        a.ImageURL,
        -- Justificación para rechazo
        a.RejectionJustification
    FROM Agency a
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN AgencyStatus s ON a.StatusId = s.Id
        LEFT JOIN AspNetUsers u ON u.AgencyId = a.Id
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@statusId IS NULL OR a.StatusId = @statusId)
    )
        AND a.IsListable = 1
    ORDER BY a.Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Obtener programas asociados
    SELECT
        p.Id,
        p.Name,
        p.Description,
        ap.AgencyId
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
    WHERE ap.AgencyId IN (SELECT a.Id
    FROM Agency a
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@statusId IS NULL OR a.StatusId = @statusId)
    ));

    -- Actualizar también la consulta del conteo para incluir los mismos filtros
    SELECT COUNT(*) AS TotalCount
    FROM Agency a
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@statusId IS NULL OR a.StatusId = @statusId)
    )
        AND a.IsListable = 1;
END;
GO

-- Procedimiento almacenado para obtener todas las agencias
CREATE OR ALTER PROCEDURE [101_GetAgencies]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT,
    @regionId INT = NULL,
    @cityId INT = NULL,
    @programId INT = NULL,
    @statusId INT = NULL,
    @userId VARCHAR(36) = NULL
AS
BEGIN
    SELECT DISTINCT
        a.Id,
        a.Name,
        a.StatusId,
        -- Datos de la Agencia
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        -- Dirección y Teléfono
        a.Address,
        a.ZipCode,
        a.PostalAddress,
        a.Phone,
        -- Coordenadas
        a.Latitude,
        a.Longitude,
        -- Joins para las relaciones
        c.Id AS CityId,
        c.Name AS CityName,
        r.Id AS RegionId,
        r.Name AS RegionName,
        s.Id AS StatusId,
        s.Name AS StatusName,
        -- Datos del usuario administrador
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        u.AdministrationTitle,
        u.Email,
        -- Auditoría
        a.CreatedAt,
        a.UpdatedAt,
        -- Imágen - Logo
        a.ImageURL,
        -- Justificación para rechazo
        a.RejectionJustification,
        -- Datos del programa
        ap.Comment,
        ap.AppointmentCoordinated,
        ap.AppointmentDate
    FROM Agency a
        INNER JOIN AgencyProgram ap ON a.Id = ap.AgencyId
        INNER JOIN UserProgram up ON ap.ProgramId = up.ProgramId
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN AgencyStatus s ON a.StatusId = s.Id
        LEFT JOIN AspNetUsers u ON u.AgencyId = a.Id
    WHERE (@alls = 1)
        OR (
            (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@statusId IS NULL OR a.StatusId = @statusId)
        AND (@userId IS NULL OR up.UserId = @userId)
        )
        AND a.IsListable = 1
    ORDER BY a.Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    -- Obtener programas asociados a las agencias filtradas y al usuario
    SELECT DISTINCT
        p.Id,
        p.Name,
        p.Description,
        ap.AgencyId,
        ap.Comment,
        ap.AppointmentCoordinated,
        ap.AppointmentDate,
        -- Usuario asignado al programa
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
        INNER JOIN Agency a ON ap.AgencyId = a.Id
        INNER JOIN UserProgram up ON p.Id = up.ProgramId
        --LEFT JOIN AspNetUsers u ON ap.UserId = u.Id
    WHERE (@alls = 1)
        OR (
            (@programId IS NULL OR ap.ProgramId = @programId)
            AND (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        )
        AND a.IsListable = 1;

    -- Obtener el conteo trotal
    SELECT COUNT(DISTINCT a.Id) AS TotalCount
    FROM Agency a
        LEFT JOIN AgencyProgram ap ON a.Id = ap.AgencyId
        LEFT JOIN UserProgram up ON ap.ProgramId = up.ProgramId
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR a.Name LIKE '%' + @name + '%')
        AND (@regionId IS NULL OR a.RegionId = @regionId)
        AND (@cityId IS NULL OR a.CityId = @cityId)
        AND (@statusId IS NULL OR a.StatusId = @statusId)
        AND (@userId IS NULL OR up.UserId = @userId)
    )
        AND a.IsListable = 1;

END;
GO

-- Procedimiento almacenado para obtener una agencia por su ID
CREATE OR ALTER PROCEDURE [100_GetAgencyById]
    @Id INT
AS
BEGIN
    -- Get agency details
    SELECT
        a.Id,
        a.Name,
        a.StatusId,
        -- Datos de la Agencia
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        -- Dirección y Teléfono
        a.Address,
        a.ZipCode,
        a.PostalAddress,

        -- Coordenadas
        a.Latitude,
        a.Longitude,
        -- Dirección Postal
        a.PostalZipCode,
        a.PostalCityId,
        a.PostalRegionId,
        -- Joins para las relaciones
        c.Id AS CityId,
        c.Name AS CityName,
        r.Id AS RegionId,
        r.Name AS RegionName,
        -- Dirección Postal
        a.Id AS PostalCityId,
        a.Name AS PostalCityName,
        a.Id AS PostalRegionId,
        a.Name AS PostalRegionName,
        -- Estatus
        s.Id AS StatusId,
        s.Name AS StatusName,
        -- Datos del usuario administrador
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        -- Datos del usuario administrador
        u.AdministrationTitle,
        u.Email,
        -- Teléfono
        a.Phone,
        -- Auditoría
        a.CreatedAt,
        a.UpdatedAt,
        -- Imágen - Logo
        a.ImageURL,
        -- Justificación para rechazo
        a.RejectionJustification
    FROM Agency a
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN AgencyStatus s ON a.StatusId = s.Id
        LEFT JOIN AspNetUsers u ON u.AgencyId = a.Id
    WHERE a.Id = @Id;

    -- Get associated programs
    SELECT
        p.Id,
        p.Name,
        p.Description
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
    WHERE ap.AgencyId = @Id;
END;
GO

-- Procedimiento almacenado para obtener todos los estatus de agencia
CREATE PROCEDURE [100_GetAllAgencyStatus]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT
AS
BEGIN
    SELECT Id, Name
    FROM AgencyStatus
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    )
    ORDER BY Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM AgencyStatus
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR Name LIKE '%' + @name + '%')
    );
END;
GO

-- Procedimiento almacenado para actualizar una agencia
CREATE OR ALTER PROCEDURE [100_UpdateAgency]
    @AgencyId INT,
    @Name NVARCHAR(MAX),
    @StatusId INT,
    -- Datos de la Agencia
    @SdrNumber NVARCHAR(255),
    @UieNumber NVARCHAR(255),
    @EinNumber NVARCHAR(255),
    -- Dirección Física
    @Address NVARCHAR(255),
    @ZipCode INT,
    @CityId INT,
    @RegionId INT,
    @Latitude FLOAT = NULL,
    @Longitude FLOAT = NULL,
    -- Dirección Postal
    @PostalAddress NVARCHAR(255),
    @PostalZipCode INT = NULL,
    @PostalCityId INT = NULL,
    @PostalRegionId INT = NULL,
    -- Teléfono
    @Phone NVARCHAR(50),
    -- Datos del Contacto
    @Email NVARCHAR(255),
    -- Imágen - Logo
    @ImageURL NVARCHAR(MAX) = NULL,
    -- Justificación para rechazo
    @RejectionJustification NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Agency
    SET 
        Name = @Name,
        StatusId = @StatusId,
        SdrNumber = @SdrNumber,
        UieNumber = @UieNumber,
        EinNumber = @EinNumber,
        -- Dirección Física
        Address = @Address,
        ZipCode = @ZipCode,
        CityId = @CityId,
        RegionId = @RegionId,
        Latitude = CASE WHEN @Latitude IS NOT NULL THEN @Latitude ELSE Latitude END,
        Longitude = CASE WHEN @Longitude IS NOT NULL THEN @Longitude ELSE Longitude END,
        -- Dirección Postal
        PostalAddress = @PostalAddress,
        PostalZipCode = CASE WHEN @PostalZipCode IS NOT NULL THEN @PostalZipCode ELSE ZipCode END,
        PostalCityId = CASE WHEN @PostalCityId IS NOT NULL THEN @PostalCityId ELSE CityId END,
        PostalRegionId = CASE WHEN @PostalRegionId IS NOT NULL THEN @PostalRegionId ELSE RegionId END,
        -- Teléfono
        Phone = @Phone,
        -- Datos del Contacto
        Email = @Email,
        ImageURL = CASE WHEN @ImageURL IS NOT NULL THEN @ImageURL ELSE ImageURL END,
        -- Justificación para rechazo
        RejectionJustification = CASE WHEN @RejectionJustification IS NOT NULL THEN @RejectionJustification ELSE RejectionJustification END
    WHERE Id = @AgencyId;

    -- Obtiene el número de filas afectadas
    SET @rowsAffected = @@ROWCOUNT;
    -- Retorna el número de filas afectadas directamente
    RETURN @rowsAffected;
END;
GO

-- Procedimiento almacenado para actualizar la URL de la imagen de una agencia
CREATE OR ALTER PROCEDURE [100_UpdateAgencyLogo]
    @AgencyId INT,
    @ImageUrl NVARCHAR(MAX)
-- Nueva URL de la imagen
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Actualizar la URL de la imagen
    UPDATE Agency
    SET 
        ImageURL = @ImageUrl,
        UpdatedAt = GETDATE()
    WHERE Id = @AgencyId;

    -- Obtiene el número de filas afectadas
    SET @rowsAffected = @@ROWCOUNT;

    -- Retorna 1 si se actualizó al menos una fila, 0 si no
    RETURN CASE WHEN @rowsAffected > 0 THEN 1 ELSE 0 END;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateAgencyStatus]
    @agencyId INT,
    @statusId INT,
    @rejectionJustification NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    -- Actualiza el estado de la agencia
    UPDATE Agency
    SET 
        StatusId = @statusId,
        RejectionJustification = @rejectionJustification,
        UpdatedAt = GETDATE()
    WHERE Id = @agencyId;

    -- Obtiene el número de filas afectadas
    SET @rowsAffected = @@ROWCOUNT;

    -- Retorna el número de filas afectadas directamente
    RETURN @rowsAffected;
END;
GO

-- Procedimiento almacenado para eliminar una agencia y sus programas asociados
CREATE PROCEDURE [dbo].[100_DeleteAgency]
    @agencyId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
            
        -- Primero eliminamos los programas asociados a la agencia
        DELETE FROM AgencyProgram 
        WHERE AgencyId = @AgencyId;
            
        -- Luego eliminamos la agencia
        DELETE FROM Agency 
        WHERE Id = @AgencyId;
            
        -- Retornamos el número de filas afectadas de la tabla Agencies
        SELECT @@ROWCOUNT;
            
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Registrar el error y relanzarlo
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;
GO

-- Procedimiento almacenado para obtener todos los programas
CREATE OR ALTER PROCEDURE [100_GetPrograms]
    @take INT,
    @skip INT,
    @name VARCHAR(255),
    @alls BIT
AS
BEGIN
    SELECT Id, Name, Description
    FROM Program
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR Name LIKE '%' + @name + '%')
        AND Name <> 'AESAN'
    )
    ORDER BY Id
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM Program
    WHERE (@alls = 1)
        OR (
        (@name IS NULL OR Name LIKE '%' + @name + '%')
        AND Name <> 'AESAN'
    );
END;
GO

-- Procedimiento almacenado para insertar una contraseña temporal
CREATE OR ALTER PROCEDURE [100_InsertTemporaryPassword]
    @UserId NVARCHAR(450),
    @TemporaryPassword NVARCHAR(256)
AS
BEGIN
    INSERT INTO TemporaryPasswords
        (UserId, TemporaryPassword, CreatedAt)
    VALUES
        (@UserId, @TemporaryPassword, GETDATE());
END;
GO

-- Procedimiento almacenado para obtener una contraseña temporal
CREATE OR ALTER PROCEDURE [100_GetTemporaryPassword]
    @UserId NVARCHAR(450)
AS
BEGIN
    SELECT TOP 1
        TemporaryPassword
    FROM TemporaryPasswords
    WHERE UserId = @UserId
    ORDER BY CreatedAt DESC;
END;
GO

-- Procedimiento almacenado para eliminar una contraseña temporal
CREATE OR ALTER PROCEDURE [100_DeleteTemporaryPassword]
    @UserId NVARCHAR(450)
AS
BEGIN
    DELETE FROM TemporaryPasswords
    WHERE UserId = @UserId;
END;
GO

-- Procedimiento almacenado para obtener una agencia por su ID para la visita preoperacional
CREATE OR ALTER PROCEDURE [dbo].[101_GetAgencyByIdAndUserId]
    @agencyId INT,
    @userId NVARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;

    -- Primera consulta: Obtener los datos de la agencia
    SELECT
        a.*,
        -- Ciudad y Región
        c.Id AS CityId,
        c.Name AS CityName,
        r.Id AS RegionId,
        r.Name AS RegionName,
        -- Ciudad y Región Postal
        pc.Id AS PostalCityId,
        pc.Name AS PostalCityName,
        pr.Id AS PostalRegionId,
        pr.Name AS PostalRegionName,
        -- Estado
        s.Id AS StatusId,
        s.Name AS StatusName,
        -- Usuario
        u.Id AS UserId,
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        u.AdministrationTitle,
        -- Comments
        ap.Comment,
        ap.AppointmentCoordinated AS AppointmentCoordinated,
        ap.AppointmentDate AS AppointmentDate
    FROM Agency a
        -- Joins para Ciudad y Región
        LEFT JOIN City c ON a.CityId = c.Id
        LEFT JOIN Region r ON a.RegionId = r.Id
        -- Joins para Ciudad y Región Postal
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        -- Join para Estado
        LEFT JOIN AgencyStatus s ON a.StatusId = s.Id
        -- Join para Usuario
        LEFT JOIN AspNetUsers u ON a.Id = u.AgencyId
        -- Join para Programas
        LEFT JOIN AgencyProgram ap ON ap.AgencyId = a.id
        LEFT JOIN UserProgram up ON up.ProgramId = ap.ProgramId
    WHERE a.Id = @agencyId AND up.UserId = @userId;

    -- Segunda consulta: Obtener los programas asociados al usuario y la agencia
    SELECT DISTINCT
        p.Id,
        p.Name,
        p.Description,
        ap.AgencyId
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId
        INNER JOIN UserProgram up ON p.Id = up.ProgramId
    WHERE ap.AgencyId = @agencyId AND up.UserId = @userId;

    -- Obtener el usuario que hizo el appointment
    SELECT
        u.Id,
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName
    FROM AspNetUsers u
        INNER JOIN AgencyProgram ap ON ap.UserId = u.Id AND ap.AgencyId = @agencyId;
END;
GO

-- Procedimiento almacenado para actualizar el programa de una agencia
CREATE OR ALTER PROCEDURE [100_UpdateAgencyProgram]
    @AgencyId INT,
    @ProgramId INT,
    @StatusId INT,
    @UserId NVARCHAR(36),
    @Comments NVARCHAR(MAX) = NULL,
    @AppointmentCoordinated BIT = NULL,
    @AppointmentDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE AgencyProgram
    SET 
        ProgramId = @ProgramId,
        UserId = @UserId,
        Comment = @Comments,
        AppointmentCoordinated = @AppointmentCoordinated,
        AppointmentDate = @AppointmentDate,
        UpdatedAt = GETDATE()
    WHERE AgencyId = @AgencyId AND ProgramId = @ProgramId;

    UPDATE Agency
    SET StatusId = @StatusId
    WHERE Id = @AgencyId;

    -- Retornar el número de filas afectadas
    RETURN @@ROWCOUNT;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_InsertProgram]
    @Name NVARCHAR(255),
    @Description NVARCHAR(MAX),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
            
        INSERT INTO Program
        (Name, Description)
    VALUES
        (@Name, @Description);
            
        SET @Id = SCOPE_IDENTITY();
            
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END;
GO

-- Primero eliminamos el tipo si existe
IF EXISTS (SELECT *
FROM sys.types
WHERE name = 'ProgramInscriptionType')
    DROP TYPE ProgramInscriptionType
GO

-- Creamos el tipo de tabla para los parámetros
CREATE TYPE ProgramInscriptionType AS TABLE
(
    AgencyId INT,
    ProgramId INT,
    ApplicationNumber NVARCHAR(255),
    IsPublic BIT,
    TotalNumberSchools INT,
    HasBasicEducationCertification BIT,
    IsAeaMenuCreated BIT,
    ExemptionRequirement NVARCHAR(255),
    ExemptionStatus NVARCHAR(255),
    ParticipatingAuthorityId INT,
    OperatingPolicyId INT,
    HasCompletedCivilRightsQuestionnaire BIT,
    NeedsInformationInOtherLanguages BIT,
    InformationInOtherLanguages NVARCHAR(MAX),
    NeedsInterpreter BIT,
    InterpreterLanguages NVARCHAR(MAX),
    NeedsAlternativeCommunication BIT,
    AlternativeCommunicationId INT,
    NeedsFederalRelayServiceId INT,
    ShowEvidenceId INT,
    ShowEvidenceDescription NVARCHAR(MAX),
    SnackPercentage DECIMAL(5,2),
    ReducedSnackPercentage DECIMAL(5,2),
    FederalFundingCertificationId INT
);
GO

-- Modificamos el stored procedure
CREATE OR ALTER PROCEDURE [dbo].[100_InsertProgramInscription]
    @inscription ProgramInscriptionType READONLY,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
            
        INSERT INTO ProgramInscription
        (
        AgencyId,
        ProgramId,
        ApplicationNumber,
        IsPublic,
        TotalNumberSchools,
        HasBasicEducationCertification,
        IsAeaMenuCreated,
        ExemptionRequirement,
        ExemptionStatus,
        ParticipatingAuthorityId,
        OperatingPolicyId,
        HasCompletedCivilRightsQuestionnaire,
        NeedsInformationInOtherLanguages,
        InformationInOtherLanguages,
        NeedsInterpreter,
        InterpreterLanguages,
        NeedsAlternativeCommunication,
        AlternativeCommunicationId,
        NeedsFederalRelayServiceId,
        ShowEvidenceId,
        ShowEvidenceDescription,
        SnackPercentage,
        ReducedSnackPercentage,
        FederalFundingCertificationId,
        [Date],
        CreatedAt
        )
    SELECT
        AgencyId,
        ProgramId,
        ApplicationNumber,
        IsPublic,
        TotalNumberSchools,
        HasBasicEducationCertification,
        IsAeaMenuCreated,
        ExemptionRequirement,
        ExemptionStatus,
        ParticipatingAuthorityId,
        OperatingPolicyId,
        HasCompletedCivilRightsQuestionnaire,
        NeedsInformationInOtherLanguages,
        InformationInOtherLanguages,
        NeedsInterpreter,
        InterpreterLanguages,
        NeedsAlternativeCommunication,
        AlternativeCommunicationId,
        NeedsFederalRelayServiceId,
        ShowEvidenceId,
        ShowEvidenceDescription,
        SnackPercentage,
        ReducedSnackPercentage,
        FederalFundingCertificationId,
        GETDATE(),
        GETDATE()
    FROM @inscription;
            
        SET @Id = SCOPE_IDENTITY();
            
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_InsertSchool]
    @Name NVARCHAR(255),
    @EducationLevelId INT,
    @OperatingPeriodId INT,
    @Address NVARCHAR(255),
    @CityId INT,
    @RegionId INT,
    @ZipCode INT,
    @OrganizationTypeId INT,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO School
        (
        Name,
        EducationLevelId,
        OperatingPeriodId,
        Address,
        CityId,
        RegionId,
        ZipCode,
        OrganizationTypeId
        )
    VALUES
        (
            @Name,
            @EducationLevelId,
            @OperatingPeriodId,
            @Address,
            @CityId,
            @RegionId,
            @ZipCode,
            @OrganizationTypeId
    );

    SET @Id = SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_InsertFederalFundingSource]
    @InscriptionId INT,
    @Name NVARCHAR(255),
    @DateFrom DATETIME,
    @DateTo DATETIME,
    @Amount DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO FederalFundingSource
        (
        Name,
        DateFrom,
        DateTo,
        Amount
        )
    VALUES
        (
            @Name,
            @DateFrom,
            @DateTo,
            @Amount
    );

    DECLARE @SourceId INT = SCOPE_IDENTITY();

    INSERT INTO ProgramInscriptionFederalFundingSource
        (
        ProgramInscriptionId,
        FederalFundingSourceId
        )
    VALUES
        (
            @InscriptionId,
            @SourceId
    );
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_GetProgramInscriptionById]
    @Id INT
AS
BEGIN
    -- Obtener la inscripción principal
    SELECT
        pi.*,
        -- Agency
        a.Name AS AgencyName,
        -- Program
        p.Name AS ProgramName,
        p.Description AS ProgramDescription,
        -- FoodAuthority
        fa.Name AS FoodAuthorityName,
        fa.Description AS FoodAuthorityDescription,
        -- OperatingPolicy
        op.Description AS OperatingPolicyDescription,
        -- AlternativeCommunication
        ac.Name AS AlternativeCommunicationName,
        -- OptionSelections
        os1.Name AS NeedsFederalRelayServiceName,
        os2.Name AS ShowEvidenceName,
        -- FederalFundingCertification
        ffc.FundingAmount,
        ffc.Description AS FederalFundingDescription
    FROM ProgramInscription pi
        LEFT JOIN Agency a ON pi.AgencyId = a.Id
        LEFT JOIN Program p ON pi.ProgramId = p.Id
        LEFT JOIN FoodAuthority fa ON pi.ParticipatingAuthorityId = fa.Id
        LEFT JOIN OperatingPolicy op ON pi.OperatingPolicyId = op.Id
        LEFT JOIN AlternativeCommunication ac ON pi.AlternativeCommunicationId = ac.Id
        LEFT JOIN OptionSelection os1 ON pi.NeedsFederalRelayServiceId = os1.Id
        LEFT JOIN OptionSelection os2 ON pi.ShowEvidenceId = os2.Id
        LEFT JOIN FederalFundingCertification ffc ON pi.FederalFundingCertificationId = ffc.Id
    WHERE pi.Id = @Id;

    -- Obtener las escuelas asociadas
    SELECT s.*
    FROM School s
        INNER JOIN ProgramInscriptionSchool pis ON s.Id = pis.SchoolId
    WHERE pis.ProgramInscriptionId = @Id;

    -- Obtener las fuentes de fondos federales
    SELECT ffs.*
    FROM FederalFundingSource ffs
        INNER JOIN ProgramInscriptionFederalFundingSource pifs ON ffs.Id = pifs.FederalFundingSourceId
    WHERE pifs.ProgramInscriptionId = @Id;

    -- Obtener los documentos requeridos
    SELECT dr.*
    FROM DocumentsRequired dr
        INNER JOIN ProgramInscriptionRequiredDocuments pird ON dr.Id = pird.DocumentsRequiredId
    WHERE pird.ProgramInscriptionId = @Id;
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[100_GetAllProgramInscriptions]
    @take INT,
    @skip INT,
    @agencyId INT = NULL,
    @programId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener las inscripciones con información relacionada
    WITH
        PaginatedInscriptions
        AS
        (
            SELECT
                pi.*,
                -- Agency
                a.Name AS AgencyName,
                -- Program
                p.Name AS ProgramName,
                p.Description AS ProgramDescription,
                -- FoodAuthority
                fa.Name AS FoodAuthorityName,
                -- OperatingPolicy
                op.Description AS OperatingPolicyDescription,
                -- AlternativeCommunication
                ac.Name AS AlternativeCommunicationName,
                -- OptionSelections
                os1.Name AS NeedsFederalRelayServiceName,
                os2.Name AS ShowEvidenceName,
                -- FederalFundingCertification
                ffc.FundingAmount,
                ffc.Description AS FederalFundingDescription,
                ROW_NUMBER() OVER (ORDER BY pi.CreatedAt DESC) AS RowNum
            FROM ProgramInscription pi
                LEFT JOIN Agency a ON pi.AgencyId = a.Id
                LEFT JOIN Program p ON pi.ProgramId = p.Id
                LEFT JOIN FoodAuthority fa ON pi.ParticipatingAuthorityId = fa.Id
                LEFT JOIN OperatingPolicy op ON pi.OperatingPolicyId = op.Id
                LEFT JOIN AlternativeCommunication ac ON pi.AlternativeCommunicationId = ac.Id
                LEFT JOIN OptionSelection os1 ON pi.NeedsFederalRelayServiceId = os1.Id
                LEFT JOIN OptionSelection os2 ON pi.ShowEvidenceId = os2.Id
                LEFT JOIN FederalFundingCertification ffc ON pi.FederalFundingCertificationId = ffc.Id
            WHERE (@agencyId IS NULL OR pi.AgencyId = @agencyId)
                AND (@programId IS NULL OR pi.ProgramId = @programId)
        )
    SELECT *
    FROM PaginatedInscriptions
    WHERE RowNum > @skip
        AND RowNum <= (@skip + @take)
    ORDER BY CreatedAt DESC;

    -- Obtener el conteo total
    SELECT COUNT(*)
    FROM ProgramInscription pi
    WHERE (@agencyId IS NULL OR pi.AgencyId = @agencyId)
        AND (@programId IS NULL OR pi.ProgramId = @programId);
END;
GO