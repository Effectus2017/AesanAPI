-- =============================================
-- Tabla: SchoolParticipant
-- Descripción: Relación muchos-a-muchos entre School y ParticipantType
-- Fecha: 2025-01-15
-- =============================================

CREATE TABLE SchoolParticipant
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    SchoolId INT NOT NULL,
    ParticipantTypeId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    CONSTRAINT UK_SchoolParticipant_SchoolId_ParticipantTypeId UNIQUE (SchoolId, ParticipantTypeId),
    CONSTRAINT FK_SchoolParticipant_SchoolId FOREIGN KEY (SchoolId) REFERENCES School(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SchoolParticipant_ParticipantTypeId FOREIGN KEY (ParticipantTypeId) REFERENCES OptionSelection(Id)
);

-- Índices para optimizar consultas
CREATE INDEX IX_SchoolParticipant_SchoolId ON SchoolParticipant(SchoolId);
CREATE INDEX IX_SchoolParticipant_ParticipantTypeId ON SchoolParticipant(ParticipantTypeId);
CREATE INDEX IX_SchoolParticipant_IsActive ON SchoolParticipant(IsActive);
