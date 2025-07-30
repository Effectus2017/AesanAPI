-- =============================================
-- Stored Procedure: 100_UpdateStaff
-- =============================================
-- Actualiza un miembro del staff existente en la base de datos

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateStaff]
    @id INT,
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
    @isActive BIT = NULL,
    @inactiveJustification NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsAffected INT;

    UPDATE Staff
    SET
        FirstName = @firstName,
        MiddleName = @middleName,
        FatherLastName = @fatherLastName,
        MotherLastName = @motherLastName,
        StatusId = @statusId,
        PositionId = @positionId,
        StaffTypeId = @staffTypeId,
        BirthDate = @birthDate,
        Email = @email,
        PostalAddress = @postalAddress,
        CityId = @cityId,
        RegionId = @regionId,
        AreaCode = @areaCode,
        Comments = @comments,
        UserId = @userId,
        IsActive = ISNULL(@isActive, IsActive),
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END