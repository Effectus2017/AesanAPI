CREATE TABLE CenterTypeProgram
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CenterTypeId INT NOT NULL,
    ProgramId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (CenterTypeId) REFERENCES CenterType(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);
