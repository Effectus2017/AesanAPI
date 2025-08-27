-- =============================================
-- Tabla: StaffRelationship (Relaciones entre Empleados)
-- =============================================
-- Registra las relaciones de parentesco entre empleados de la agencia.
-- Permite manejar conflictos de interés y relaciones familiares.

CREATE TABLE StaffRelationship
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    -- Identificador único autoincremental

    -- Empleados relacionados
    StaffId INT NOT NULL,
    -- Referencia al empleado principal
    RelatedStaffId INT NOT NULL,
    -- Referencia al empleado relacionado
    RelationshipTypeId INT NOT NULL,
    -- Referencia a OptionSelection con optionKey = 'staffRelationshipType'

    -- Estado y auditoría
    IsActive BIT NOT NULL DEFAULT 1,
    -- Indica si la relación está activa
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    -- Fecha y hora de creación
    UpdatedAt DATETIME NULL,
    -- Fecha y hora de última actualización

    -- Restricciones
    FOREIGN KEY (StaffId) REFERENCES Staff(Id),
    FOREIGN KEY (RelatedStaffId) REFERENCES Staff(Id),
    FOREIGN KEY (RelationshipTypeId) REFERENCES OptionSelection(Id),

    -- Restricción para evitar relaciones duplicadas
    CONSTRAINT UQ_StaffRelationship_Unique UNIQUE (StaffId, RelatedStaffId, RelationshipTypeId)
);

-- Índices para mejorar el rendimiento
CREATE INDEX IX_StaffRelationship_StaffId ON StaffRelationship(StaffId);
CREATE INDEX IX_StaffRelationship_RelatedStaffId ON StaffRelationship(RelatedStaffId);
CREATE INDEX IX_StaffRelationship_Type ON StaffRelationship(RelationshipTypeId);
CREATE INDEX IX_StaffRelationship_Active ON StaffRelationship(IsActive);
CREATE INDEX IX_StaffRelationship_CreatedAt ON StaffRelationship(CreatedAt);

-- Índice compuesto para consultas de relaciones bidireccionales
CREATE INDEX IX_StaffRelationship_Bidirectional ON StaffRelationship(StaffId, RelatedStaffId, IsActive);
