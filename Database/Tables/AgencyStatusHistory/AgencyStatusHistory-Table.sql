-- Historial de cambios de estado de agencia.
-- Solo IDs y fechas: AgencyId, StatusId, ChangedBy, ChangedAt, Justification.
CREATE TABLE AgencyStatusHistory
(
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    AgencyId INT NOT NULL,
    StatusId INT NOT NULL,
    ChangedBy NVARCHAR(450) NOT NULL,
    ChangedAt DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    Justification NVARCHAR(MAX) NULL,
    FOREIGN KEY (AgencyId) REFERENCES Agency(Id),
    FOREIGN KEY (StatusId) REFERENCES AgencyStatus(Id),
    FOREIGN KEY (ChangedBy) REFERENCES AspNetUsers(Id),
    INDEX IX_AgencyStatusHistory_AgencyId (AgencyId),
    INDEX IX_AgencyStatusHistory_ChangedAt (ChangedAt DESC)
);
GO
