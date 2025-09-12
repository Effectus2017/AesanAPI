-- =============================================
-- Tabla: SchoolService
-- Descripción: Servicios de alimentación para escuelas
-- Fecha: 2025-01-15
-- =============================================

CREATE TABLE SchoolService
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    SchoolId INT NOT NULL,
    ChildGroupId INT NULL,
    -- NULL = servicio general, NOT NULL = servicio por grupo específico
    Breakfast BIT NULL,
    BreakfastFrom TIME NULL,
    BreakfastTo TIME NULL,
    Lunch BIT NULL,
    LunchFrom TIME NULL,
    LunchTo TIME NULL,
    SnackAM BIT NULL,
    -- Merienda AM
    SnackAMFrom TIME NULL,
    SnackAMTo TIME NULL,
    Dinner BIT NULL,
    DinnerFrom TIME NULL,
    DinnerTo TIME NULL,
    SnackPM BIT NULL,
    -- Merienda PM
    SnackPMFrom TIME NULL,
    SnackPMTo TIME NULL,
    SnackNight BIT NULL,
    SnackNightFrom TIME NULL,
    SnackNightTo TIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    CONSTRAINT FK_SchoolService_SchoolId FOREIGN KEY (SchoolId) REFERENCES School(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SchoolService_ChildGroupId FOREIGN KEY (ChildGroupId) REFERENCES OptionSelection(Id) ON DELETE CASCADE
);

-- Índices para optimizar consultas
CREATE INDEX IX_SchoolService_SchoolId ON SchoolService(SchoolId);
CREATE INDEX IX_SchoolService_ChildGroupId ON SchoolService(ChildGroupId);
CREATE INDEX IX_SchoolService_SchoolId_ChildGroupId ON SchoolService(SchoolId, ChildGroupId);