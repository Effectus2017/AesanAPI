-- =============================================
-- Tabla: FoodAssistanceApplication (Solicitud Principal)
-- =============================================
-- Tabla principal que almacena las solicitudes de asistencia alimentaria.
-- Contiene la información básica de la solicitud y los datos de contacto
-- del adulto que completa el formulario.
CREATE TABLE FoodAssistanceApplication
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    -- Identificador único autoincremental
    ApplicationNumber NVARCHAR(50) NOT NULL,
    -- Número único de solicitud (formato: YYYY-ESCUELA-SECUENCIAL)
    SchoolId INT NULL,
    -- ID de la escuela donde se presenta la solicitud, SE PUEDE NO INGRESAR 
    ApplicationTypeId INT NOT NULL,
    -- Tipo de solicitud (referencia a ApplicationType)
    SchoolYear NVARCHAR(9) NOT NULL,
    -- Año escolar en formato 'YYYY-YYYY'
    StreetAddress NVARCHAR(255) NOT NULL,
    -- Dirección física del hogar
    ApartmentNumber NVARCHAR(50) NULL,
    -- Número de apartamento (opcional)
    CityId INT NOT NULL,
    -- ID de la ciudad de residencia
    RegionId INT NOT NULL,
    -- ID de la región/estado
    ZipCode NVARCHAR(5) NOT NULL,
    -- Código postal (5 dígitos)
    Phone NVARCHAR(50) NULL,
    -- Teléfono de contacto (opcional)
    Email NVARCHAR(100) NULL,
    -- Correo electrónico (opcional)
    CompletedByFirstName NVARCHAR(100) NOT NULL,
    -- Nombre de quien completa el formulario
    CompletedByMiddleName NVARCHAR(100) NULL,
    -- Segundo nombre (opcional)
    CompletedByFatherLastName NVARCHAR(100) NOT NULL,
    -- Apellido paterno
    CompletedByMotherLastName NVARCHAR(100) NOT NULL,
    -- Apellido materno
    CompletedDate DATE NOT NULL,
    -- Fecha de completado del formulario
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pendiente',
    -- Estado actual de la solicitud
    IsActive BIT NOT NULL DEFAULT 1,
    -- Indica si la solicitud está activa
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,
    -- Fecha y hora de última actualización
    FOREIGN KEY (SchoolId) REFERENCES School(Id),
    FOREIGN KEY (ApplicationTypeId) REFERENCES ApplicationType(Id),
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id)
);