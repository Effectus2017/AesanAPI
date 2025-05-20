-- =============================================
-- Tabla: Household (Hogar)
-- =============================================
-- Registra la información del hogar que presenta la solicitud.
-- Incluye detalles como dirección, contacto, fecha de completado, etc.
CREATE TABLE Household
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Street NVARCHAR(200) NOT NULL,
    -- Calle y número de casa
    Apartment NVARCHAR(50) NULL,
    -- Número de apartamento (opcional)
    CityId INT NOT NULL,
    -- FK a City
    RegionId INT NOT NULL,
    -- FK a Region
    ZipCode NVARCHAR(20) NOT NULL,
    -- Código Postal
    Phone NVARCHAR(50) NULL,
    -- Teléfono de contacto
    Email NVARCHAR(100) NULL,
    -- Correo electrónico (opcional)
    CompletedBy NVARCHAR(100) NOT NULL,
    -- Nombre del adulto que completa el formulario
    CompletedDate DATE NOT NULL,
    -- Fecha en que se llenó el formulario
    IsActive BIT NOT NULL DEFAULT 1,
    -- Auditoría: activo/inactivo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    -- Fecha de creación
    UpdatedAt DATETIME NULL,
    -- Fecha de última actualización
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id)
);