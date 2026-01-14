-- Agencias Auspiciadoras (Sponsoring Agencies)
CREATE TABLE Agency
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AgencyStatusId INT NOT NULL,
    CityId INT NOT NULL,
    PostalCityId INT NULL,
    RegionId INT NOT NULL,
    PostalRegionId INT NULL,
    Name NVARCHAR(255) NOT NULL,
    UieNumber BIGINT NOT NULL,
    EinNumber INT NOT NULL,
    SdrNumber BIGINT NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    ZipCode NVARCHAR(20) NULL,
    PostalAddress NVARCHAR(255) NOT NULL,
    PostalZipCode NVARCHAR(20) NULL,
    Phone NVARCHAR(20) NOT NULL DEFAULT '',
    Email NVARCHAR(255) NOT NULL DEFAULT '',
    Latitude REAL NOT NULL DEFAULT 0,
    Longitude REAL NOT NULL DEFAULT 0,
    ImageUrl NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsListable BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    AgencyCode NVARCHAR(50) NULL,
    -- EL valor por defecto es 0, porque es una agencia auspiciadora, 1 es unicamente para NUTRE para que no se muestre en la lista de agencias
    IsProprietary BIT NULL DEFAULT 0,
    -- Si la agencia es recurrente
    IsRecurrent BIT NULL DEFAULT 0,
    FOREIGN KEY (AgencyStatusId) REFERENCES AgencyStatus(Id),
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (PostalCityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id),
    FOREIGN KEY (PostalRegionId) REFERENCES Region(Id)
);
GO