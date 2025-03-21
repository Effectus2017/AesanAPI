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
    StateDepartmentRegistration NVARCHAR(20) NOT NULL,
    UieNumber INT NOT NULL,
    EinNumber INT NOT NULL,
    SdrNumber INT NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    ZipCode INT NULL,
    PostalAddress NVARCHAR(255) NOT NULL,
    PostalZipCode INT NULL,
    Phone NVARCHAR(20) NOT NULL DEFAULT '',
    Email NVARCHAR(255) NOT NULL DEFAULT '',
    Latitude FLOAT NOT NULL DEFAULT 0,
    Longitude FLOAT NOT NULL DEFAULT 0,
    NonProfit BIT NULL DEFAULT 0,
    FederalFundsDenied BIT NULL DEFAULT 0,
    StateFundsDenied BIT NULL DEFAULT 0,
    OrganizedAthleticPrograms BIT NULL DEFAULT 0,
    AtRiskService BIT NULL DEFAULT 0,
    RejectionJustification NVARCHAR(MAX) NULL,
    ImageURL NVARCHAR(MAX) NULL,
    Comment NVARCHAR(MAX) NULL,
    AppointmentCoordinated BIT NULL,
    AppointmentDate DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsListable BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    AgencyCode NVARCHAR(50) NULL,
    IsPropietary BIT NULL DEFAULT 0,
    FOREIGN KEY (AgencyStatusId) REFERENCES AgencyStatus(Id),
    FOREIGN KEY (CityId) REFERENCES City(Id),
    FOREIGN KEY (PostalCityId) REFERENCES City(Id),
    FOREIGN KEY (RegionId) REFERENCES Region(Id),
    FOREIGN KEY (PostalRegionId) REFERENCES Region(Id)
);
GO

ALTER TABLE Agency
ADD AtRiskService BIT NULL DEFAULT 0;

ALTER TABLE Agency
ADD IsPropietary BIT NULL DEFAULT 0;

INSERT INTO Agency
    (Name, AgencyStatusId, CityId, PostalCityId, RegionId, PostalRegionId, SdrNumber, UieNumber, EinNumber, 
    Address, ZipCode, PostalAddress, PostalZipCode, Latitude, Longitude, Phone, Email, ImageURL, 
    IsActive, IsListable, AgencyCode, CreatedAt)
VALUES
    ('NUTRE', 1, 1, NULL, 1, NULL, 123456, 123456, 123456, 
    'Calle Nutre #456', 00123, 'Calle Nutre #456', 00123, 18.220833, -66.590149, '787-987-6543', 
    'contact@nutre.pr.gov', NULL, 1, 1, 'NUTRE-2025-001', GETDATE());
