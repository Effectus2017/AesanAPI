-- =============================================
-- Tabla: StaffType (Tipos de Staff)
-- =============================================
-- Registra todos los tipos de staff del sistema.
-- Un tipo de staff define la categoría o rol del personal.

CREATE TABLE StaffType
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
CREATE INDEX IX_StaffType_Name ON StaffType(Name);
CREATE INDEX IX_StaffType_NameEn ON StaffType(NameEn);
CREATE INDEX IX_StaffType_SortOrder ON StaffType(SortOrder);
CREATE INDEX IX_StaffType_IsActive ON StaffType(IsActive);
CREATE INDEX IX_StaffType_CreatedAt ON StaffType(CreatedAt);


INSERT INTO StaffType
    (Name, NameEn, SortOrder, IsActive, CreatedAt, UpdatedAt)
VALUES
    ('Administrativo', 'Administrative', 1, 1, GETDATE(), NULL);

INSERT INTO StaffType
    (Name, NameEn, SortOrder, IsActive, CreatedAt, UpdatedAt)
VALUES
    ('Operacional', 'Operational', 2, 1, GETDATE(), NULL);
