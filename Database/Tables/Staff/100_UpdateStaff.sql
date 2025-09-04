-- =============================================
-- Stored Procedure: 100_UpdateStaff
-- =============================================
-- Actualiza un miembro del staff existente en la base de datos

CREATE OR ALTER PROCEDURE [dbo].[100_UpdateStaff]
    @id INT,
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
    @areaCode NVARCHAR(10) = NULL,
    @agencyId INT = NULL,
    @comments NVARCHAR(1000) = NULL,
    @userId NVARCHAR(450) = NULL,
    @isActive BIT = NULL,
    @inactiveJustification NVARCHAR(500) = NULL,
    @reviewResultId INT = NULL,
    @reviewDate DATETIME = NULL,
    @reviewJustification NVARCHAR(500) = NULL
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
        StaffClassificationId = @staffClassificationId,
        ContractStartDate = @contractStartDate,
        ContractEndDate = @contractEndDate,
        BirthDate = @birthDate,
        Email = @email,
        PhoneNumber = @phoneNumber,
        PostalAddress = @postalAddress,
        CityId = @cityId,
        RegionId = @regionId,
        AreaCode = @areaCode,
        AgencyId = @agencyId,
        Comments = @comments,
        UserId = @userId,
        IsActive = ISNULL(@isActive, IsActive),
        ReviewResultId = @reviewResultId,
        ReviewDate = @reviewDate,
        ReviewJustification = @reviewJustification,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsAffected = @@ROWCOUNT;
    RETURN @rowsAffected;
END