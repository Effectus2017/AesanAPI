CREATE TABLE DeliveryTypeProgram
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DeliveryTypeId INT NOT NULL,
    ProgramId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (DeliveryTypeId) REFERENCES DeliveryType(Id),
    FOREIGN KEY (ProgramId) REFERENCES Program(Id)
);

