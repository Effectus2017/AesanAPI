CREATE TABLE KitchenTypeGroupType
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KitchenTypeId INT NOT NULL,
    GroupTypeId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (KitchenTypeId) REFERENCES KitchenType(Id),
    FOREIGN KEY (GroupTypeId) REFERENCES GroupType(Id)
);
