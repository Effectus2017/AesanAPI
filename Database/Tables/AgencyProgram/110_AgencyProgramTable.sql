
-- Asignación de programas a agencias
-- 110_AgencyProgramTable.sql
CREATE TABLE AgencyProgram
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AgencyId INT NOT NULL,
    ProgramId INT NOT NULL,
    UserId NVARCHAR(36) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);