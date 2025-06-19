-- DEPRECATED: Esta versión de la tabla School ha sido reemplazada por una nueva versión. No modificar ni usar para nuevas migraciones
-- Tabla: School (Sitios/Escuelas)
-- Versión 2.0 - Estructura extendida y actualizada según nuevos requerimientos
CREATE TABLE School
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AgencyId INT NOT NULL,
    Name NVARCHAR(255) NOT NULL,

    StartDate DATE NULL,
    Address NVARCHAR(255) NOT NULL,
    CityId INT NOT NULL,
    RegionId INT NOT NULL,
    ZipCode NVARCHAR(20) NOT NULL,
    Latitude FLOAT NULL,
    Longitude FLOAT NULL,

    PostalAddress NVARCHAR(255) NULL,
    PostalCityId INT NULL,
    PostalRegionId INT NULL,
    PostalZipCode NVARCHAR(20) NULL,
    SameAsPhysicalAddress BIT NULL,

    OrganizationTypeId INT NOT NULL,
    CenterTypeId INT NULL,
    NonProfit BIT NULL,
    BaseYear INT NULL,
    RenewalYear INT NULL,
    EducationLevelId INT NOT NULL,
    OperatingDays INT NULL,
    KitchenTypeId INT NULL,
    GroupTypeId INT NULL,
    DeliveryTypeId INT NULL,
    SponsorTypeId INT NULL,
    ApplicantTypeId INT NULL,
    ResidentialTypeId INT NULL,
    OperatingPolicyId INT NULL,
    HasWarehouse BIT NULL,
    HasDiningRoom BIT NULL,
    AdministratorAuthorizedName NVARCHAR(255) NULL,
    SitePhone NVARCHAR(20) NULL,
    Extension NVARCHAR(10) NULL,
    MobilePhone NVARCHAR(20) NULL,
    Breakfast BIT NULL,
    BreakfastFrom TIME NULL,
    BreakfastTo TIME NULL,
    Lunch BIT NULL,
    LunchFrom TIME NULL,
    LunchTo TIME NULL,
    Snack BIT NULL,
    SnackFrom TIME NULL,
    SnackTo TIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    InactiveJustification NVARCHAR(500) NULL,
    InactiveDate DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

-- Relaciones
ALTER TABLE School
    ADD CONSTRAINT FK_School_City FOREIGN KEY (CityId) REFERENCES City(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_Region FOREIGN KEY (RegionId) REFERENCES Region(Id);

-- OrganizationType se maneja en su tabla OrganizationType
ALTER TABLE School
    ADD CONSTRAINT FK_School_OrganizationType FOREIGN KEY (OrganizationTypeId) REFERENCES OrganizationType(Id);

-- EducationLevel se maneja en su tabla EducationLevel
ALTER TABLE School
    ADD CONSTRAINT FK_School_EducationLevel FOREIGN KEY (EducationLevelId) REFERENCES EducationLevel(Id);

-- KitchenType se maneja en su tabla KitchenType
ALTER TABLE School
    ADD CONSTRAINT FK_School_KitchenType FOREIGN KEY (KitchenTypeId) REFERENCES KitchenType(Id);

-- GroupType se maneja en su tabla GroupType
ALTER TABLE School
    ADD CONSTRAINT FK_School_GroupType FOREIGN KEY (GroupTypeId) REFERENCES GroupType(Id);

-- DeliveryType se maneja en su tabla DeliveryType
ALTER TABLE School
    ADD CONSTRAINT FK_School_DeliveryType FOREIGN KEY (DeliveryTypeId) REFERENCES DeliveryType(Id);

-- SponsorType se maneja en su tabla SponsorType
ALTER TABLE School
    ADD CONSTRAINT FK_School_SponsorType FOREIGN KEY (SponsorTypeId) REFERENCES SponsorType(Id);

-- ApplicantType se maneja en OptionSelection (Fix)
ALTER TABLE School
    ADD CONSTRAINT FK_School_ApplicantType FOREIGN KEY (ApplicantTypeId) REFERENCES OptionSelection(Id);

-- ResidentialType se maneja en OptionSelection (Fix)
ALTER TABLE School
    ADD CONSTRAINT FK_School_ResidentialType FOREIGN KEY (ResidentialTypeId) REFERENCES OptionSelection(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_OperatingPolicy FOREIGN KEY (OperatingPolicyId) REFERENCES OperatingPolicy(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_PostalCity FOREIGN KEY (PostalCityId) REFERENCES City(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_PostalRegion FOREIGN KEY (PostalRegionId) REFERENCES Region(Id);

ALTER TABLE School
    ADD CONSTRAINT FK_School_Agency FOREIGN KEY (AgencyId) REFERENCES Agency(Id);


-- Facilidades (Almacén, Salón Comedor) se gestionan en SchoolFacility
-- Los catálogos KitchenType, GroupType, DeliveryType, SponsorType, ApplicantType, OperatingPolicy deben crearse si no existen. 

ALTER TABLE School
    ADD CONSTRAINT FK_School_CenterType FOREIGN KEY (CenterTypeId) REFERENCES CenterType(Id);