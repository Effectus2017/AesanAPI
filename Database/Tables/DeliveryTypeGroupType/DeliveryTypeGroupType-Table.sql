CREATE TABLE DeliveryTypeGroupType
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DeliveryTypeId INT NOT NULL,
    GroupTypeId INT NOT NULL,
    RequiresPermission BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (DeliveryTypeId) REFERENCES DeliveryType(Id),
    FOREIGN KEY (GroupTypeId) REFERENCES GroupType(Id)
);

