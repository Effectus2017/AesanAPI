-- Tabla: SchoolFacility (Facilidades de la Escuela)
-- Versión 1.0 - Estructura extendida
CREATE TABLE SchoolFacility (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SchoolId INT NOT NULL,
    FacilityId INT NOT NULL, -- FK a catálogo de facilidades (ej. Almacén, Salón Comedor, etc.)
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (SchoolId) REFERENCES School(Id),
    FOREIGN KEY (FacilityId) REFERENCES Facility(Id)
); 