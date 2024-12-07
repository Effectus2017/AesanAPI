-- Ciudades
CREATE TABLE City
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL
);

-- Regiones
CREATE TABLE Region
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    CityId INT,
    Name VARCHAR(50) NOT NULL,
    FOREIGN KEY (CityId) REFERENCES City(Id)
);

-- Estados de la agencia
CREATE TABLE AgencyStatus
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
);

-- Asignación de programas a agencias
CREATE TABLE AgencyProgram
(
    AgencyId INT NOT NULL,
    ProgramId INT NOT NULL,
    UserId NVARCHAR(36) NULL,
    Comments NVARCHAR(MAX) NULL,
    AppointmentCoordinated BIT NULL DEFAULT 0,
    AppointmentDate DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

-- Agregar las columnas RejectionJustification, AppointmentCoordinated y AppointmentDate a la tabla AgencyProgram
-- ALTER TABLE AgencyProgram ADD RejectionJustification NVARCHAR(MAX) NULL;
-- ALTER TABLE AgencyProgram ADD AppointmentCoordinated BIT NULL DEFAULT 0;
-- ALTER TABLE AgencyProgram ADD AppointmentDate DATETIME NULL;

-- ALTER TABLE AgencyProgram ADD UpdatedAt DATETIME NULL;
-- ALTER TABLE AgencyProgram ADD CreatedAt DATETIME NOT NULL DEFAULT GETDATE();

-- ALTER TABLE AgencyProgram ADD UserId NVARCHAR(450) NULL;

--INSERT INTO AgencyProgram (AgencyId, ProgramId) VALUES (1, 1);
--INSERT INTO AgencyProgram (AgencyId, ProgramId) VALUES (1, 2);

-- Agencias Auspiciadoras (Sponsoring Agencies)
CREATE TABLE Agency
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    StatusId INT NOT NULL,
    CityId INT NOT NULL,
    RegionId INT NOT NULL,
    PostalCityId INT NOT NULL,
    PostalRegionId INT NOT NULL,
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
    RejectionJustification NVARCHAR(MAX) NULL,
    AppointmentCoordinated BIT NULL DEFAULT 0,
    AppointmentDate DATETIME NULL,
    IsActive BIT NULL DEFAULT 1,
    IsListable BIT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id),
    FOREIGN KEY (StatusId) REFERENCES AgencyStatus(Id)
);
GO

-- agregar las columnas AppointmentCoordinated y AppointmentDate a la tabla Agency
ALTER TABLE AgencyProgram ADD AppointmentCoordinated BIT NULL DEFAULT 0;
ALTER TABLE AgencyProgram ADD AppointmentDate DATETIME NULL;

-- Agregar la columna IsActive a la tabla Agency
--ALTER TABLE Agency ADD IsActive BIT NULL DEFAULT 1;
-- Agregar la columna IsListable a la tabla Agency
--ALTER TABLE Agency ADD IsListable BIT NULL DEFAULT 1;

-- Programas de AESAN, agregar un id de registro unico entre programa y agencia
CREATE TABLE Program
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL
);

-- Crear tabla AspNetUsers
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
    -- Datos del usuario
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

ALTER TABLE AspNetUsers
ADD IsTemporalPasswordActived BIT NOT NULL DEFAULT 0;


GO
-- Crear tabla AspNetRoles
CREATE TABLE AspNetRoles
(
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    Name NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL
);

-- Crear tabla AspNetUserRoles
CREATE TABLE AspNetUserRoles
(
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);

-- Crear tabla AspNetUserClaims
CREATE TABLE AspNetUserClaims
(
    Id INT NOT NULL PRIMARY KEY IDENTITY,
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Crear tabla AspNetRoleClaims
CREATE TABLE AspNetRoleClaims
(
    Id INT NOT NULL PRIMARY KEY IDENTITY,
    RoleId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);

-- Crear tabla AspNetUserLogins
CREATE TABLE AspNetUserLogins
(
    LoginProvider NVARCHAR(128) NOT NULL,
    ProviderKey NVARCHAR(128) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Crear tabla para almacenar contraseñas temporales sin restricciones de clave foránea
CREATE TABLE TemporaryPasswords
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    TemporaryPassword NVARCHAR(256) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
);

CREATE TABLE UserProgram
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    ProgramId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

-- Crear ��ndice para mejorar el rendimiento de las búsquedas
CREATE INDEX IX_UserProgram_UserId ON UserProgram(UserId);
CREATE INDEX IX_UserProgram_ProgramId ON UserProgram(ProgramId);

---------------------------------
-- Nuevas tablas para inscripción a programas
---------------------------------

-- Autoridades Escolar de Alimentos
CREATE TABLE FoodAuthority
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    -- Autoridad Escolar de Alimentos del Departamento de Educación / Autoridad Escolar de Alimentos Independiente
    Description NVARCHAR(MAX) NOT NULL,
    -- Autoridad Escolar de Alimentos del Departamento de Educación / Autoridad Escolar de Alimentos Independiente
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

INSERT INTO FoodAuthority
    (Name, Description)
VALUES
    ('Autoridad Escolar de Alimentos del Departamento de Educación', 'La Autoridad Escolar de Alimentos del Departamento de Educación le proveerá los alimentos, materiales de limpieza, menús, adiestramientos al personal del servicio de  alimentos, gas y servicio de fumigación entre otras cosas.  La entidad tiene que proveer los empleados del servicio de alimentos y las facilidades necesarias de la cafetería escolar con todo el equipo mínimo necesario.');
INSERT INTO FoodAuthority
    (Name, Description)
VALUES
    ('Autoridad Escolar de Alimentos Independiente', 'La entidad solicitante será responsable de la administración del Programa de Servicios de Alimentos.  El Departamento de Educación reembolsará a la entidad participante una cantidad de dinero por cada ración servida a los estudiantes que cualifiquen para ello.  Con ese reembolso la entidad adquirirá los alimentos frescos, congelados y enlatados a usarse, pagará a los empleados del servicio de alimentos y adquirirá equipo para el mismo.');

CREATE TABLE OperatingPolicy
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Description NVARCHAR(255) NOT NULL
);

INSERT INTO OperatingPolicy
    (Description)
VALUES
    ('Gratis/Reducido');
INSERT INTO OperatingPolicy
    (Description)
VALUES
    ('Pagando');
INSERT INTO OperatingPolicy
    (Description)
VALUES
    ('Provisión 1');
INSERT INTO OperatingPolicy
    (Description)
VALUES
    ('Provisión 2');
INSERT INTO OperatingPolicy
    (Description)
VALUES
    ('Provisión 3');

CREATE TABLE AlternativeCommunication
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
    -- Nombre del medio alternativo
);

INSERT INTO AlternativeCommunication
    (Name)
VALUES
    ('Sistema Braile');
INSERT INTO AlternativeCommunication
    (Name)
VALUES
    ('Letras Grandes');
INSERT INTO AlternativeCommunication
    (Name)
VALUES
    ('Cinta de Audio');
INSERT INTO AlternativeCommunication
    (Name)
VALUES
    ('Otros');

-- tabla para Sí/No/N/A
CREATE TABLE OptionSelection
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);

INSERT INTO OptionSelection
    (Name)
VALUES
    ('Sí');
INSERT INTO OptionSelection
    (Name)
VALUES
    ('No');
INSERT INTO OptionSelection
    (Name)
VALUES
    ('N/A');

-- Niveles de Educación
CREATE TABLE EducationLevel
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);

INSERT INTO EducationLevel
    (Name)
VALUES
    ('Kinder');
INSERT INTO EducationLevel
    (Name)
VALUES
    ('Elemental');
INSERT INTO EducationLevel
    (Name)
VALUES
    ('Intermedio');
INSERT INTO EducationLevel
    (Name)
VALUES
    ('Superior');

-- Períodos de funcionamiento
CREATE TABLE OperatingPeriod
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
    -- Nombre del período de funcionamiento (e.g., Ago-May, Ago-Jun, Ago-Jul)
);

INSERT INTO OperatingPeriod
    (Name)
VALUES
    ('Ago-May');
INSERT INTO OperatingPeriod
    (Name)
VALUES
    ('Ago-Jun');
INSERT INTO OperatingPeriod
    (Name)
VALUES
    ('Ago-Jul');

CREATE TABLE MealType
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
    -- Nombre del tipo de comida (Almuerzo/Desayuno/Meriendas/Alimentos Federales)
);

INSERT INTO MealType
    (Name)
VALUES
    ('Desayuno');
INSERT INTO MealType
    (Name)
VALUES
    ('Almuerzo');
INSERT INTO MealType
    (Name)
VALUES
    ('Merienda');
INSERT INTO MealType
    (Name)
VALUES
    ('Alimentos Federales');

-- Tipo de Organización	Escuela/Satélite/Institución Residencial/Otros
CREATE TABLE OrganizationType
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);

INSERT INTO OrganizationType
    (Name)
VALUES
    ('Escuela');
INSERT INTO OrganizationType
    (Name)
VALUES
    ('Satélite');
INSERT INTO OrganizationType
    (Name)
VALUES
    ('Institución Residencial');
INSERT INTO OrganizationType
    (Name)
VALUES
    ('Otros');

-- Facilidades	Almacén/Cocina/Salón Comedor
CREATE TABLE Facility
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);

INSERT INTO Facility
    (Name)
VALUES
    ('Almacén');
INSERT INTO Facility
    (Name)
VALUES
    ('Cocina');
INSERT INTO Facility
    (Name)
VALUES
    ('Salón Comedor');

-- Documentos requeridos para solicitar el servicio de merienda:	Lista de Participantes/Plan de Actividades/Menú a Utilizar
CREATE TABLE DocumentsRequired
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);

INSERT INTO DocumentsRequired
    (Name)
VALUES
    ('Lista de Participantes');
INSERT INTO DocumentsRequired
    (Name)
VALUES
    ('Plan de Actividades');
INSERT INTO DocumentsRequired
    (Name)
VALUES
    ('Menú a Utilizar');


-- Escuelas
CREATE TABLE School
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    -- Nombre de la Escuela	Alfa-numérico
    EducationLevelId INT NOT NULL,
    -- Nivel de Educación
    OperatingPeriodId INT NOT NULL,
    -- Período de funcionamiento
    Address NVARCHAR(255) NOT NULL,
    -- Dirección de la Escuela
    CityId INT NOT NULL,
    -- Ciudad
    RegionId INT NOT NULL,
    -- Región
    ZipCode INT NOT NULL,
    -- Código Postal
    OrganizationTypeId INT NOT NULL,
    -- Tipo de Organización
    FOREIGN KEY (EducationLevelId) REFERENCES EducationLevel(Id),
    FOREIGN KEY (OperatingPeriodId) REFERENCES OperatingPeriod(Id),
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id),
    FOREIGN KEY (OrganizationTypeId) REFERENCES OrganizationType(Id)
);

CREATE TABLE SatelliteSchool
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    MainSchoolId INT NOT NULL,
    SatelliteSchoolId INT NOT NULL,
    FOREIGN KEY (MainSchoolId) REFERENCES School(Id),
    FOREIGN KEY (SatelliteSchoolId) REFERENCES School(Id)
);

-- servicio que solicita la escuela
CREATE TABLE SchoolMealRequest
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    MealTypeId INT NOT NULL,
    SchoolId INT NOT NULL,
    FOREIGN KEY (MealTypeId) REFERENCES MealType(Id),
    FOREIGN KEY (SchoolId) REFERENCES School(Id)
);

-- Facilidades de la Escuela
CREATE TABLE SchoolFacility
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    SchoolId INT NOT NULL,
    FacilityId INT NOT NULL,
    FOREIGN KEY (SchoolId) REFERENCES School(Id),
    FOREIGN KEY (FacilityId) REFERENCES Facility(Id)
);

CREATE TABLE FederalFundingCertification
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    FundingAmount DECIMAL(10,2) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    UpdatedAt DATETIME NULL,
);

INSERT INTO FederalFundingCertification
    (FundingAmount, Description, UpdatedAt)
VALUES
    (750000, '$750,000 o más en fondos federales en un año fiscal', GETDATE());
INSERT INTO FederalFundingCertification
    (FundingAmount, Description, UpdatedAt)
VALUES
    (749999, '$749,999 o menos en fondos federales en un año fiscal', GETDATE());

-- Esta tabla permite a AESAN certificar la cantidad de fondos federales recibidos por la institución en un año fiscal.
-- Se debe elegir entre $750,000 o más o $749,999 o menos.

-- tabla para De recibir $750,000.00 o más en fondos federales, favor de detallar las diferentes fuentes que comprenden dichos fondos.	Nombre Completo de la Fuente de Ingreso/Año Desde - Hasta/ Cantidad	Esto deberá ser una tabla con esos encabezados y varias filas para completar
CREATE TABLE FederalFundingSource
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    DateFrom DATETIME NOT NULL,
    DateTo DATETIME NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
);


-- Inscripción a programas
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

-- Asignación de escuelas a programas de inscripción
CREATE TABLE ProgramInscriptionSchool
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProgramInscriptionId INT NOT NULL,
    SchoolId INT NOT NULL,
    FOREIGN KEY (ProgramInscriptionId) REFERENCES ProgramInscription(Id),
    FOREIGN KEY (SchoolId) REFERENCES School(Id)
);

-- nueva tabla para relacionar programInscription con FederalFundingSource
CREATE TABLE ProgramInscriptionFederalFundingSource
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProgramInscriptionId INT NOT NULL,
    FederalFundingSourceId INT NOT NULL,
    FOREIGN KEY (ProgramInscriptionId) REFERENCES ProgramInscription(Id),
    FOREIGN KEY (FederalFundingSourceId) REFERENCES FederalFundingSource(Id)
);

-- Crear tabla para relacionar la inscripción a programas y documentos requeridos
CREATE TABLE ProgramInscriptionRequiredDocuments
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProgramInscriptionId INT NOT NULL,
    DocumentsRequiredId INT NOT NULL,
    FOREIGN KEY (ProgramInscriptionId) REFERENCES ProgramInscription(Id),
    FOREIGN KEY (DocumentsRequiredId) REFERENCES DocumentsRequired(Id)
);