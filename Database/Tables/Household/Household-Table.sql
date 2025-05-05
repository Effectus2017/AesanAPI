-- =============================================
-- Tabla: Household (Hogar)
-- =============================================
-- Registra la información del hogar que presenta la solicitud.
-- Incluye detalles como dirección, contacto, fecha de completado, etc.
CREATE TABLE Household (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Street NVARCHAR(200) NOT NULL,         -- Calle y número de casa
    Apartment NVARCHAR(50) NULL,           -- Número de apartamento (opcional)
    CityId INT NOT NULL,                   -- FK a City
    RegionId INT NOT NULL,                 -- FK a Region
    ZipCode NVARCHAR(20) NOT NULL,         -- Código Postal
    Phone NVARCHAR(50) NULL,               -- Teléfono de contacto
    Email NVARCHAR(100) NULL,              -- Correo electrónico (opcional)
    CompletedBy NVARCHAR(100) NOT NULL,    -- Nombre del adulto que completa el formulario
    CompletedDate DATE NOT NULL,           -- Fecha en que se llenó el formulario
    IsActive BIT NOT NULL DEFAULT 1,       -- Auditoría: activo/inactivo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha de creación
    UpdatedAt DATETIME NULL,               -- Fecha de última actualización
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id)
); 


-- =============================================
-- Tabla: HouseholdMember (Miembros del Hogar)
-- =============================================
-- Registra todos los miembros del hogar incluidos en la solicitud.
-- Incluye tanto niños como adultos, con campos específicos para
-- estudiantes y situaciones especiales.
CREATE TABLE HouseholdMember
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    ApplicationId INT NOT NULL,                 -- ID de la solicitud a la que pertenece
    FirstName NVARCHAR(100) NOT NULL,           -- Nombre del miembro
    MiddleName NVARCHAR(100) NULL,              -- Segundo nombre (opcional)
    FatherLastName NVARCHAR(100) NOT NULL,      -- Apellido paterno
    MotherLastName NVARCHAR(100) NOT NULL,      -- Apellido materno
    IsStudent BIT NOT NULL DEFAULT 0,           -- Indica si es estudiante (1) o no (0)
    SchoolId INT NULL,                          -- ID de la escuela si es estudiante
    Grade NVARCHAR(10) NULL,                    -- Grado escolar si es estudiante
    IsFoster BIT NOT NULL DEFAULT 0,            -- Indica si es niño en cuidado adoptivo temporal
    IsMigrant BIT NOT NULL DEFAULT 0,           -- Indica si es migrante
    IsHomeless BIT NOT NULL DEFAULT 0,          -- Indica si está sin hogar
    IsRunaway BIT NOT NULL DEFAULT 0,           -- Indica si es fugitivo
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si el registro está activo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,                    -- Fecha y hora de última actualización
    FOREIGN KEY (ApplicationId) REFERENCES FoodAssistanceApplication(Id),
    FOREIGN KEY (SchoolId) REFERENCES School(Id)
);

-- =============================================
-- Tabla: HouseholdMemberIncome (Ingresos de Miembros)
-- =============================================
-- Registra los ingresos de cada miembro del hogar.
-- Permite múltiples ingresos por miembro con diferentes tipos y frecuencias.
CREATE TABLE HouseholdMemberIncome
(
    Id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único autoincremental
    MemberId INT NOT NULL,                      -- ID del miembro del hogar
    IncomeTypeId INT NOT NULL,                  -- Tipo de ingreso
    Amount DECIMAL(10,2) NOT NULL,              -- Monto del ingreso
    FrequencyId INT NOT NULL,                   -- Frecuencia con que se recibe
    IsActive BIT NOT NULL DEFAULT 1,            -- Indica si el registro está activo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,                    -- Fecha y hora de última actualización
    FOREIGN KEY (MemberId) REFERENCES HouseholdMember(Id),
    FOREIGN KEY (IncomeTypeId) REFERENCES IncomeType(Id),
    FOREIGN KEY (FrequencyId) REFERENCES IncomeFrequency(Id)
);