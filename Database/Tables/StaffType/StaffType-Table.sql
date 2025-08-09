-- =============================================
-- Tabla: StaffType (Tipos de Staff)
-- =============================================
-- Registra los tipos principales de staff del sistema.
-- Solo existen dos tipos: Empleado y Miembro de la Junta

CREATE TABLE StaffType
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    NameEn NVARCHAR(100) NOT NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Índices para mejorar el rendimiento
CREATE INDEX IX_StaffType_Name ON StaffType(Name);
CREATE INDEX IX_StaffType_NameEn ON StaffType(NameEn);
CREATE INDEX IX_StaffType_DisplayOrder ON StaffType(DisplayOrder);
CREATE INDEX IX_StaffType_IsActive ON StaffType(IsActive);
CREATE INDEX IX_StaffType_CreatedAt ON StaffType(CreatedAt);


INSERT INTO StaffType
    (Name, NameEn, DisplayOrder, IsActive, CreatedAt, UpdatedAt)
VALUES
    ('Empleado', 'Employee', 1, 1, GETDATE(), NULL);

INSERT INTO StaffType
    (Name, NameEn, DisplayOrder, IsActive, CreatedAt, UpdatedAt)
VALUES
    ('Miembro de la Junta', 'Board Member', 2, 1, GETDATE(), NULL);