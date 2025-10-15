-- Tabla: SchoolOperatingDays
-- Almacena días de funcionamiento individuales para cada escuela
-- Permite manejar horarios específicos y excepciones (fines de semana, días excluidos)
-- Versión 1.0 - Estructura inicial
-- Última actualización: 2025-01-15

CREATE TABLE SchoolOperatingDays
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SchoolId INT NOT NULL,
    OperatingDate DATE NOT NULL,
    StartTime TIME NULL,
    -- Horario de inicio (ej: 08:00:00)
    EndTime TIME NULL,
    -- Horario de fin (ej: 16:00:00)
    IsWeekendOverride BIT NOT NULL DEFAULT 0,
    -- true si es fin de semana pero funciona
    IsExcluded BIT NOT NULL DEFAULT 0,
    -- true si es día hábil pero no funciona
    Comment NVARCHAR(500) NULL,
    -- Comentario opcional para el día
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Relaciones
ALTER TABLE SchoolOperatingDays
    ADD CONSTRAINT FK_SchoolOperatingDays_School 
        FOREIGN KEY (SchoolId) REFERENCES School(Id);

-- Índices para optimización
CREATE INDEX IX_SchoolOperatingDays_SchoolId ON SchoolOperatingDays(SchoolId);
CREATE INDEX IX_SchoolOperatingDays_OperatingDate ON SchoolOperatingDays(OperatingDate);
CREATE INDEX IX_SchoolOperatingDays_SchoolId_OperatingDate ON SchoolOperatingDays(SchoolId, OperatingDate);

-- Restricción única para evitar duplicados
ALTER TABLE SchoolOperatingDays
    ADD CONSTRAINT UQ_SchoolOperatingDays_School_Date 
        UNIQUE (SchoolId, OperatingDate);