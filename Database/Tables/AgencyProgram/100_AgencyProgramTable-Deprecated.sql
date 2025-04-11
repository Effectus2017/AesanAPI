-- 100_AgencyProgramTable-Deprecated.sql
-- Asignación de programas a agencias
-- Deprecated
-- Se reemplaza por la tabla 110_AgencyProgramTable
CREATE TABLE AgencyProgram
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AgencyId INT NOT NULL,
    ProgramId INT NOT NULL,
    UserId NVARCHAR(36) NULL,
    Comments NVARCHAR(MAX) NULL,
    AppointmentCoordinated BIT NULL DEFAULT 0,
    AppointmentDate DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);