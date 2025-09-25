-- Crear tabla de relación GroupTypeSiteLocation
-- Relaciona GroupType con SiteLocation (OptionSelection)
-- Versión: 1.0
-- Fecha: 2025-01-15

CREATE TABLE GroupTypeSiteLocation
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GroupTypeId INT NOT NULL,
    SiteLocationId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (GroupTypeId) REFERENCES GroupType(Id),
    FOREIGN KEY (SiteLocationId) REFERENCES OptionSelection(Id)
);