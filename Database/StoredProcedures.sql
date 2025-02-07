

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
        AgencyStatusId = @statusId,
        RejectionJustification = @rejectionJustification,
        UpdatedAt = GETDATE()
    WHERE Id = @agencyId;

    -- Obtiene el número de filas afectadas
    SET @rowsAffected = @@ROWCOUNT;

    -- Retorna el número de filas afectadas directamente
    RETURN @rowsAffected;
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
        Comments = @Comments,
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