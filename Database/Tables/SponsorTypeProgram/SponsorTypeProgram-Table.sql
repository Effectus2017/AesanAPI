CREATE TABLE SponsorTypeProgram
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SponsorTypeId INT NOT NULL,
    ProgramId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (SponsorTypeId) REFERENCES SponsorType(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

