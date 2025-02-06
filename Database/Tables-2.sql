-- Ciudades
CREATE TABLE City
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Regiones
CREATE TABLE Region
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Relación entre Ciudad y Región
CREATE TABLE CityRegion
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    CityId INT NOT NULL,
    RegionId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id)
);

-- Estados de la agencia
CREATE TABLE AgencyStatus
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Agencias Auspiciadoras (Sponsoring Agencies)
CREATE TABLE Agency
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    StatusId INT NOT NULL,
    CityRegionId INT NOT NULL,
    PostalCityRegionId INT NOT NULL,
    ProgramId INT NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    SdrNumber INT NOT NULL,
    UieNumber INT NOT NULL,
    EinNumber INT NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    ZipCode INT NOT NULL,
    PostalAddress NVARCHAR(255) NOT NULL,
    PostalZipCode INT NOT NULL,
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    ImageURL NVARCHAR(MAX) NULL,
    NonProfit BIT NULL DEFAULT 0,
    FederalFundsDenied BIT NULL DEFAULT 0,
    StateFundsDenied BIT NULL DEFAULT 0,
    OrganizedAthleticPrograms BIT NULL DEFAULT 0,
    RejectionJustification NVARCHAR(MAX) NULL,
    AppointmentCoordinated BIT NULL DEFAULT 0,
    AppointmentDate DATETIME NULL,
    IsActive BIT NULL DEFAULT 1,
    IsListable BIT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (CityRegionId) REFERENCES CityRegion(Id),
    FOREIGN KEY (PostalCityRegionId) REFERENCES CityRegion(Id),
    FOREIGN KEY (StatusId) REFERENCES AgencyStatus(Id)
);

-- Asignación de programas a agencias
CREATE TABLE AgencyProgram
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AgencyId INT NOT NULL,
    ProgramId INT NOT NULL,
    UserId NVARCHAR(36) NULL,
    Comments NVARCHAR(MAX) NULL,
    AppointmentCoordinated BIT NULL DEFAULT 0,
    AppointmentDate DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

-- Programas de AESAN
CREATE TABLE Program
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Niveles de Educación
CREATE TABLE EducationLevel
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Períodos de funcionamiento
CREATE TABLE OperatingPeriod
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Tipo de Organización
CREATE TABLE OrganizationType
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Escuelas
CREATE TABLE School
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    EducationLevelId INT NOT NULL,
    OperatingPeriodId INT NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    CityRegionId INT NOT NULL,
    ZipCode INT NOT NULL,
    OrganizationTypeId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (EducationLevelId) REFERENCES EducationLevel(Id),
    FOREIGN KEY (OperatingPeriodId) REFERENCES OperatingPeriod(Id),
    FOREIGN KEY (CityRegionId) REFERENCES CityRegion(Id),
    FOREIGN KEY (OrganizationTypeId) REFERENCES OrganizationType(Id)
);

-- Escuelas Satélite
CREATE TABLE SatelliteSchool
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    MainSchoolId INT NOT NULL,
    SatelliteSchoolId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (MainSchoolId) REFERENCES School(Id),
    FOREIGN KEY (SatelliteSchoolId) REFERENCES School(Id)
);

-- Tipos de Comida
CREATE TABLE MealType
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Servicio que solicita la escuela
CREATE TABLE SchoolMeal
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    MealTypeId INT NOT NULL,
    SchoolId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (MealTypeId) REFERENCES MealType(Id),
    FOREIGN KEY (SchoolId) REFERENCES School(Id)
);

-- Facilidades
CREATE TABLE Facility
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Facilidades de la Escuela
CREATE TABLE SchoolFacility
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    SchoolId INT NOT NULL,
    FacilityId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (SchoolId) REFERENCES School(Id),
    FOREIGN KEY (FacilityId) REFERENCES Facility(Id)
);

-- Resto de las tablas mantienen la misma estructura pero agregando los campos de auditoría
-- IsActive BIT NOT NULL DEFAULT 1,
-- CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
-- UpdatedAt DATETIME NULL 

CREATE TABLE FederalFundingCertification
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    FundingAmount DECIMAL(10,2) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE FederalFundingSource
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    DateFrom DATETIME NOT NULL,
    DateTo DATETIME NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE FoodAuthority
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE OperatingPolicy
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Description NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE AlternativeCommunication
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE OptionSelection
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE DocumentsRequired
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE ProgramInscription
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AgencyId INT NOT NULL,
    ProgramId INT NOT NULL,
    ApplicationNumber NVARCHAR(255) NOT NULL,
    IsPublic BIT NOT NULL,
    TotalNumberSchools INT NOT NULL,
    HasBasicEducationCertification BIT NOT NULL,
    IsAeaMenuCreated BIT NOT NULL,
    ExemptionRequirement NVARCHAR(255) NOT NULL,
    ExemptionStatus NVARCHAR(255) NOT NULL,
    ParticipatingAuthorityId INT NOT NULL,
    OperatingPolicyId INT NOT NULL,
    HasCompletedCivilRightsQuestionnaire BIT NOT NULL,
    NeedsInformationInOtherLanguages BIT NOT NULL,
    InformationInOtherLanguages NVARCHAR(MAX) NULL,
    NeedsInterpreter BIT NOT NULL,
    InterpreterLanguages NVARCHAR(MAX) NULL,
    NeedsAlternativeCommunication BIT NOT NULL,
    AlternativeCommunicationId INT NULL,
    NeedsFederalRelayServiceId INT NOT NULL,
    ShowEvidenceId INT NOT NULL,
    ShowEvidenceDescription NVARCHAR(MAX) NULL,
    SnackPercentage DECIMAL(5,2) NULL,
    ReducedSnackPercentage DECIMAL(5,2) NULL,
    FederalFundingCertificationId INT NULL,
    [Date] DATETIME NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id),
    FOREIGN KEY (ParticipatingAuthorityId) REFERENCES FoodAuthority(Id),
    FOREIGN KEY (OperatingPolicyId) REFERENCES OperatingPolicy(Id),
    FOREIGN KEY (AlternativeCommunicationId) REFERENCES AlternativeCommunication(Id),
    FOREIGN KEY (NeedsFederalRelayServiceId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (ShowEvidenceId) REFERENCES OptionSelection(Id),
    FOREIGN KEY (FederalFundingCertificationId) REFERENCES FederalFundingCertification(Id)
);

CREATE TABLE ProgramInscriptionSchool
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProgramInscriptionId INT NOT NULL,
    SchoolId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (ProgramInscriptionId) REFERENCES ProgramInscription(Id),
    FOREIGN KEY (SchoolId) REFERENCES School(Id)
);

CREATE TABLE ProgramInscriptionFederalFundingSource
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProgramInscriptionId INT NOT NULL,
    FederalFundingSourceId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (ProgramInscriptionId) REFERENCES ProgramInscription(Id),
    FOREIGN KEY (FederalFundingSourceId) REFERENCES FederalFundingSource(Id)
);

CREATE TABLE ProgramInscriptionRequiredDocuments
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProgramInscriptionId INT NOT NULL,
    DocumentsRequiredId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (ProgramInscriptionId) REFERENCES ProgramInscription(Id),
    FOREIGN KEY (DocumentsRequiredId) REFERENCES DocumentsRequired(Id)
);

-- Crear tabla AspNetUsers con los campos necesarios
CREATE TABLE AspNetUsers
(
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    AgencyId INT NULL,
    UserName NVARCHAR(256) NULL,
    NormalizedUserName NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    TwoFactorEnabled BIT NOT NULL DEFAULT 0,
    LockoutEnd DATETIMEOFFSET NULL,
    LockoutEnabled BIT NOT NULL DEFAULT 0,
    AccessFailedCount INT NOT NULL DEFAULT 0,
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    FatherLastName NVARCHAR(100) NOT NULL,
    MotherLastName NVARCHAR(100) NOT NULL,
    AdministrationTitle NVARCHAR(100) NOT NULL,
    ImageURL NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsTemporalPasswordActived BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id)
);

CREATE TABLE AspNetRoles
(
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    Name NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE AspNetUserRoles
(
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);

CREATE TABLE AspNetUserClaims
(
    Id INT NOT NULL PRIMARY KEY IDENTITY,
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE AspNetRoleClaims
(
    Id INT NOT NULL PRIMARY KEY IDENTITY,
    RoleId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);

CREATE TABLE AspNetUserLogins
(
    LoginProvider NVARCHAR(128) NOT NULL,
    ProviderKey NVARCHAR(128) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId NVARCHAR(450) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE TemporaryPasswords
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    TemporaryPassword NVARCHAR(256) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE UserProgram
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    ProgramId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

-- Crear índices para mejorar el rendimiento
CREATE INDEX IX_UserProgram_UserId ON UserProgram(UserId);
CREATE INDEX IX_UserProgram_ProgramId ON UserProgram(ProgramId); 