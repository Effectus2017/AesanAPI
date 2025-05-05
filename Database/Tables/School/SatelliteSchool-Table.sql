-- Tabla: SatelliteSchool (Relación Escuela Principal - Satélite)
-- Versión 1.0 - Estructura extendida
CREATE TABLE SatelliteSchool (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MainSchoolId INT NOT NULL, -- Escuela principal
    SatelliteSchoolId INT NOT NULL, -- Escuela satélite
    AssignmentDate DATE NULL, -- Fecha de asignación
    Status NVARCHAR(50) NULL, -- Estado de la relación (ej: Activa, Suspendida, etc.)
    Comment NVARCHAR(255) NULL, -- Comentario adicional
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (MainSchoolId) REFERENCES School(Id),
    FOREIGN KEY (SatelliteSchoolId) REFERENCES School(Id),
    CONSTRAINT UQ_SatelliteSchool UNIQUE (SatelliteSchoolId) -- Una satélite solo puede tener una principal
); 