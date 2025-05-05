/*
===========================================
Módulo de Elegibilidad para Comida Escolar
===========================================
Este módulo gestiona las solicitudes de asistencia alimentaria escolar,
incluyendo la información de los miembros del hogar, sus ingresos,
participación en programas de asistencia y revisión de elegibilidad.

Versión: 1.0
Fecha: 2024-03-21

Descripción de las Tablas:
-------------------------
Este módulo contiene las siguientes tablas principales:

1. ApplicationType: Catálogo de tipos de solicitudes
2. FoodAssistanceApplication: Tabla principal de solicitudes
3. HouseholdMember: Miembros del hogar incluidos en la solicitud
4. ApplicationProgramParticipation: Participación en programas de asistencia
5. IncomeType: Catálogo de tipos de ingresos
6. IncomeFrequency: Catálogo de frecuencias de ingreso
7. HouseholdMemberIncome: Registro de ingresos por miembro
8. ApplicationReview: Revisiones y decisiones de elegibilidad
9. Ethnicity y Race: Catálogos de grupos étnicos y raciales
10. MemberEthnicity y MemberRace: Relaciones entre miembros y sus características demográficas

Nota: Para la gestión de documentos se utiliza la tabla existente AgencyFiles
*/

-- =============================================
-- Tabla: ApplicationType (Tipo de Solicitud)
-- =============================================
-- Almacena los diferentes tipos de solicitudes que pueden presentarse.
-- Esta tabla es un catálogo que define los tipos válidos de solicitudes
-- que el sistema puede procesar.
CREATE TABLE ApplicationType
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    Name NVARCHAR(255) NOT NULL,                -- Nombre del tipo de solicitud (ej: Regular, Categórica, Directa)
    Description NVARCHAR(MAX) NULL,             -- Descripción detallada del tipo de solicitud
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si el tipo está activo (1) o inactivo (0)
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación del registro
    UpdatedAt DATETIME NULL                     -- Fecha y hora de la última actualización
);

-- =============================================
-- Tabla: FoodAssistanceApplication (Solicitud Principal)
-- =============================================
-- Tabla principal que almacena las solicitudes de asistencia alimentaria.
-- Contiene la información básica de la solicitud y los datos de contacto
-- del adulto que completa el formulario.
CREATE TABLE FoodAssistanceApplication
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    ApplicationNumber NVARCHAR(50) NOT NULL,    -- Número único de solicitud (formato: YYYY-ESCUELA-SECUENCIAL)
    SchoolId INT NULL,                          -- ID de la escuela donde se presenta la solicitud, SE PUEDE NO INGRESAR 
    ApplicationTypeId INT NOT NULL,             -- Tipo de solicitud (referencia a ApplicationType)
    SchoolYear NVARCHAR(9) NOT NULL,            -- Año escolar en formato 'YYYY-YYYY'
    StreetAddress NVARCHAR(255) NOT NULL,       -- Dirección física del hogar
    ApartmentNumber NVARCHAR(50) NULL,          -- Número de apartamento (opcional)
    CityId INT NOT NULL,                        -- ID de la ciudad de residencia
    RegionId INT NOT NULL,                      -- ID de la región/estado
    ZipCode NVARCHAR(5) NOT NULL,               -- Código postal (5 dígitos)
    Phone NVARCHAR(50) NULL,                    -- Teléfono de contacto (opcional)
    Email NVARCHAR(100) NULL,                   -- Correo electrónico (opcional)
    CompletedByFirstName NVARCHAR(100) NOT NULL,    -- Nombre de quien completa el formulario
    CompletedByMiddleName NVARCHAR(100) NULL,       -- Segundo nombre (opcional)
    CompletedByFatherLastName NVARCHAR(100) NOT NULL, -- Apellido paterno
    CompletedByMotherLastName NVARCHAR(100) NOT NULL, -- Apellido materno
    CompletedDate DATE NOT NULL,                -- Fecha de completado del formulario
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pendiente', -- Estado actual de la solicitud
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si la solicitud está activa
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,                    -- Fecha y hora de última actualización
    FOREIGN KEY (SchoolId) REFERENCES School(Id),
    FOREIGN KEY (ApplicationTypeId) REFERENCES ApplicationType(Id),
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id)
);

-- =============================================
-- Tabla: ApplicationProgramParticipation (Participación en Programas)
-- =============================================
-- Registra la participación en programas de asistencia gubernamental.
-- Relaciona la solicitud con programas como SNAP, TANF o FDPIR.
CREATE TABLE ApplicationProgramParticipation
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    ApplicationId INT NOT NULL,                 -- ID de la solicitud relacionada
    ProgramId INT NOT NULL,                     -- ID del programa de asistencia
    CaseNumber NVARCHAR(50) NOT NULL,           -- Número de caso del programa
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si el registro está activo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,                    -- Fecha y hora de última actualización
    FOREIGN KEY (ApplicationId) REFERENCES FoodAssistanceApplication(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

-- =============================================
-- Tabla: IncomeType (Tipos de Ingreso)
-- =============================================
-- Catálogo de los diferentes tipos de ingresos que pueden reportarse.
-- Diferencia entre ingresos para niños y adultos.
CREATE TABLE IncomeType
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    Name NVARCHAR(255) NOT NULL,                -- Nombre del tipo de ingreso
    Description NVARCHAR(MAX) NULL,             -- Descripción detallada
    AppliesTo NVARCHAR(20) NOT NULL,            -- Indica si aplica a niños ('Child'), adultos ('Adult') o ambos ('Both')
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si el tipo está activo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL                     -- Fecha y hora de última actualización
);

-- =============================================
-- Tabla: IncomeFrequency (Frecuencia de Ingreso)
-- =============================================
-- Catálogo de frecuencias con las que se reciben los ingresos.
-- Incluye el factor de conversión para calcular el ingreso anual.
CREATE TABLE IncomeFrequency
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    Name NVARCHAR(255) NOT NULL,                -- Nombre de la frecuencia
    ConversionFactor DECIMAL(10,2) NOT NULL,    -- Factor para convertir a ingreso anual
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si la frecuencia está activa
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL                     -- Fecha y hora de última actualización
);

-- =============================================
-- Tabla: ApplicationReview (Revisión de Solicitud)
-- =============================================
-- Almacena las revisiones y decisiones sobre las solicitudes.
-- Incluye cálculos finales y determina la elegibilidad.
CREATE TABLE ApplicationReview
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    ApplicationId INT NOT NULL,                 -- ID de la solicitud revisada
    ReviewerId NVARCHAR(450) NOT NULL,          -- ID del usuario que realiza la revisión
    TotalAnnualIncome DECIMAL(10,2) NOT NULL,   -- Ingreso anual total calculado
    HouseholdSize INT NOT NULL,                 -- Número total de miembros del hogar
    EligibilityResult NVARCHAR(50) NOT NULL,    -- Resultado (Free, Reduced, Denied)
    ReviewDate DATE NOT NULL,                   -- Fecha de la revisión
    EffectiveDate DATE NOT NULL,                -- Fecha de inicio de la elegibilidad
    ExpirationDate DATE NOT NULL,               -- Fecha de expiración de la elegibilidad
    Notes NVARCHAR(MAX) NULL,                   -- Notas adicionales de la revisión
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si la revisión está activa
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,                    -- Fecha y hora de última actualización
    FOREIGN KEY (ApplicationId) REFERENCES FoodAssistanceApplication(Id),
    FOREIGN KEY (ReviewerId) REFERENCES AspNetUsers(Id)
);

-- =============================================
-- Tabla: Ethnicity (Etnicidad)
-- =============================================
-- Catálogo de grupos étnicos.
-- Utilizado para la clasificación demográfica opcional.
CREATE TABLE Ethnicity
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    Name NVARCHAR(100) NOT NULL,                -- Nombre del grupo étnico
    Description NVARCHAR(MAX) NULL,             -- Descripción detallada
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si el grupo étnico está activo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL                     -- Fecha y hora de última actualización
);

-- =============================================
-- Tabla: Race (Raza)
-- =============================================
-- Catálogo de grupos raciales.
-- Utilizado para la clasificación demográfica opcional.
CREATE TABLE Race
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    Name NVARCHAR(100) NOT NULL,                -- Nombre del grupo racial
    Description NVARCHAR(MAX) NULL,             -- Descripción detallada
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si el grupo racial está activo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL                     -- Fecha y hora de última actualización
);

-- =============================================
-- Tabla: MemberEthnicity (Relación Miembro-Etnicidad)
-- =============================================
-- Tabla de relación para asociar miembros con etnias.
-- Permite registrar la etnicidad de cada miembro.
CREATE TABLE MemberEthnicity
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    MemberId INT NOT NULL,                      -- ID del miembro del hogar
    EthnicityId INT NOT NULL,                   -- ID de la etnia
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si la relación está activa
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,                    -- Fecha y hora de última actualización
    FOREIGN KEY (MemberId) REFERENCES HouseholdMember(Id),
    FOREIGN KEY (EthnicityId) REFERENCES Ethnicity(Id)
);

-- =============================================
-- Tabla: MemberRace (Relación Miembro-Raza)
-- =============================================
-- Tabla de relación para asociar miembros con razas.
-- Permite registrar múltiples razas por miembro.
CREATE TABLE MemberRace
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    MemberId INT NOT NULL,                      -- ID del miembro del hogar
    RaceId INT NOT NULL,                        -- ID de la raza
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si la relación está activa
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,                    -- Fecha y hora de última actualización
    FOREIGN KEY (MemberId) REFERENCES HouseholdMember(Id),
    FOREIGN KEY (RaceId) REFERENCES Race(Id)
);

-- =============================================
-- Índices para Optimización de Consultas
-- =============================================
-- Índices estratégicos para mejorar el rendimiento
-- de las consultas más comunes en el sistema.

CREATE INDEX IX_FoodAssistanceApplication_SchoolYear ON FoodAssistanceApplication(SchoolYear);
CREATE INDEX IX_FoodAssistanceApplication_Status ON FoodAssistanceApplication(Status);
CREATE INDEX IX_HouseholdMember_Names ON HouseholdMember(FatherLastName, MotherLastName, FirstName);
CREATE INDEX IX_HouseholdMember_ApplicationId ON HouseholdMember(ApplicationId);
CREATE INDEX IX_ApplicationProgramParticipation_ProgramId ON ApplicationProgramParticipation(ProgramId);
CREATE INDEX IX_ApplicationProgramParticipation_ApplicationId ON ApplicationProgramParticipation(ApplicationId);
CREATE INDEX IX_ApplicationReview_ApplicationId ON ApplicationReview(ApplicationId);
CREATE INDEX IX_HouseholdMemberIncome_MemberId ON HouseholdMemberIncome(MemberId);
CREATE INDEX IX_MemberEthnicity_MemberId ON MemberEthnicity(MemberId);
CREATE INDEX IX_MemberRace_MemberId ON MemberRace(MemberId); 