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
    @birthDate DATETIME = NULL,
    @email NVARCHAR(255) = NULL,
    @postalAddress NVARCHAR(500) = NULL,
    @cityId INT = NULL,
    @regionId INT = NULL,
    @areaCode NVARCHAR(10) = NULL,
    @comments NVARCHAR(1000) = NULL,
    @userId NVARCHAR(450) = NULL,
    @reviewResultId INT = NULL,
    @reviewDate DATETIME = NULL,
    @reviewJustification NVARCHAR(500) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

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
        BirthDate,
        Email,
        PostalAddress,
        CityId,
        RegionId,
        AreaCode,
        Comments,
        UserId,
        ReviewResultId,
        ReviewDate,
        ReviewJustification,
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
            @staffClassificationId,
            @birthDate,
            @email,
            @postalAddress,
            @cityId,
            @regionId,
            @areaCode,
            @comments,
            @userId,
            @reviewResultId,
            @reviewDate,
            @reviewJustification,
            GETDATE(),
            1
    );

    SET @id = SCOPE_IDENTITY();
END