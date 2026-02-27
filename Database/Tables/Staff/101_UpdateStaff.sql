-- =============================================
-- Stored Procedure: 101_UpdateStaff
-- =============================================
-- Versión 102: Origen del Salario en tabla StaffSalaryOrigin (SP 100_UpdateStaffSalaryOrigin).
-- Parámetros lowercase; cuerpo con columnas CapitalCase.

CREATE OR ALTER PROCEDURE [dbo].[101_UpdateStaff]
    @id INT,
    @firstname NVARCHAR(100) = NULL,
    @middlename NVARCHAR(100) = NULL,
    @fatherlastname NVARCHAR(100) = NULL,
    @motherlastname NVARCHAR(100) = NULL,
    @statusid INT,
    @positionid INT,
    @stafftypeid INT,
    @staffclassificationid INT = NULL,
    @contractstartdate DATETIME = NULL,
    @contractenddate DATETIME = NULL,
    @birthdate DATETIME = NULL,
    @email NVARCHAR(255) = NULL,
    @phonenumber NVARCHAR(50) = NULL,
    @postaladdress NVARCHAR(500) = NULL,
    @cityid INT = NULL,
    @regionid INT = NULL,
    @zipcode NVARCHAR(10) = NULL,
    @agencyid INT = NULL,
    @comments NVARCHAR(1000) = NULL,
    @userid NVARCHAR(450) = NULL,
    @isactive BIT = NULL,
    @inactivejustification NVARCHAR(500) = NULL,
    @reviewresultid INT = NULL,
    @reviewdate DATETIME = NULL,
    @reviewjustification NVARCHAR(500) = NULL,
    @tenureduration INT = NULL,
    @tenuredurationunitid INT = NULL,
    @receivesprogramsalaryid INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @rowsaffected INT;

    UPDATE Staff
    SET
        FirstName = @firstname,
        MiddleName = @middlename,
        FatherLastName = @fatherlastname,
        MotherLastName = @motherlastname,
        StatusId = @statusid,
        PositionId = @positionid,
        StaffTypeId = @stafftypeid,
        StaffClassificationId = @staffclassificationid,
        ContractStartDate = @contractstartdate,
        ContractEndDate = @contractenddate,
        BirthDate = @birthdate,
        Email = @email,
        PhoneNumber = @phonenumber,
        PostalAddress = @postaladdress,
        CityId = @cityid,
        RegionId = @regionid,
        ZipCode = @zipcode,
        AgencyId = @agencyid,
        Comments = @comments,
        UserId = ISNULL(@userid, UserId),
        IsActive = ISNULL(@isactive, IsActive),
        ReviewResultId = @reviewresultid,
        ReviewDate = @reviewdate,
        ReviewJustification = @reviewjustification,
        TenureDuration = @tenureduration,
        TenureDurationUnitId = @tenuredurationunitid,
        ReceivesProgramSalaryId = @receivesprogramsalaryid,
        UpdatedAt = GETDATE()
    WHERE Id = @id;

    SET @rowsaffected = @@ROWCOUNT;
    RETURN @rowsaffected;
END
