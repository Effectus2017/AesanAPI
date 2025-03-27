CREATE TABLE AgencyUsers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL, -- Clave foránea que referencia al usuario que se asigna a la agencia
    AgencyId INT NOT NULL, -- Clave foránea que referencia a la agencia a la que se asigna el usuario
    IsOwner BIT NULL DEFAULT 0, -- Solo para el usuario que crea la agencia
    IsMonitor BIT NULL DEFAULT 0, -- Usuario que puede monitorear la agencia
    IsActive BIT NULL DEFAULT 1, -- Usuario activo en la agencia
    AssignedDate DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha de asignación del usuario a la agencia
    AssignedBy NVARCHAR(450) NOT NULL, -- Usuario que asignó el usuario a la agencia, generalmente es un administrador de NUTRE o administrador de la agencia (creador de la agencia)
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha de creación del registro
    UpdatedAt DATETIME NULL, -- Fecha de actualización del registro
    CONSTRAINT FK_AgencyUsers_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id), -- Clave foránea que referencia al usuario que se asigna a la agencia
    CONSTRAINT FK_AgencyUsers_AssignedBy FOREIGN KEY (AssignedBy) REFERENCES AspNetUsers(Id), -- Clave foránea que referencia al usuario que asignó el usuario a la agencia
    CONSTRAINT FK_AgencyUsers_Agencies FOREIGN KEY (AgencyId) REFERENCES Agency(Id) -- Clave foránea que referencia a la agencia a la que se asigna el usuario
);

GO

CREATE INDEX IX_AgencyUsers_UserId ON AgencyUsers (UserId);
CREATE INDEX IX_AgencyUsers_AgencyId ON AgencyUsers (AgencyId);
CREATE INDEX IX_AgencyUsers_AssignedBy ON AgencyUsers (AssignedBy);