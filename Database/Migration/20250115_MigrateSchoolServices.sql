-- =============================================
-- Script de Migración: School Services Refactoring
-- Descripción: Migra campos de servicios de School a tablas separadas
-- Fecha: 2025-01-15
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY
    PRINT 'Iniciando migración de servicios de School...';
    
    -- 1. Crear las nuevas tablas
    PRINT 'Creando tabla SchoolService...';
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolService')
    BEGIN
    CREATE TABLE SchoolService
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        SchoolId INT NOT NULL,
        ChildGroupId INT NULL,
        Breakfast BIT NULL,
        BreakfastFrom TIME NULL,
        BreakfastTo TIME NULL,
        Lunch BIT NULL,
        LunchFrom TIME NULL,
        LunchTo TIME NULL,
        SnackAM BIT NULL,
        SnackAMFrom TIME NULL,
        SnackAMTo TIME NULL,
        Dinner BIT NULL,
        DinnerFrom TIME NULL,
        DinnerTo TIME NULL,
        SnackPM BIT NULL,
        SnackPMFrom TIME NULL,
        SnackPMTo TIME NULL,
        SnackNight BIT NULL,
        SnackNightFrom TIME NULL,
        SnackNightTo TIME NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NULL,

        CONSTRAINT FK_SchoolService_SchoolId FOREIGN KEY (SchoolId) REFERENCES School(Id) ON DELETE CASCADE,
        CONSTRAINT FK_SchoolService_ChildGroupId FOREIGN KEY (ChildGroupId) REFERENCES ChildGroup(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_SchoolService_SchoolId ON SchoolService(SchoolId);
    CREATE INDEX IX_SchoolService_ChildGroupId ON SchoolService(ChildGroupId);
    CREATE INDEX IX_SchoolService_SchoolId_ChildGroupId ON SchoolService(SchoolId, ChildGroupId);

    PRINT 'Tabla SchoolService creada exitosamente.';
END
    ELSE
    BEGIN
    PRINT 'Tabla SchoolService ya existe.';
END
    
    PRINT 'Creando tabla SchoolDayCareHome...';
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolDayCareHome')
    BEGIN
    CREATE TABLE SchoolDayCareHome
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        SchoolId INT NOT NULL,
        IsAuthorizedToOperate BIT NULL,
        HasFamilyDepartmentLicense BIT NULL,
        NumberOfEnrolledChildren INT NULL,
        NumberOfProviderChildren INT NULL,
        NumberOfParticipantsWithBloodTies INT NULL,
        NumberOfParticipantsWithoutBloodTies INT NULL,
        MinorsLiveWithProvider BIT NULL,
        RelationshipTypeId INT NULL,
        OffersServiceToImmigrantChildren BIT NULL,
        HomeTypeId INT NULL,
        AdministratorAuthorizedName NVARCHAR(255) NULL,
        AdministratorBirthDate DATE NULL,
        OffersServiceToDifferentGroups BIT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NULL,

        CONSTRAINT FK_SchoolDayCareHome_SchoolId FOREIGN KEY (SchoolId) REFERENCES School(Id) ON DELETE CASCADE,
        CONSTRAINT FK_SchoolDayCareHome_RelationshipTypeId FOREIGN KEY (RelationshipTypeId) REFERENCES OptionSelection(Id),
        CONSTRAINT FK_SchoolDayCareHome_HomeTypeId FOREIGN KEY (HomeTypeId) REFERENCES OptionSelection(Id),
        CONSTRAINT UK_SchoolDayCareHome_SchoolId UNIQUE (SchoolId)
    );

    CREATE INDEX IX_SchoolDayCareHome_SchoolId ON SchoolDayCareHome(SchoolId);
    CREATE INDEX IX_SchoolDayCareHome_RelationshipTypeId ON SchoolDayCareHome(RelationshipTypeId);
    CREATE INDEX IX_SchoolDayCareHome_HomeTypeId ON SchoolDayCareHome(HomeTypeId);

    PRINT 'Tabla SchoolDayCareHome creada exitosamente.';
END
    ELSE
    BEGIN
    PRINT 'Tabla SchoolDayCareHome ya existe.';
END
    
    PRINT 'Creando tabla SchoolParticipant...';
    IF NOT EXISTS (SELECT *
FROM sys.tables
WHERE name = 'SchoolParticipant')
    BEGIN
    CREATE TABLE SchoolParticipant
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        SchoolId INT NOT NULL,
        ParticipantTypeId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NULL,

        CONSTRAINT UK_SchoolParticipant_SchoolId_ParticipantTypeId UNIQUE (SchoolId, ParticipantTypeId),
        CONSTRAINT FK_SchoolParticipant_SchoolId FOREIGN KEY (SchoolId) REFERENCES School(Id) ON DELETE CASCADE,
        CONSTRAINT FK_SchoolParticipant_ParticipantTypeId FOREIGN KEY (ParticipantTypeId) REFERENCES OptionSelection(Id)
    );

    CREATE INDEX IX_SchoolParticipant_SchoolId ON SchoolParticipant(SchoolId);
    CREATE INDEX IX_SchoolParticipant_ParticipantTypeId ON SchoolParticipant(ParticipantTypeId);
    CREATE INDEX IX_SchoolParticipant_IsActive ON SchoolParticipant(IsActive);

    PRINT 'Tabla SchoolParticipant creada exitosamente.';
END
    ELSE
    BEGIN
    PRINT 'Tabla SchoolParticipant ya existe.';
END
    
    -- 2. Migrar datos existentes de School a SchoolService
    PRINT 'Migrando datos de servicios existentes...';
    INSERT INTO SchoolService
    (
    SchoolId, ChildGroupId,
    Breakfast, BreakfastFrom, BreakfastTo,
    Lunch, LunchFrom, LunchTo,
    SnackAM, SnackAMFrom, SnackAMTo,
    Dinner, DinnerFrom, DinnerTo,
    SnackPM, SnackPMFrom, SnackPMTo,
    SnackNight, SnackNightFrom, SnackNightTo,
    CreatedAt, UpdatedAt
    )
SELECT
    Id, NULL, -- ChildGroupId = NULL para servicios generales
    Breakfast, BreakfastFrom, BreakfastTo,
    Lunch, LunchFrom, LunchTo,
    Snack, SnackFrom, SnackTo, -- Migrar Snack a SnackAM
    Dinner, DinnerFrom, DinnerTo,
    NULL, NULL, NULL, -- SnackPM será NULL inicialmente
    SnackNight, SnackNightFrom, SnackNightTo,
    CreatedAt, UpdatedAt
FROM School
WHERE Breakfast IS NOT NULL
    OR Lunch IS NOT NULL
    OR Snack IS NOT NULL
    OR Dinner IS NOT NULL
    OR SnackNight IS NOT NULL;
    
    DECLARE @MigratedServices INT = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@MigratedServices AS VARCHAR(10)) + ' registros de servicios.';
    
    -- 3. Migrar datos específicos de Day Care Homes
    PRINT 'Migrando datos de Day Care Homes...';
    INSERT INTO SchoolDayCareHome
    (
    SchoolId, IsAuthorizedToOperate, HasFamilyDepartmentLicense,
    NumberOfEnrolledChildren, NumberOfProviderChildren,
    NumberOfParticipantsWithBloodTies, NumberOfParticipantsWithoutBloodTies,
    MinorsLiveWithProvider, RelationshipTypeId, OffersServiceToImmigrantChildren,
    HomeTypeId, AdministratorAuthorizedName, AdministratorBirthDate,
    OffersServiceToDifferentGroups, CreatedAt, UpdatedAt
    )
SELECT
    s.Id, s.IsAuthorizedToOperate, s.HasFamilyDepartmentLicense,
    s.NumberOfEnrolledChildren, s.NumberOfProviderChildren,
    s.NumberOfParticipantsWithBloodTies, s.NumberOfParticipantsWithoutBloodTies,
    s.MinorsLiveWithProvider, s.RelationshipTypeId, s.OffersServiceToImmigrantChildren,
    s.HomeTypeId, s.AdministratorAuthorizedName, s.AdministratorBirthDate,
    s.OffersServiceToDifferentGroups, s.CreatedAt, s.UpdatedAt
FROM School s
    INNER JOIN Agency a ON s.AgencyId = a.Id
    INNER JOIN AgencyInscription ai ON a.Id = ai.AgencyId
WHERE ai.IsDayCareHome = 1;
    
    DECLARE @MigratedDayCareHomes INT = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@MigratedDayCareHomes AS VARCHAR(10)) + ' registros de Day Care Homes.';
    
    -- 4. Migrar ParticipantTypeIds (si existe el campo)
    PRINT 'Migrando tipos de participantes...';
    IF EXISTS (SELECT *
FROM sys.columns
WHERE object_id = OBJECT_ID('School') AND name = 'ParticipantTypeIds')
    BEGIN
    INSERT INTO SchoolParticipant
        (SchoolId, ParticipantTypeId, CreatedAt, UpdatedAt)
    SELECT DISTINCT
        s.Id,
        CAST(value AS INT) as ParticipantTypeId,
        s.CreatedAt,
        s.UpdatedAt
    FROM School s
        CROSS APPLY STRING_SPLIT(s.ParticipantTypeIds, ',')
    WHERE s.ParticipantTypeIds IS NOT NULL
        AND s.ParticipantTypeIds != ''
        AND value != '';

    DECLARE @MigratedParticipants INT = @@ROWCOUNT;
    PRINT 'Migrados ' + CAST(@MigratedParticipants AS VARCHAR(10)) + ' registros de tipos de participantes.';
END
    ELSE
    BEGIN
    PRINT 'Campo ParticipantTypeIds no existe en School.';
END
    
    -- 5. Eliminar campos de servicios de School
    PRINT 'Eliminando campos de servicios de School...';
    
    -- Verificar que las migraciones fueron exitosas antes de eliminar campos
    IF EXISTS (SELECT 1
FROM SchoolService)
    BEGIN
    -- Eliminar campos de servicios
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'Breakfast')
            ALTER TABLE School DROP COLUMN Breakfast;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'BreakfastFrom')
            ALTER TABLE School DROP COLUMN BreakfastFrom;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'BreakfastTo')
            ALTER TABLE School DROP COLUMN BreakfastTo;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'Lunch')
            ALTER TABLE School DROP COLUMN Lunch;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'LunchFrom')
            ALTER TABLE School DROP COLUMN LunchFrom;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'LunchTo')
            ALTER TABLE School DROP COLUMN LunchTo;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'Snack')
            ALTER TABLE School DROP COLUMN Snack;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'SnackFrom')
            ALTER TABLE School DROP COLUMN SnackFrom;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'SnackTo')
            ALTER TABLE School DROP COLUMN SnackTo;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'Dinner')
            ALTER TABLE School DROP COLUMN Dinner;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'DinnerFrom')
            ALTER TABLE School DROP COLUMN DinnerFrom;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'DinnerTo')
            ALTER TABLE School DROP COLUMN DinnerTo;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'SnackNight')
            ALTER TABLE School DROP COLUMN SnackNight;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'SnackNightFrom')
            ALTER TABLE School DROP COLUMN SnackNightFrom;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'SnackNightTo')
            ALTER TABLE School DROP COLUMN SnackNightTo;

    -- Eliminar campos específicos de Day Care Home
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'IsAuthorizedToOperate')
            ALTER TABLE School DROP COLUMN IsAuthorizedToOperate;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'HasFamilyDepartmentLicense')
            ALTER TABLE School DROP COLUMN HasFamilyDepartmentLicense;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'NumberOfEnrolledChildren')
            ALTER TABLE School DROP COLUMN NumberOfEnrolledChildren;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'NumberOfProviderChildren')
            ALTER TABLE School DROP COLUMN NumberOfProviderChildren;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'NumberOfParticipantsWithBloodTies')
            ALTER TABLE School DROP COLUMN NumberOfParticipantsWithBloodTies;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'NumberOfParticipantsWithoutBloodTies')
            ALTER TABLE School DROP COLUMN NumberOfParticipantsWithoutBloodTies;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'MinorsLiveWithProvider')
            ALTER TABLE School DROP COLUMN MinorsLiveWithProvider;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'RelationshipTypeId')
            ALTER TABLE School DROP COLUMN RelationshipTypeId;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'OffersServiceToImmigrantChildren')
            ALTER TABLE School DROP COLUMN OffersServiceToImmigrantChildren;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'HomeTypeId')
            ALTER TABLE School DROP COLUMN HomeTypeId;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'AdministratorAuthorizedName')
            ALTER TABLE School DROP COLUMN AdministratorAuthorizedName;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'AdministratorBirthDate')
            ALTER TABLE School DROP COLUMN AdministratorBirthDate;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'OffersServiceToDifferentGroups')
            ALTER TABLE School DROP COLUMN OffersServiceToDifferentGroups;
    IF EXISTS (SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID('School') AND name = 'ParticipantTypeIds')
            ALTER TABLE School DROP COLUMN ParticipantTypeIds;

    PRINT 'Campos de servicios eliminados de School exitosamente.';
END
    ELSE
    BEGIN
    PRINT 'ERROR: No se encontraron datos migrados. Abortando eliminación de campos.';
    ROLLBACK TRANSACTION;
    RETURN;
END
    
    -- 6. Crear stored procedures para manejar las nuevas tablas
    PRINT 'Creando stored procedures...';
    
    -- Stored procedure para insertar/actualizar servicios de escuela
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = 'sp_InsertOrUpdateSchoolService')
    BEGIN
    EXEC('
        CREATE PROCEDURE sp_InsertOrUpdateSchoolService
            @SchoolId INT,
            @ChildGroupId INT = NULL,
            @Breakfast BIT = NULL,
            @BreakfastFrom TIME = NULL,
            @BreakfastTo TIME = NULL,
            @Lunch BIT = NULL,
            @LunchFrom TIME = NULL,
            @LunchTo TIME = NULL,
            @SnackAM BIT = NULL,
            @SnackAMFrom TIME = NULL,
            @SnackAMTo TIME = NULL,
            @Dinner BIT = NULL,
            @DinnerFrom TIME = NULL,
            @DinnerTo TIME = NULL,
            @SnackPM BIT = NULL,
            @SnackPMFrom TIME = NULL,
            @SnackPMTo TIME = NULL,
            @SnackNight BIT = NULL,
            @SnackNightFrom TIME = NULL,
            @SnackNightTo TIME = NULL
        AS
        BEGIN
            SET NOCOUNT ON;
            
            IF EXISTS (SELECT 1 FROM SchoolService WHERE SchoolId = @SchoolId AND ChildGroupId = @ChildGroupId)
            BEGIN
                UPDATE SchoolService SET
                    Breakfast = @Breakfast,
                    BreakfastFrom = @BreakfastFrom,
                    BreakfastTo = @BreakfastTo,
                    Lunch = @Lunch,
                    LunchFrom = @LunchFrom,
                    LunchTo = @LunchTo,
                    SnackAM = @SnackAM,
                    SnackAMFrom = @SnackAMFrom,
                    SnackAMTo = @SnackAMTo,
                    Dinner = @Dinner,
                    DinnerFrom = @DinnerFrom,
                    DinnerTo = @DinnerTo,
                    SnackPM = @SnackPM,
                    SnackPMFrom = @SnackPMFrom,
                    SnackPMTo = @SnackPMTo,
                    SnackNight = @SnackNight,
                    SnackNightFrom = @SnackNightFrom,
                    SnackNightTo = @SnackNightTo,
                    UpdatedAt = GETDATE()
                WHERE SchoolId = @SchoolId AND ChildGroupId = @ChildGroupId;
            END
            ELSE
            BEGIN
                INSERT INTO SchoolService (
                    SchoolId, ChildGroupId, Breakfast, BreakfastFrom, BreakfastTo,
                    Lunch, LunchFrom, LunchTo, SnackAM, SnackAMFrom, SnackAMTo,
                    Dinner, DinnerFrom, DinnerTo, SnackPM, SnackPMFrom, SnackPMTo,
                    SnackNight, SnackNightFrom, SnackNightTo
                ) VALUES (
                    @SchoolId, @ChildGroupId, @Breakfast, @BreakfastFrom, @BreakfastTo,
                    @Lunch, @LunchFrom, @LunchTo, @SnackAM, @SnackAMFrom, @SnackAMTo,
                    @Dinner, @DinnerFrom, @DinnerTo, @SnackPM, @SnackPMFrom, @SnackPMTo,
                    @SnackNight, @SnackNightFrom, @SnackNightTo
                );
            END
        END
        ');
    PRINT 'Stored procedure sp_InsertOrUpdateSchoolService creado.';
END
    
    -- Stored procedure para manejar tipos de participantes
    IF NOT EXISTS (SELECT *
FROM sys.procedures
WHERE name = 'sp_UpdateSchoolParticipants')
    BEGIN
    EXEC('
        CREATE PROCEDURE sp_UpdateSchoolParticipants
            @SchoolId INT,
            @ParticipantTypeIds NVARCHAR(MAX)
        AS
        BEGIN
            SET NOCOUNT ON;
            
            -- Eliminar participantes existentes
            DELETE FROM SchoolParticipant WHERE SchoolId = @SchoolId;
            
            -- Insertar nuevos participantes
            IF @ParticipantTypeIds IS NOT NULL AND @ParticipantTypeIds != ''''
            BEGIN
                INSERT INTO SchoolParticipant (SchoolId, ParticipantTypeId)
                SELECT @SchoolId, CAST(value AS INT)
                FROM STRING_SPLIT(@ParticipantTypeIds, '','')
                WHERE value != '''' AND ISNUMERIC(value) = 1;
            END
        END
        ');
    PRINT 'Stored procedure sp_UpdateSchoolParticipants creado.';
END
    
    PRINT 'Migración completada exitosamente.';
    COMMIT TRANSACTION;
    
END TRY
BEGIN CATCH
    PRINT 'ERROR durante la migración: ' + ERROR_MESSAGE();
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;
