/*
===========================================
Procedimientos Almacenados - Módulo de Elegibilidad
===========================================
Este script contiene los procedimientos almacenados necesarios para
la gestión del módulo de elegibilidad para comida escolar.

Versión: 1.0
Fecha: 2024-03-21
*/

-- Procedimiento: Crear Nueva Solicitud
CREATE OR ALTER PROCEDURE [dbo].[SP_CreateFoodAssistanceApplication]
    @SchoolId INT,
    @ApplicationTypeId INT,
    @SchoolYear NVARCHAR(9),
    @StreetAddress NVARCHAR(255),
    @ApartmentNumber NVARCHAR(50) = NULL,
    @CityId INT,
    @RegionId INT,
    @ZipCode NVARCHAR(5),
    @Phone NVARCHAR(50) = NULL,
    @Email NVARCHAR(100) = NULL,
    @CompletedByFirstName NVARCHAR(100),
    @CompletedByMiddleName NVARCHAR(100) = NULL,
    @CompletedByFatherLastName NVARCHAR(100),
    @CompletedByMotherLastName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ApplicationNumber NVARCHAR(50);
    DECLARE @CurrentDate DATE = GETDATE();
    
    -- Generar número de solicitud (Año-Escuela-Secuencial)
    SELECT @ApplicationNumber = CONCAT(
        YEAR(@CurrentDate),
        '-',
        RIGHT('000' + CAST(@SchoolId AS NVARCHAR(3)), 3),
        '-',
        RIGHT('000' + CAST(
            (SELECT ISNULL(MAX(CAST(RIGHT(ApplicationNumber, 3) AS INT)), 0) + 1
             FROM FoodAssistanceApplication
             WHERE SchoolId = @SchoolId
             AND YEAR(CreatedAt) = YEAR(@CurrentDate))
        AS NVARCHAR(3)), 3)
    );

    INSERT INTO FoodAssistanceApplication (
        ApplicationNumber,
        SchoolId,
        ApplicationTypeId,
        SchoolYear,
        StreetAddress,
        ApartmentNumber,
        CityId,
        RegionId,
        ZipCode,
        Phone,
        Email,
        CompletedByFirstName,
        CompletedByMiddleName,
        CompletedByFatherLastName,
        CompletedByMotherLastName,
        CompletedDate
    )
    VALUES (
        @ApplicationNumber,
        @SchoolId,
        @ApplicationTypeId,
        @SchoolYear,
        @StreetAddress,
        @ApartmentNumber,
        @CityId,
        @RegionId,
        @ZipCode,
        @Phone,
        @Email,
        @CompletedByFirstName,
        @CompletedByMiddleName,
        @CompletedByFatherLastName,
        @CompletedByMotherLastName,
        @CurrentDate
    );

    SELECT SCOPE_IDENTITY() AS ApplicationId;
END;
GO

-- Procedimiento: Agregar Miembro del Hogar
CREATE OR ALTER PROCEDURE [dbo].[SP_AddHouseholdMember]
    @ApplicationId INT,
    @FirstName NVARCHAR(100),
    @MiddleName NVARCHAR(100) = NULL,
    @FatherLastName NVARCHAR(100),
    @MotherLastName NVARCHAR(100),
    @IsStudent BIT,
    @SchoolId INT = NULL,
    @Grade NVARCHAR(10) = NULL,
    @IsFoster BIT = 0,
    @IsMigrant BIT = 0,
    @IsHomeless BIT = 0,
    @IsRunaway BIT = 0,
    @EthnicityId INT = NULL,
    @RaceIds NVARCHAR(MAX) = NULL -- Lista de IDs de razas separados por comas
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @MemberId INT;

    -- Insertar miembro del hogar
    INSERT INTO HouseholdMember (
        ApplicationId,
        FirstName,
        MiddleName,
        FatherLastName,
        MotherLastName,
        IsStudent,
        SchoolId,
        Grade,
        IsFoster,
        IsMigrant,
        IsHomeless,
        IsRunaway
    )
    VALUES (
        @ApplicationId,
        @FirstName,
        @MiddleName,
        @FatherLastName,
        @MotherLastName,
        @IsStudent,
        @SchoolId,
        @Grade,
        @IsFoster,
        @IsMigrant,
        @IsHomeless,
        @IsRunaway
    );

    SET @MemberId = SCOPE_IDENTITY();

    -- Agregar etnicidad si se proporciona
    IF @EthnicityId IS NOT NULL
    BEGIN
        INSERT INTO MemberEthnicity (MemberId, EthnicityId)
        VALUES (@MemberId, @EthnicityId);
    END

    -- Agregar razas si se proporcionan
    IF @RaceIds IS NOT NULL
    BEGIN
        INSERT INTO MemberRace (MemberId, RaceId)
        SELECT @MemberId, value
        FROM STRING_SPLIT(@RaceIds, ',');
    END

    SELECT @MemberId AS MemberId;
END;
GO

-- Procedimiento: Agregar Ingreso de Miembro
CREATE OR ALTER PROCEDURE [dbo].[SP_AddMemberIncome]
    @MemberId INT,
    @IncomeTypeId INT,
    @Amount DECIMAL(10,2),
    @FrequencyId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO HouseholdMemberIncome (
        MemberId,
        IncomeTypeId,
        Amount,
        FrequencyId
    )
    VALUES (
        @MemberId,
        @IncomeTypeId,
        @Amount,
        @FrequencyId
    );

    SELECT SCOPE_IDENTITY() AS IncomeId;
END;
GO

-- Procedimiento: Calcular Ingreso Total Anual del Hogar
CREATE OR ALTER PROCEDURE [dbo].[SP_CalculateHouseholdAnnualIncome]
    @ApplicationId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        SUM(hmi.Amount * f.ConversionFactor) AS TotalAnnualIncome,
        COUNT(DISTINCT hm.Id) AS HouseholdSize
    FROM FoodAssistanceApplication a
    JOIN HouseholdMember hm ON hm.ApplicationId = a.Id
    LEFT JOIN HouseholdMemberIncome hmi ON hmi.MemberId = hm.Id
    LEFT JOIN IncomeFrequency f ON f.Id = hmi.FrequencyId
    WHERE a.Id = @ApplicationId
    GROUP BY a.Id;
END;
GO

-- Procedimiento: Revisar Solicitud
CREATE OR ALTER PROCEDURE [dbo].[SP_ReviewApplication]
    @ApplicationId INT,
    @ReviewerId NVARCHAR(450),
    @EligibilityResult NVARCHAR(50),
    @Notes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalAnnualIncome DECIMAL(10,2);
    DECLARE @HouseholdSize INT;
    DECLARE @CurrentDate DATE = GETDATE();
    
    -- Obtener ingreso total y tamaño del hogar
    SELECT 
        @TotalAnnualIncome = TotalAnnualIncome,
        @HouseholdSize = HouseholdSize
    FROM OPENROWSET(
        'SQLNCLI',
        'Server=(local);Trusted_Connection=yes;',
        'EXEC SP_CalculateHouseholdAnnualIncome @ApplicationId'
    );

    -- Insertar revisión
    INSERT INTO ApplicationReview (
        ApplicationId,
        ReviewerId,
        TotalAnnualIncome,
        HouseholdSize,
        EligibilityResult,
        ReviewDate,
        EffectiveDate,
        ExpirationDate,
        Notes
    )
    VALUES (
        @ApplicationId,
        @ReviewerId,
        @TotalAnnualIncome,
        @HouseholdSize,
        @EligibilityResult,
        @CurrentDate,
        @CurrentDate,
        DATEADD(YEAR, 1, @CurrentDate),
        @Notes
    );

    -- Actualizar estado de la solicitud
    UPDATE FoodAssistanceApplication
    SET Status = 'Revisado',
        UpdatedAt = GETDATE()
    WHERE Id = @ApplicationId;

    SELECT SCOPE_IDENTITY() AS ReviewId;
END;
GO 