-- =============================================
-- Tabla: SchoolChildGroup
-- Descripción: Grupos específicos de niños para servicios de alimentación
-- Fecha: 2025-01-15
-- =============================================

CREATE TABLE SchoolChildGroup
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    SchoolId INT NOT NULL,
    GroupName NVARCHAR(100) NOT NULL,
    -- "Grupo 1", "Grupo 2", etc.
    NumberOfChildren INT NOT NULL,
    -- Cantidad de niños en el grupo
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    CONSTRAINT FK_SchoolChildGroup_SchoolId FOREIGN KEY (SchoolId) REFERENCES School(Id) ON DELETE CASCADE,
    CONSTRAINT UK_SchoolChildGroup_SchoolId_GroupName UNIQUE (SchoolId, GroupName)
);

-- Índices para optimizar consultas
CREATE INDEX IX_SchoolChildGroup_SchoolId ON SchoolChildGroup(SchoolId);
CREATE INDEX IX_SchoolChildGroup_GroupName ON SchoolChildGroup(GroupName);
CREATE INDEX IX_SchoolChildGroup_SchoolId_GroupName ON SchoolChildGroup(SchoolId, GroupName);
