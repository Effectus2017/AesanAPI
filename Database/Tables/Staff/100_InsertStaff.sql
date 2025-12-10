-- =============================================
-- Stored Procedure: 100_InsertStaff
-- =============================================
-- Crea un nuevo miembro del staff en la base de datos

CREATE OR ALTER PROCEDURE [dbo].[100_InsertStaff]
    @firstName NVARCHAR(100) = NULL,
    @middleName NVARCHAR(100) = NULL,
    @fatherLastName NVARCHAR(100) = NULL,
    @motherLastName NVARCHAR(100) = NULL,
    @statusId INT,
    @positionId INT,
    @staffTypeId INT,
    @staffClassificationId INT = NULL,
    @contractStartDate DATETIME = NULL,
    @contractEndDate DATETIME = NULL,
    @birthDate DATETIME = NULL,
    @email NVARCHAR(255) = NULL,
    @phoneNumber NVARCHAR(50) = NULL,
    @postalAddress NVARCHAR(500) = NULL,
    @cityId INT = NULL,
    @regionId INT = NULL,
    @zipCode NVARCHAR(10) = NULL,
    @agencyId INT = NULL,
    @comments NVARCHAR(1000) = NULL,
    @userId NVARCHAR(450) = NULL,
    @reviewResultId INT = NULL,
    @reviewDate DATETIME = NULL,
    @reviewJustification NVARCHAR(500) = NULL,
    @tenureDuration INT = NULL,
    @tenureDurationUnitId INT = NULL,
    @receivesProgramSalaryId INT = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Staff
        (
        FirstName,
        MiddleName,
        FatherLastName,
        MotherLastName,
        StatusId,
        PositionId,
        StaffTypeId,
        StaffClassificationId,
        ContractStartDate,
        ContractEndDate,
        BirthDate,
        Email,
        PhoneNumber,
        PostalAddress,
        CityId,
        RegionId,
        ZipCode,
        AgencyId,
        Comments,
        UserId,
        ReviewResultId,
        ReviewDate,
        ReviewJustification,
        TenureDuration,
        TenureDurationUnitId,
        ReceivesProgramSalaryId,
        CreatedAt,
        IsActive
        )
    VALUES
        (
            @firstName,
            @middleName,
            @fatherLastName,
            @motherLastName,
            @statusId,
            @positionId,
            @staffTypeId,
            ISNULL(@staffClassificationId, 1), -- Usar 1 (Administrativo) por defecto si es NULL
            @contractStartDate,
            @contractEndDate,
            ISNULL(@birthDate, '2000-01-01'), -- Fecha por defecto 1/1/2000 si es NULL
            @email,
            @phoneNumber,
            @postalAddress,
            @cityId,
            @regionId,
            @zipCode,
            @agencyId,
            @comments,
            @userId,
            @reviewResultId,
            @reviewDate,
            @reviewJustification,
            @tenureDuration,
            @tenureDurationUnitId,
            @receivesProgramSalaryId,
            GETDATE(),
            1
        );

        SET @id = SCOPE_IDENTITY();
        
        IF @id IS NULL OR @id = 0
        BEGIN
        RAISERROR('Error: No se pudo obtener el ID del staff insertado', 16, 1);
    END
    END TRY
    BEGIN CATCH
        SET @id = 0;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END