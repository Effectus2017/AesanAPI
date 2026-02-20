-- =============================================
-- Tabla: StaffContractByClassification
-- =============================================
-- Almacena cargo, fechas de contrato y horario por clasificación (Administrativo/Operacional).
-- Un empleado con clasificación "Ambos" tiene dos filas (una por clasificación).

CREATE TABLE StaffContractByClassification
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    StaffId INT NOT NULL,
    StaffClassificationId INT NOT NULL,
    PositionId INT NOT NULL,
    ContractStartDate DATETIME NULL,
    ContractEndDate DATETIME NULL,
    ScheduleFrom TIME NULL,
    ScheduleTo TIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_StaffContractByClassification_Staff FOREIGN KEY (StaffId) REFERENCES Staff(Id),
    CONSTRAINT FK_StaffContractByClassification_StaffClassification FOREIGN KEY (StaffClassificationId) REFERENCES StaffClassification(Id),
    CONSTRAINT FK_StaffContractByClassification_OptionSelection FOREIGN KEY (PositionId) REFERENCES OptionSelection(Id),
    CONSTRAINT UQ_StaffContractByClassification_StaffId_StaffClassificationId UNIQUE (StaffId, StaffClassificationId)
);

CREATE INDEX IX_StaffContractByClassification_StaffId ON StaffContractByClassification(StaffId);
CREATE INDEX IX_StaffContractByClassification_StaffClassificationId ON StaffContractByClassification(StaffClassificationId);
CREATE INDEX IX_StaffContractByClassification_IsActive ON StaffContractByClassification(IsActive);
