-- =============================================
-- Stored Procedure: 100_InsertStaff
-- =============================================
-- Crea un nuevo miembro del staff en la base de datos

CREATE OR ALTER PROCEDURE [dbo].[100_InsertStaff]
    @firstName NVARCHAR(100),
    @middleName NVARCHAR(100) = NULL,
    @fatherLastName NVARCHAR(100),
    @motherLastName NVARCHAR(100),
    @statusId INT,
    @positionId INT,
    @staffTypeId INT,
    @birthDate DATETIME,
    @email NVARCHAR(255),
    @postalAddress NVARCHAR(500),
    @cityId INT,
    @regionId INT,
    @areaCode NVARCHAR(10),
    @comments NVARCHAR(1000) = NULL,
    @userId NVARCHAR(450) = NULL,
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
        BirthDate,
        Email,
        PostalAddress,
        CityId,
        RegionId,
        AreaCode,
        Comments,
        UserId,
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
            @birthDate,
            @email,
            @postalAddress,
            @cityId,
            @regionId,
            @areaCode,
            @comments,
            @userId,
            GETDATE(),
            1
    );

    SET @id = SCOPE_IDENTITY();
END