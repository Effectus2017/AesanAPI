CREATE TABLE KitchenTypeProgram
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KitchenTypeId INT NOT NULL,
    ProgramId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (KitchenTypeId) REFERENCES KitchenType(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id),
    CONSTRAINT [UX_KitchenTypeProgram_KitchenTypeId_ProgramId] UNIQUE (KitchenTypeId, ProgramId)
);