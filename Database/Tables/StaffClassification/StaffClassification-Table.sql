-- =============================================
-- Tabla: StaffClassification (Clasificaciones de Staff)
-- =============================================
-- Registra las clasificaciones de staff del sistema.
-- Solo existen dos clasificaciones: Administrativo y Operacional

CREATE TABLE StaffClassification
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    NameEn NVARCHAR(100) NOT NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Índices para mejorar el rendimiento
CREATE INDEX IX_StaffClassification_Name ON StaffClassification(Name);
CREATE INDEX IX_StaffClassification_NameEn ON StaffClassification(NameEn);
CREATE INDEX IX_StaffClassification_SortOrder ON StaffClassification(SortOrder);
CREATE INDEX IX_StaffClassification_IsActive ON StaffClassification(IsActive);
CREATE INDEX IX_StaffClassification_CreatedAt ON StaffClassification(CreatedAt);

-- Insertar las clasificaciones iniciales
INSERT INTO StaffClassification
    (Name, NameEn, SortOrder, IsActive, CreatedAt, UpdatedAt)
VALUES
    ('Administrativo', 'Administrative', 1, 1, GETDATE(), NULL);

INSERT INTO StaffClassification
    (Name, NameEn, SortOrder, IsActive, CreatedAt, UpdatedAt)
VALUES
    ('Operacional', 'Operational', 2, 1, GETDATE(), NULL); 