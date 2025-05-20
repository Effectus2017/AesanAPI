
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

-- =============================================
-- Author:      Claude
-- Create date: 2024-03-21
-- Description: Obtiene la agencia asignada a un usuario por su userId
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[103_GetUserAssignedAgency]
    @userId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el usuario es Agency-Administrator
    DECLARE @isAgencyAdmin BIT = 0;
    SELECT @isAgencyAdmin = 1
    FROM AspNetUserRoles ur
        INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @userId AND r.Name = 'Agency-Administrator';

    IF @isAgencyAdmin = 1
    BEGIN
        -- Para Agency-Administrator, obtener la agencia donde es owner
        SELECT TOP 1
            a.Id,
            a.Name,
            a.Address,
            a.Phone,
            a.Email,
            a.IsActive,
            a.CreatedAt,
            a.UpdatedAt,
            au.IsOwner,
            au.IsMonitor
        FROM [dbo].[Agency] a
            INNER JOIN [dbo].[AgencyUsers] au ON a.Id = au.AgencyId
        WHERE au.UserId = @userId
            AND au.IsOwner = 1
            AND au.IsActive = 1
        ORDER BY au.CreatedAt DESC;
    END
    ELSE
    BEGIN
        -- Para otros usuarios, obtener la agencia principal (donde no es monitor)
        SELECT TOP 1
            a.Id,
            a.Name,
            a.Address,
            a.Phone,
            a.Email,
            a.IsActive,
            a.CreatedAt,
            a.UpdatedAt,
            au.IsOwner,
            au.IsMonitor
        FROM [dbo].[Agency] a
            INNER JOIN [dbo].[AgencyUsers] au ON a.Id = au.AgencyId
        WHERE au.UserId = @userId
            AND au.IsMonitor = 0
            AND au.IsActive = 1
        ORDER BY au.CreatedAt DESC;
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[101_UpdateUserMainAgency]
    @userId NVARCHAR(450),
    @agencyId INT,
    @assignedBy NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @oldAgencyId INT;

    -- Obtener la agencia principal actual
    SELECT @oldAgencyId = AgencyId
    FROM AgencyUsers
    WHERE UserId = @userId AND IsOwner = 1;

    -- Si la agencia es diferente, actualizar
    IF @oldAgencyId != @agencyId
    BEGIN
        -- Desactivar la asignación anterior
        UPDATE AgencyUsers
        SET IsActive = 0,
            UpdatedAt = GETUTCDATE(),
            AssignedBy = @assignedBy
        WHERE UserId = @userId AND AgencyId = @oldAgencyId AND IsOwner = 1;

        -- Crear nueva asignación
        INSERT INTO AgencyUsers
            (
            UserId,
            AgencyId,
            IsOwner,
            IsMonitor,
            IsActive,
            CreatedAt,
            AssignedBy
            )
        VALUES
            (
                @userId,
                @agencyId,
                1, -- IsOwner
                0, -- IsMonitor
                1, -- IsActive
                GETUTCDATE(),
                @assignedBy
        );

        SELECT SCOPE_IDENTITY() AS Id;
    END
    ELSE
    BEGIN
        SELECT @oldAgencyId AS Id;
    END
END
GO

-- Actualizar el SP para obtener una agencia por ID y userId
CREATE OR ALTER PROCEDURE [dbo].[106_GetAgencyByIdAndUserId]
    @Id INT,
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el usuario tiene acceso a la agencia
    IF NOT EXISTS (
        SELECT 1
    FROM AgencyUsers au
    WHERE au.AgencyId = @Id
        AND au.UserId = @UserId
        AND au.IsActive = 1
    )
    BEGIN
        RAISERROR ('El usuario no tiene acceso a esta agencia.', 16, 1);
        RETURN;
    END

    -- Obtener los datos de la agencia
    SELECT
        a.Id,
        a.Name,
        a.AgencyStatusId,
        ast.Name AS AgencyStatusName,
        a.SdrNumber,
        a.UieNumber,
        a.EinNumber,
        a.Address,
        a.ZipCode,
        a.CityId,
        c.Name AS CityName,
        a.RegionId,
        r.Name AS RegionName,
        a.PostalAddress,
        a.PostalZipCode,
        a.PostalCityId,
        pc.Name AS PostalCityName,
        a.PostalRegionId,
        pr.Name AS PostalRegionName,
        a.Latitude,
        a.Longitude,
        a.Phone,
        a.Email,
        a.ImageURL,
        a.NonProfit,
        a.FederalFundsDenied,
        a.StateFundsDenied,
        a.OrganizedAthleticPrograms,
        a.AtRiskService,
        a.ServiceTime,
        a.TaxExemptionStatus,
        a.TaxExemptionType,
        a.RejectionJustification,
        a.Comment,
        a.AppointmentCoordinated,
        a.AppointmentDate,
        a.IsActive,
        a.IsListable,
        a.CreatedAt,
        a.UpdatedAt,
        a.AgencyCode,

        -- Datos del usuario de la agencia (owner)
        u.Id as UserId,
        u.FirstName AS UserFirstName,
        u.MiddleName AS UserMiddleName,
        u.FatherLastName AS UserFatherLastName,
        u.MotherLastName AS UserMotherLastName,
        u.AdministrationTitle AS UserAdministrationTitle,
        u.Email AS UserEmail,
        u.ImageURL AS UserImageURL,

        -- Datos del usuario monitor
        mon.UserId as MonitorId,
        mu.FirstName AS MonitorFirstName,
        mu.MiddleName AS MonitorMiddleName,
        mu.FatherLastName AS MonitorFatherLastName,
        mu.MotherLastName AS MonitorMotherLastName,
        mu.ImageURL AS MonitorImageURL

    FROM Agency a
        INNER JOIN AgencyStatus ast ON a.AgencyStatusId = ast.Id
        INNER JOIN City c ON a.CityId = c.Id
        INNER JOIN Region r ON a.RegionId = r.Id
        LEFT JOIN City pc ON a.PostalCityId = pc.Id
        LEFT JOIN Region pr ON a.PostalRegionId = pr.Id
        -- Obtener el usuario owner
        LEFT JOIN AgencyUsers own ON a.Id = own.AgencyId AND own.IsOwner = 1 AND own.IsActive = 1
        LEFT JOIN AspNetUsers u ON own.UserId = u.Id
        -- Obtener el usuario monitor
        LEFT JOIN AgencyUsers mon ON a.Id = mon.AgencyId AND mon.IsMonitor = 1 AND mon.IsActive = 1
        LEFT JOIN AspNetUsers mu ON mon.UserId = mu.Id
    WHERE a.Id = @Id AND a.IsActive = 1;

    -- Obtener los programas asociados a la agencia
    SELECT
        p.Id,
        p.Name,
        p.Description,
        ap.AgencyId
    FROM Program p
        INNER JOIN AgencyProgram ap ON p.Id = ap.ProgramId AND ap.IsActive = 1
    WHERE ap.AgencyId = @Id;
END
GO

-- Actualizar el SP para obtener todos los usuarios
CREATE OR ALTER PROCEDURE [dbo].[107_GetAllUsersFromDb]
    @take INT,
    @skip INT,
    @name NVARCHAR(255) = NULL,
    @agencyId INT = NULL,
    @roles NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT u.Id,
        u.Email,
        u.FirstName,
        u.MiddleName,
        u.FatherLastName,
        u.MotherLastName,
        u.AdministrationTitle,
        u.PhoneNumber,
        u.ImageURL,
        u.IsActive,
        STRING_AGG(r.Name, ',') AS RoleName
    FROM AspNetUsers u
        LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
        LEFT JOIN AgencyUsers au ON u.Id = au.UserId AND au.IsActive = 1
    WHERE (@agencyId IS NULL OR au.AgencyId = @agencyId)
        AND (@name IS NULL OR u.FirstName LIKE '%' + @name + '%' OR u.FatherLastName LIKE '%' + @name + '%')
        AND (@roles IS NULL OR r.Name IN (SELECT value
        FROM STRING_SPLIT(@roles, ',')))
    GROUP BY u.Id, u.Email, u.FirstName, u.MiddleName, u.FatherLastName, u.MotherLastName, u.AdministrationTitle, u.PhoneNumber, u.ImageURL, u.IsActive
    ORDER BY u.FirstName
    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

    SELECT COUNT(*)
    FROM AspNetUsers u
        LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
        LEFT JOIN AgencyUsers au ON u.Id = au.UserId AND au.IsActive = 1
    WHERE (@agencyId IS NULL OR au.AgencyId = @agencyId)
        AND (@name IS NULL OR u.FirstName LIKE '%' + @name + '%' OR u.FatherLastName LIKE '%' + @name + '%')
        AND (@roles IS NULL OR r.Name IN (SELECT value
        FROM STRING_SPLIT(@roles, ',')));
END
GO