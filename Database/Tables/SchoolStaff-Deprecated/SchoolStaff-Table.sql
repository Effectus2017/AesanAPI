-- =============================================
-- Tabla: SchoolStaff (Relación Sitio - Empleado Asignado)
-- =============================================
-- Registra la relación entre sitios/escuelas y empleados del staff.
-- Permite asignar múltiples empleados a un sitio y viceversa.
-- Solo se pueden asignar empleados (StaffTypeId = 1), no miembros de junta.

CREATE TABLE SchoolStaff
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    -- Identificador único autoincremental

    -- Relaciones principales
    SchoolId INT NOT NULL,
    -- Referencia al sitio/escuela
    StaffId INT NOT NULL,
    -- Referencia al empleado del staff

    -- Información de la asignación
    AssignmentDate DATE NOT NULL DEFAULT GETDATE(),
    -- Fecha de asignación
    AssignmentTypeId INT NOT NULL DEFAULT 1,
    -- Referencia a OptionSelection con optionKey = 'staffAssignmentType'
    -- (ej: Principal, Secundario, Temporal, Apoyo)
    IsPrimary BIT NOT NULL DEFAULT 0,
    -- Indica si es el empleado principal del sitio
    StartDate DATE NULL,
    -- Fecha de inicio de la asignación
    EndDate DATE NULL,
    -- Fecha de finalización de la asignación
    Comments NVARCHAR(500) NULL,
    -- Comentarios adicionales

    -- Estado y auditoría
    IsActive BIT NOT NULL DEFAULT 1,
    -- Estado de la asignación
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    -- Fecha y hora de creación
    UpdatedAt DATETIME NULL
    -- Fecha y hora de última actualización
);

-- Restricciones de integridad referencial
ALTER TABLE SchoolStaff
    ADD CONSTRAINT FK_SchoolStaff_School FOREIGN KEY (SchoolId) REFERENCES School(Id);

ALTER TABLE SchoolStaff
    ADD CONSTRAINT FK_SchoolStaff_Staff FOREIGN KEY (StaffId) REFERENCES Staff(Id);

ALTER TABLE SchoolStaff
    ADD CONSTRAINT FK_SchoolStaff_AssignmentType FOREIGN KEY (AssignmentTypeId) REFERENCES OptionSelection(Id);

-- Restricción única: una persona no puede estar asignada al mismo sitio más de una vez activamente
ALTER TABLE SchoolStaff
    ADD CONSTRAINT UQ_SchoolStaff_Active UNIQUE (SchoolId, StaffId, IsActive);

-- Índices para mejorar el rendimiento
CREATE INDEX IX_SchoolStaff_SchoolId ON SchoolStaff(SchoolId);
CREATE INDEX IX_SchoolStaff_StaffId ON SchoolStaff(StaffId);
CREATE INDEX IX_SchoolStaff_AssignmentTypeId ON SchoolStaff(AssignmentTypeId);
CREATE INDEX IX_SchoolStaff_IsActive ON SchoolStaff(IsActive);
CREATE INDEX IX_SchoolStaff_IsPrimary ON SchoolStaff(IsPrimary);
CREATE INDEX IX_SchoolStaff_AssignmentDate ON SchoolStaff(AssignmentDate);
CREATE INDEX IX_SchoolStaff_StartDate ON SchoolStaff(StartDate);
CREATE INDEX IX_SchoolStaff_EndDate ON SchoolStaff(EndDate);
