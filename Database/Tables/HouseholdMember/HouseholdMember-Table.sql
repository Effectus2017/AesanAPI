-- =============================================
-- Tabla: HouseholdMember (Miembros del Hogar)
-- =============================================
-- Registra todos los miembros del hogar incluidos en la solicitud.
-- Incluye tanto niños como adultos, con campos específicos para
-- estudiantes y situaciones especiales.
CREATE TABLE HouseholdMember
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    -- Identificador único autoincremental
    ApplicationId INT NOT NULL,
    -- ID de la solicitud a la que pertenece
    FirstName NVARCHAR(100) NOT NULL,
    -- Nombre del miembro
    MiddleName NVARCHAR(100) NULL,
    -- Segundo nombre (opcional)
    FatherLastName NVARCHAR(100) NOT NULL,
    -- Apellido paterno
    MotherLastName NVARCHAR(100) NOT NULL,
    -- Apellido materno
    IsStudent BIT NOT NULL DEFAULT 0,
    -- Indica si es estudiante (1) o no (0)
    SchoolId INT NULL,
    -- ID de la escuela si es estudiante
    Grade NVARCHAR(10) NULL,
    -- Grado escolar si es estudiante
    IsFoster BIT NOT NULL DEFAULT 0,
    -- Indica si es niño en cuidado adoptivo temporal
    IsMigrant BIT NOT NULL DEFAULT 0,
    -- Indica si es migrante
    IsHomeless BIT NOT NULL DEFAULT 0,
    -- Indica si está sin hogar
    IsRunaway BIT NOT NULL DEFAULT 0,
    -- Indica si es fugitivo
    IsActive BIT NOT NULL DEFAULT 1,
    -- Indica si el registro está activo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,
    -- Fecha y hora de última actualización
    FOREIGN KEY (ApplicationId) REFERENCES FoodAssistanceApplication(Id),
    FOREIGN KEY (SchoolId) REFERENCES School(Id)
);