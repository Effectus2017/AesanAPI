-- =============================================
-- Stored Procedure: 101_InsertStaff
-- =============================================
-- Versión 101: incluye SalaryOriginIds (Origen del Salario, selección múltiple).
-- Parámetros y alias en lowercase; cuerpo con columnas CapitalCase.

CREATE OR ALTER PROCEDURE [dbo].[101_InsertStaff]
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
    @reviewresultid INT = NULL,
    @reviewdate DATETIME = NULL,
    @reviewjustification NVARCHAR(500) = NULL,
    @tenureduration INT = NULL,
    @tenuredurationunitid INT = NULL,
    @receivesprogramsalaryid INT = NULL,
    @salaryoriginids NVARCHAR(50) = NULL,
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
            SalaryOriginIds,
            CreatedAt,
            IsActive
        )
        VALUES
        (
            @firstname,
            @middlename,
            @fatherlastname,
            @motherlastname,
            @statusid,
            @positionid,
            @stafftypeid,
            ISNULL(@staffclassificationid, 1),
            @contractstartdate,
            @contractenddate,
            ISNULL(@birthdate, '2000-01-01'),
            @email,
            @phonenumber,
            @postaladdress,
            @cityid,
            @regionid,
            @zipcode,
            @agencyid,
            @comments,
            @userid,
            @reviewresultid,
            @reviewdate,
            @reviewjustification,
            @tenureduration,
            @tenuredurationunitid,
            @receivesprogramsalaryid,
            @salaryoriginids,
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
        DECLARE @errormessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @errorseverity INT = ERROR_SEVERITY();
        DECLARE @errorstate INT = ERROR_STATE();

        RAISERROR(@errormessage, @errorseverity, @errorstate);
    END CATCH
END
