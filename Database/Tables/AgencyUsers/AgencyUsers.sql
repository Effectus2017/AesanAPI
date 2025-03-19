CREATE TABLE AgencyUsers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    AgencyId INT NOT NULL,
    IsOwner BIT NULL DEFAULT 0,
    IsMonitor BIT NULL DEFAULT 0,
    IsActive BIT NULL DEFAULT 1,
    AssignedDate DATETIME NOT NULL DEFAULT GETDATE(),
    AssignedBy NVARCHAR(450) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_AgencyUsers_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_AgencyUsers_AssignedBy FOREIGN KEY (AssignedBy) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_AgencyUsers_Agencies FOREIGN KEY (AgencyId) REFERENCES Agency(Id)
);

GO

CREATE INDEX IX_AgencyUsers_UserId ON AgencyUsers (UserId);
CREATE INDEX IX_AgencyUsers_AgencyId ON AgencyUsers (AgencyId);
CREATE INDEX IX_AgencyUsers_AssignedBy ON AgencyUsers (AssignedBy);