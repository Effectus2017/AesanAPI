
-- Programa al cual la agencia se esta inscribiendo en su intento de participacion

CREATE TABLE AgencyProgram
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AgencyId INT NOT NULL, -- id de la agencia
    ProgramId INT NOT NULL, -- id del programa
    UserId NVARCHAR(36) NULL, -- id del usuario que modificó el programa
    IsActive BIT NOT NULL DEFAULT 1, -- si el programa sigue activo para la agencia
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- fecha de creación del programa
    UpdatedAt DATETIME NULL, -- fecha de modificación del programa
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);