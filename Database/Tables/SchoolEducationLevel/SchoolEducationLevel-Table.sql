-- Tabla: SchoolEducationLevel (Relación muchos-a-muchos entre School y EducationLevel)
-- Permite que una escuela pueda tener múltiples niveles educativos
-- Versión 1.0 - Estructura inicial

CREATE TABLE SchoolEducationLevel
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    SchoolId INT NOT NULL,
    EducationLevelId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    -- Constraint para evitar duplicados
    CONSTRAINT UK_SchoolEducationLevel_SchoolId_EducationLevelId UNIQUE (SchoolId, EducationLevelId)
);

-- Relaciones con Foreign Keys
ALTER TABLE SchoolEducationLevel
    ADD CONSTRAINT FK_SchoolEducationLevel_School FOREIGN KEY (SchoolId) REFERENCES School(Id);

ALTER TABLE SchoolEducationLevel
    ADD CONSTRAINT FK_SchoolEducationLevel_EducationLevel FOREIGN KEY (EducationLevelId) REFERENCES EducationLevel(Id);

-- Índices para optimizar consultas
CREATE INDEX IX_SchoolEducationLevel_SchoolId ON SchoolEducationLevel(SchoolId);
CREATE INDEX IX_SchoolEducationLevel_EducationLevelId ON SchoolEducationLevel(EducationLevelId);
CREATE INDEX IX_SchoolEducationLevel_IsActive ON SchoolEducationLevel(IsActive); 