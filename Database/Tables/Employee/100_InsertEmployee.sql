-- =============================================
-- DEPRECATED: Este stored procedure está deprecado. Use Staff en su lugar.
-- Será eliminado en una versión futura.
-- =============================================
-- Stored Procedure: 100_InsertEmployee
-- =============================================
-- Inserta un nuevo empleado en la base de datos
-- Parámetros: Todos los campos del empleado
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[100_InsertEmployee]
    @firstName NVARCHAR(100),
    @middleName NVARCHAR(100) = NULL,
    @fatherLastName NVARCHAR(100),
    @motherLastName NVARCHAR(100),
    @statusId INT,
    @positionId INT,
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

    INSERT INTO Employee
        (
        FirstName,
        MiddleName,
        FatherLastName,
        MotherLastName,
        StatusId,
        PositionId,
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